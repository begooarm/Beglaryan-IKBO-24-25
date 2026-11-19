using practic2.models;

namespace practic2;

public class Practic2
{
    public static void Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : "configs/mainConfig.toml";

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

        // dotnet run -- <конфиг> [suite] [component] [arch]
        string suite = args.Length > 1 ? args[1] : "jammy";
        string component = args.Length > 2 ? args[2] : "main";
        string arch = args.Length > 3 ? args[3] : "amd64";

        List<string>? dependencies;
        try
        {
            dependencies = AptDependencyFetcher.GetDirectDependencies(
                config.Location, suite, component, arch, config.PackageName, config.PackageVersion);
        }
        catch (HttpRequestException e)
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

        Console.WriteLine("\nОграничение: версионные условия вида (>= 1.2) и альтернативы a | b (берётся первый вариант) не учитываются.");
    }
}
