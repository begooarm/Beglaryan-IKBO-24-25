namespace Practic2.Models;

public class AptPackage
{
    public string Name;
    public string Version;
    public string Depends;

    public AptPackage(string name, string version, string depends)
    {
        Name = name;
        Version = version;
        Depends = depends;
    }
}
