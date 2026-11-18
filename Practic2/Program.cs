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
    }
}
