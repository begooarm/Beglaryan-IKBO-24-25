using System.Text.RegularExpressions;

namespace practic1.models;

public class CLIOperator {
    public SystemController systemController = new();

    public static string GetPrompt() { return $"{SystemController.GetUsername()}@{SystemController.GetHostname()}:~$ "; }

    private static string[] ReadInputLine(string inputLine)
    {
        string[] splitedInput = inputLine.Split(' ');

        for (int i = 0; i < splitedInput.Length; i++)
        {
            splitedInput[i] = Regex.Replace(splitedInput[i], @"\$(?:\{([a-zA-Z_][a-zA-Z0-9_]*)\}|([a-zA-Z_][a-zA-Z0-9_]*))",
                match => Environment.GetEnvironmentVariable(
                    match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value) ?? "");
        }

        return splitedInput;
    }

    public void DoCommand(string? inputLine)
    {
        if (inputLine == null) { Console.WriteLine("Empty command"); return; }

        string[] splitedInput = ReadInputLine(inputLine);

        if (systemController.commands.TryGetValue(splitedInput[0], out List<Action<string[]>>? functions))
        {
            foreach (Action<string[]> function in functions)
                function(splitedInput[1..]);
            return;
        }

        Console.WriteLine("Unknown command");
    }
}
