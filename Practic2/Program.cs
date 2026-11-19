using practic2.models;

namespace practic2;

public class Practic2
{
    // dotnet run -- <конфиг> [suite] [component] [arch]
    public static void Main(string[] args)
    {
        string path = args.Length > 0 ? args[0] : "configs/mainConfig.toml";
        string suite = args.Length > 1 ? args[1] : "jammy";
        string component = args.Length > 2 ? args[2] : "main";
        string arch = args.Length > 3 ? args[3] : "amd64";

        var executor = new Executor();

        executor.Step1(path);
        executor.Step2(suite, component, arch);
        executor.Step3(suite, component, arch);
        executor.Step4();
    }
}
