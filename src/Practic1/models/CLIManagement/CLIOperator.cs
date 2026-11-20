using System.Text.RegularExpressions;
using Practic1.Models.VFS;

namespace Practic1.Models.CLIManagement;

public class CLIOperator {
    public SystemController systemController;
    public VFSManager vfsManager = new();

    public CLIOperator()
    {
        systemController = new SystemController(vfsManager);
        systemController.commands["setup"].Add(StartFile);
    }

    public static string GetPrompt() =>
        $"{SystemController.GetUsername()}@{SystemController.GetHostname()}:~$ ";

    private static string[] ReadInputLine(string inputLine)
    {
        string[] tokens = inputLine.Split(' ');

        for (int i = 0; i < tokens.Length; i++)
        {
            tokens[i] = Regex.Replace(tokens[i],
                @"\$(?:\{([a-zA-Z_][a-zA-Z0-9_]*)\}|([a-zA-Z_][a-zA-Z0-9_]*))",
                match => Environment.GetEnvironmentVariable(
                    match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value) ?? "");
        }

        return tokens;
    }

    public void StartFile(params string[] args)
    {
        string? startScriptPath = systemController.startScriptPath;
        if (startScriptPath == null) return;

        if (!File.Exists(startScriptPath))
        {
            Console.WriteLine("The file is not exist");
            return;
        }

        Console.WriteLine("\nThe following lines contain commands stored in the startup script: ");

        foreach (string line in File.ReadLines(startScriptPath))
        {
            Console.WriteLine($"{GetPrompt()}{line}");
            DoCommand(line);
            Console.WriteLine(".");
        }
    }

    public void DoCommand(string? inputLine)
    {
        if (inputLine == null) { Console.WriteLine("Empty command"); return; }

        string[] tokens = ReadInputLine(inputLine);

        if (systemController.commands.TryGetValue(tokens[0], out List<Action<string[]>>? functions))
        {
            foreach (Action<string[]> function in functions)
                function(tokens[1..]);
            return;
        }

        Console.WriteLine("Unknown command");
    }
}
