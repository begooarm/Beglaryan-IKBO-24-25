using System.IO;
using System.Collections.Generic;

static class IniReader
{
    public static Dictionary<string, string>? Read(string filePath)
    {
        if(!File.Exists(filePath)) { return null; }

        Dictionary<string, string> config = new();

        foreach(string line in File.ReadLines(filePath))
        {
            if(line[0] == '[' || line[0] == ';') { continue; }

            // line form in ini is "something = something_else"
            string[] lineSplit = line.Split(' ');
            config[lineSplit[0]] = lineSplit[2];
        }

        return config;
    }
}