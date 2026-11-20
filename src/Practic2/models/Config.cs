namespace Practic2.Models;

public class Config
{
    public string PackageName = "";
    public string Location = "";
    public string Mode = "";
    public string PackageVersion = "";
    public string OutputFile = "";
    public bool AsciiTree;
    public int MaxDepth;

    public Config(
        string packageName, string location, string mode, string packageVersion,
        string outputFile, bool asciiTree, int maxDepth)
    {
        PackageName = packageName;
        Location = location;
        Mode = mode;
        PackageVersion = packageVersion;
        OutputFile = outputFile;
        AsciiTree = asciiTree;
        MaxDepth = maxDepth;
    }

    public Config() {}

    public List<string> Validate()
    {
        List<string> errors = [];

        if(string.IsNullOrWhiteSpace(PackageName))
            errors.Add("package_name: имя пакета не может быть пустым");

        if(string.IsNullOrWhiteSpace(PackageVersion))
            errors.Add("package_version: версия пакета не может быть пустой");

        ValidateRepository(errors);

        if (string.IsNullOrWhiteSpace(OutputFile) || !OutputFile.EndsWith(".svg"))
            errors.Add("output_file: имя файла должно оканчиваться на .svg");

        if (MaxDepth < 0)
            errors.Add($"max_depth: глубина не может быть меньше 0, получено {MaxDepth}");

        return errors;
    }
    internal static bool IsTestPackageName(string name) =>
        name.Length > 0 && name.All(char.IsAsciiLetterUpper);

    private void ValidateRepository(List<string> errors)
    {
        if (Mode != "url" && Mode != "test")
            errors.Add("test_mode: допустимые значения - \"url\" или \"test\"");

        if (Mode == "url" && !Location.StartsWith("http://") && !Location.StartsWith("https://"))
            errors.Add("location: для режима url нужен адрес, начинающийся с http:// или https://");

        if (Mode == "test" && !IsTestPackageName(PackageName))
            errors.Add($"package_name: в режиме test имя пакета должно состоять из больших латинских букв, " +
                       $"получено '{PackageName}'");

        if (Mode == "test" && !File.Exists(Location))
            errors.Add($"location: файл тестового репозитория не найден: '{Location}'");
    }
}
