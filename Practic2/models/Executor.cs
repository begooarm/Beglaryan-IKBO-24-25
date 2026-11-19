namespace practic2.models;

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

        if (config.Mode != "url")
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
                    return;
                }

                string name = line[..colon].Trim();
                var deps = line[(colon + 1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (string package in deps.Prepend(name))
                {
                    if (package.Length == 0 || !package.All(c => c >= 'A' && c <= 'Z'))
                    {
                        Console.WriteLine($"\nОшибка в {config.Location}, строка {lineNumber}: имя пакета '{package}' должно состоять из больших латинских букв");
                        return;
                    }
                }

                graph[name] = deps;
            }

            if (!graph.ContainsKey(config.PackageName))
            {
                Console.WriteLine($"\nПакет {config.PackageName} не найден в тестовом репозитории {config.Location}");
                return;
            }

            _resolver = name => graph.TryGetValue(name, out var deps) ? deps : null;
            var testTraversal = _traversal.Traverse(config.PackageName, _resolver, config.MaxDepth);

            if (testTraversal.Count == 0)
            {
                Console.WriteLine($"\nУ пакета {config.PackageName} нет зависимостей");
                return;
            }

            Console.WriteLine($"\nГраф зависимостей {config.PackageName} из {config.Location} (глубина {config.MaxDepth}):");
            foreach (var (from, to, depth) in testTraversal)
                Console.WriteLine($"  {new string(' ', depth * 2)}{from} -> {to}");
            return;
        }

        List<AptPackage> index;
        try
        {
            index = AptDependencyFetcher.LoadIndex(config.Location, suite, component, arch);
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            Console.WriteLine($"\nНе удалось загрузить индекс из {config.Location} ({e.Message})");
            return;
        }

        var resolver = AptDependencyFetcher.CreateDependencyResolver(index, config.PackageName, config.PackageVersion);
        if (resolver == null)
        {
            Console.WriteLine($"\nПакет {config.PackageName} {config.PackageVersion} не найден в индексе");
            return;
        }

        _resolver = resolver;
        var traversal = _traversal.Traverse(config.PackageName, resolver, config.MaxDepth);
        if (traversal.Count == 0)
        {
            Console.WriteLine($"\nУ пакета {config.PackageName} {config.PackageVersion} нет зависимостей");
            return;
        }

        Console.WriteLine($"\nГраф зависимостей {config.PackageName} {config.PackageVersion} (глубина {config.MaxDepth}):");
        foreach (var (from, to, depth) in traversal)
            Console.WriteLine($"  {new string(' ', depth * 2)}{from} -> {to}");
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
}
