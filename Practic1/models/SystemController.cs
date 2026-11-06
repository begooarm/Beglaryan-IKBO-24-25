namespace practic1.models;
using System.Collections.Generic;

public class SystemController {
    public string? vfsPath;
    public string? startScriptPath;
    public string? configFilePath;

    public Dictionary<string, string> config = [];

    public Dictionary<string, List<Action<string[]>>> commands;

    public SystemController()
    {
        commands = new()
        {
            ["ls"] = [ls],
            ["cd"] = [cd],
            ["exit"] = [exit],
            ["setup"] = [setup, StartFile],
            ["conf-dump"] = [ConfDump],
        };
    }

    public static string GetUsername() { return Environment.UserName; }
    public static string GetHostname() { return Environment.MachineName; }

    public void ls(params string[] args) => Console.WriteLine("ls command");
    public void cd(params string[] args) => Console.WriteLine("cd command");

    public void ConfDump(params string[] args)
    {
        Console.WriteLine("Emulators variables:");
        Console.WriteLine($"userName={GetUsername()}");
        Console.WriteLine($"hostname={GetHostname()}");
        Console.WriteLine($"vfsPath={vfsPath}");
        Console.WriteLine($"startScriptPath={startScriptPath}");
        Console.WriteLine($"configPath={configFilePath}");
    }

    public void exit(params string[] args)
    {
        Environment.Exit(0);
    }

    public void setup(params string[] args)
    {
        if (args.Length == 0) return;
        
        for(int i = 0; i < args.Length; i++)
        {
            if(args[i] == "--vfsPath") { vfsPath = args[i+1]; }
            if(args[i] == "--startScriptPath") { startScriptPath = args[i+1]; }
            if(args[i] == "--configPath") { configFilePath = args[i+1]; }
        }

        if(configFilePath != null)
            Validator.CheckIniConfig(configFilePath, ref vfsPath, ref startScriptPath);

        Console.WriteLine("All paths were successfully saved");

        Console.WriteLine($"VFS path is: {vfsPath}");
        Console.WriteLine($"Start sctipt path is: {startScriptPath}");
        Console.WriteLine($"Config file path is: {configFilePath}");
    }

    public void StartFile(params string[] args)
    {
        if(startScriptPath != null)
        {
            if(!File.Exists(startScriptPath))
            {
                Console.WriteLine("The file is not exist");
                return;
            }

            CLIOperator dropCLIOperator = new();
            Console.WriteLine("\nThe following lines contain commands stored in the startup script: ");


            foreach(string line in File.ReadLines(startScriptPath))
            {
                Console.WriteLine($"{CLIOperator.GetPrompt()}" + line);
                dropCLIOperator.DoCommand(line);
                Console.WriteLine(".");
            }
        }
    }
}
