using System.IO;
using System.Collections.Generic;

namespace Practic1.Models.CLIManagement;

static class IniReader
{
    /// <summary>Reads settings written as key = value from an INI file.</summary>
    public static Dictionary<string, string>? Read(string filePath)
    {
        if(!File.Exists(filePath)) { return null; }

        Dictionary<string, string> config = new();

        foreach(string line in File.ReadLines(filePath))
        {
            if(line[0] == '[' || line[0] == ';') { continue; }

            string[] lineSplit = line.Split(' ');
            config[lineSplit[0]] = lineSplit[2];
        }

        return config;
    }
}
