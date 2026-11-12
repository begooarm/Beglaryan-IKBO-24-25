using practic1.models.VFS;
using System.Diagnostics;
using System.Text;

namespace practic1.models.CLIManagement;
using System.Collections.Generic;

public class SystemController {
    private readonly VFSManager vfsManager;
    private readonly Stopwatch runningTime = Stopwatch.StartNew();

    public string? vfsPath;
    public string? startScriptPath;
    public string? configFilePath;

    public Dictionary<string, string> config = [];

    public Dictionary<string, List<Action<string[]>>> commands;

    public SystemController(VFSManager vfsManager)
    {
        this.vfsManager = vfsManager;

        commands = new()
        {
            ["ls"] = [ls],
            ["cd"] = [cd],
            ["wc"] = [wc],
            ["rm"] = [rm],
            ["uptime"] = [uptime],
            ["exit"] = [exit],
            ["setup"] = [setup],
            ["conf-dump"] = [ConfDump],
        };
    }

    public static string GetUsername() { return Environment.UserName; }
    public static string GetHostname() { return Environment.MachineName; }

    public void ls(params string[] args)
    {
        if (args.Length == 0)
            vfsManager.DisplayDirectory(".");
        else
            vfsManager.DisplayDirectory(args[0]);
    }

    public void cd(params string[] args)
    {
        if (args.Length == 0)
            vfsManager.currentFolder = vfsManager.GetFolder("/");
        else
            vfsManager.currentFolder = vfsManager.GetFolder(args[0]);
    }

    public void wc(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("wc: specify a file path");
            return;
        }

        foreach (string path in args)
        {
            VFSElement element;

            try
            {
                element = vfsManager.GetElement(path);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"wc: {path}: no such file or directory");
                continue;
            }
            catch (InvalidCastException)
            {
                Console.WriteLine($"wc: {path}: not a directory");
                continue;
            }

            if (element is not MyFile file)
            {
                Console.WriteLine($"wc: {path}: is a directory");
                continue;
            }

            string text = Encoding.UTF8.GetString(file.content);

            int lines = 0;
            int words = 0;

            bool inWord = false;

            foreach (char symbol in text)
            {
                if (symbol == '\n')
                    lines++;

                if (char.IsWhiteSpace(symbol))
                    inWord = false;

                else
                {
                    if (!inWord)
                        words++;

                    inWord = true;
                }
            }

            Console.WriteLine($"{lines} {words} {file.content.Length} {path}");
        }
    }

    public void rm(params string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("rm: specify exactly one file path");
            return;
        }

        string path = args[0];
        VFSElement element;
        try
        {
            element = vfsManager.GetElement(path);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"rm: {path}: no such file or directory");
            return;
        }
        catch (InvalidCastException)
        {
            Console.WriteLine($"rm: {path}: not a directory");
            return;
        }

        if (element is Folder)
        {
            Console.WriteLine($"rm: {path}: is a directory");
            return;
        }

        vfsManager.allElements.Remove(element);
    }

    public void uptime(params string[] args)
    {
        if (args.Length != 0)
        {
            Console.WriteLine("uptime: arguments are not supported");
            return;
        }

        TimeSpan elapsed = runningTime.Elapsed;
        Console.WriteLine($"up {elapsed.Days} days, {elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}");
    }

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

        if(vfsPath != null)
            vfsManager.Load(vfsPath);

        Console.WriteLine("All paths were successfully saved");

        Console.WriteLine($"VFS path is: {vfsPath}");
        Console.WriteLine($"Start sctipt path is: {startScriptPath}");
        Console.WriteLine($"Config file path is: {configFilePath}");
    }

}
