namespace Practic2.Models;

public class Executor
{
    private Config? _config;
    private readonly DependencyGraphTraversal _traversal = new();
    private Func<string, List<string>?>? _resolver;

    public void Step1(string path)
    {
        Config config;
        try
        {
            config = TomlReader.ReadToml(path);
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine($"Ошибка чтения конфига: файл '{path}' не найден");
            return;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Ошибка чтения конфига '{path}': {e.Message}");
            return;
        }

        var errors = config.Validate();
        if (errors.Count > 0)
        {
            Console.Error.WriteLine("Ошибки в файле:");
            foreach (var error in errors)
                Console.Error.WriteLine($"  - {error}");
            return;
        }

        Console.WriteLine($"package_name = {config.PackageName}");
        Console.WriteLine($"repository = {config.Location}");
        Console.WriteLine($"test_mode = {config.Mode}");
        Console.WriteLine($"package_version = {config.PackageVersion}");
        Console.WriteLine($"output_file = {config.OutputFile}");
        Console.WriteLine($"ascii_tree = {config.AsciiTree}");
        Console.WriteLine($"max_depth = {config.MaxDepth}");

        _config = config;
    }

    public void Step2(string suite, string component, string arch)
    {
        if (_config == null)
            return;

        Config config = _config;

        if (config.Mode != "url")
            return;

        List<string>? dependencies;
        try
        {
            dependencies = AptDependencyFetcher.GetDirectDependencies(
                config.Location, suite, component, arch, config.PackageName, config.PackageVersion);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            Console.WriteLine($"\nНе удалось загрузить индекс из {config.Location} ({e.Message})");
            return;
        }

        if (dependencies == null)
        {
            Console.WriteLine($"\nПакет {config.PackageName} {config.PackageVersion} не найден в индексе");
            return;
        }

        Console.WriteLine($"\nПрямые зависимости {config.PackageName} {config.PackageVersion}:");
        foreach (string name in dependencies)
            Console.WriteLine($"  {name}");
    }

    public void Step3(string suite, string component, string arch)
    {
        if (_config == null)
            return;

        Config config = _config;

        bool testMode = config.Mode == "test";
        _resolver = testMode
            ? ReadTestRepository(config)
            : ReadAptRepository(config, suite, component, arch);
        if (_resolver == null)
            return;

        var edges = _traversal.Traverse(config.PackageName, _resolver, config.MaxDepth);
        string package = testMode ? config.PackageName : $"{config.PackageName} {config.PackageVersion}";
        if (edges.Count == 0)
        {
            Console.WriteLine($"\nУ пакета {package} нет зависимостей");
            return;
        }

        string source = testMode ? $" из {config.Location}" : "";
        Console.WriteLine($"\nГраф зависимостей {package}{source} (глубина {config.MaxDepth}):");
        foreach (var (from, to, depth) in edges)
            Console.WriteLine($"  {new string(' ', depth * 2)}{from} -> {to}");
    }

    private static Func<string, List<string>?>? ReadTestRepository(Config config)
    {
        var graph = new Dictionary<string, List<string>>();
        int lineNumber = 0;
        foreach (string rawLine in File.ReadLines(config.Location))
        {
            lineNumber++;
            string line = rawLine.Trim();
            if (line == "" || line.StartsWith('#'))
                continue;

            int colon = line.IndexOf(':');
            if (colon < 0)
            {
                Console.WriteLine($"\nОшибка в {config.Location}, строка {lineNumber}: нет ':' в '{line}'");
                return null;
            }

            string name = line[..colon].Trim();
            var deps = line[(colon + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            string? invalid = deps.Prepend(name).FirstOrDefault(package => !Config.IsTestPackageName(package));
            if (invalid != null)
            {
                Console.WriteLine($"\nОшибка в {config.Location}, строка {lineNumber}: " +
                    $"имя пакета '{invalid}' должно состоять из больших латинских букв");
                return null;
            }
            graph[name] = deps;
        }

        if (graph.ContainsKey(config.PackageName))
            return name => graph.GetValueOrDefault(name);

        Console.WriteLine($"\nПакет {config.PackageName} не найден в тестовом репозитории {config.Location}");
        return null;
    }

    private static Func<string, List<string>?>? ReadAptRepository(
        Config config, string suite, string component, string arch)
    {
        List<AptPackage> index;
        try
        {
            index = AptDependencyFetcher.LoadIndex(config.Location, suite, component, arch);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            Console.WriteLine($"\nНе удалось загрузить индекс из {config.Location} ({e.Message})");
            return null;
        }

        var resolver = AptDependencyFetcher.CreateDependencyResolver(index, config.PackageName, config.PackageVersion);
        if (resolver == null)
        {
            Console.WriteLine($"\nПакет {config.PackageName} {config.PackageVersion} не найден в индексе");
            return null;
        }

        return resolver;
    }

    public void Step4()
    {
        if (_config == null || _resolver == null)
            return;

        var cycles = new List<string>();
        var order = _traversal.GetLoadOrder(_config.PackageName, _resolver, _config.MaxDepth, cycles);

        foreach (string cycle in cycles)
            Console.WriteLine($"\nВнимание: циклическая зависимость {cycle}, порядок условный");

        Console.WriteLine($"\nПорядок загрузки {_config.PackageName}:");
        for (int i = 0; i < order.Count; i++)
            Console.WriteLine($"  {i + 1}. {order[i]}");
    }

    public void Step5()
    {
        if (_config == null || _resolver == null)
            return;

        Config config = _config;
        var edges = _traversal.Traverse(config.PackageName, _resolver, config.MaxDepth);

        var graph = new Dictionary<string, List<string>>();
        foreach (var (from, to, _) in edges)
        {
            if (!graph.TryGetValue(from, out var deps))
                graph[from] = deps = [];
            deps.Add(to);
        }

        string dot = GraphVisualizer.ToDot(config.PackageName, edges);
        Console.WriteLine($"\nГраф на языке Graphviz:\n{dot}");

        GraphVisualizer.SaveSvg(dot, config.OutputFile);

        if (config.AsciiTree)
        {
            Console.WriteLine($"\nДерево зависимостей {config.PackageName}:");
            Console.WriteLine(config.PackageName);
            GraphVisualizer.PrintAsciiTree(config.PackageName, graph, "", 0, config.MaxDepth, [config.PackageName]);
        }
    }
}
