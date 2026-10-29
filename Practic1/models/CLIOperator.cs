using System.Text.RegularExpressions;

namespace practic1.models;

public class CLIOperator {
    public string initialString;
    private SystemController systemController = new();

    public CLIOperator()
    {
        initialString = $"{systemController.GetUsername()}@{systemController.GetHostname()}:~$ ";
    }

    private string[] ReadInputLine(string inputLine)
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

    public string DoCommand(string inputLine)
    {
        string[] splitedInput = ReadInputLine(inputLine);
        
        if (systemController.commands.TryGetValue(splitedInput[0], out Func<string>? func))
        { return func(); }

        return "Unknow command";
    }
}