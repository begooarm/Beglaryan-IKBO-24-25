namespace Practic2.Models;

public class TomlReader
{
    public static Config ReadToml(string path)
    {
        Dictionary<string, Object> configData = [];

        foreach(string line in File.ReadLines(path))
        {
            if(line == "") { continue; }

            string[] tokens = line.Split(" ");
            char firstChar = tokens[0][0];

            if((firstChar != '#') && (firstChar != '['))
                configData[tokens[0]] = ParseValue(tokens[2])!;
        }

        return new Config(
            GetValue<string>(configData, "package_name"),
            GetValue<string>(configData, "repository"),
            GetValue<string>(configData, "test_mode"),
            GetValue<string>(configData, "package_version"),
            GetValue<string>(configData, "output_file"),
            GetValue<bool>(configData, "ascii_tree"),
            GetValue<int>(configData, "max_depth")
        );
    }

    private static T GetValue<T>(Dictionary<string, Object> data, string key)
    {
        if(!data.TryGetValue(key, out Object? value))
            throw new KeyNotFoundException($"Параметр '{key}' не найден в конфиге.");

        if(value is not T typed)
            throw new InvalidCastException(
                $"Параметр '{key}': ожидался тип {typeof(T).Name}");

        return typed;
    }

    private static Object ParseValue(string s)
    {
        if(s == "true") { return true; }
        if(s == "false") { return false; }

        if(s.StartsWith('"') && s.EndsWith('"')) { return s.Trim('"'); }
        if(int.TryParse(s, out int number)) { return number; }

        return s;
    }
}
