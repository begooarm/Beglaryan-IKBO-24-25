using Practic2.Models;

namespace Practic2;

public class Program
{
    /// <summary>Runs package dependency analysis with optional repository and configuration arguments.</summary>
    public static void Main(string[] args)
    {
        string path = args.ElementAtOrDefault(0) ?? "configs/mainConfig.toml";
        string suite = args.ElementAtOrDefault(1) ?? "jammy";
        string component = args.ElementAtOrDefault(2) ?? "main";
        string arch = args.ElementAtOrDefault(3) ?? "amd64";

        var executor = new Executor();

        executor.Step1(path);
        executor.Step2(suite, component, arch);
        executor.Step3(suite, component, arch);
        executor.Step4();
        executor.Step5();
    }
}
