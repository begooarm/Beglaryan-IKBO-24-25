namespace practic1.models;

public class SystemController {
    private readonly string username = Environment.UserName;
    private readonly string hostName = Environment.MachineName;

    public string? vfsPath = "null";
    public string? startScriptPath = "null";

    public Dictionary<string, Func<string[], string>> commands;

    public SystemController()
    {
        commands = new()
        {
            ["ls"] = ls,
            ["cd"] = cd,
            ["exit"] = exit,
            ["setup"] = setUp
        };
    }

    public string GetUsername() { return username; }
    public string GetHostname() { return hostName; }

    public string ls(params string[] args) => "ls command";
    public string cd(params string[] args) => "cd command";

    public string exit(params string[] args)
    {
        Environment.Exit(1);
        return "";
    }

    public string setUp(params string[] args)
    {
        if(args.Length == 0) { return "";  }
        
        for(int i = 0; i < args.Length; i++)
        {
            if(args[i] == "--vfs") { vfsPath = args[i + 1]; }
            if(args[i] == "--startScript") { startScriptPath = args[i + 1]; }
        }
        
        return $"vfs path: {vfsPath}\nstart script path: {startScriptPath}";
    }
}