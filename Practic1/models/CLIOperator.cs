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