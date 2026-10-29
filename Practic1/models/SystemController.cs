namespace practic1.models;

public class SystemController {
    private readonly string username = Environment.UserName;
    private readonly string hostName = Environment.MachineName;

    public Dictionary<string, Func<string>> commands;

    public SystemController()
    {
        commands = new()
        {
            ["ls"] = ls,
            ["cd"] = cd,
            ["exit"] = exit
        };
    }

    public string GetUsername() { return username; }
    public string GetHostname() { return hostName; }

    public string ls() => "ls command";
    public string cd() => "cd command";
    
    public string exit()
    {
        Environment.Exit(1);
        return "";
    }
}