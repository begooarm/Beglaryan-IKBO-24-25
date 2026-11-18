namespace practic2.models;

public class TomlReader
{
    public static Config ReadToml(string path)
    {
        Dictionary<string, Object> configData = [];

        foreach(string line in File.ReadLines(path))
        {
            if(line == "") { continue; }

            string[] splited_line = line.Split(" ");
            char firstChar = splited_line[0][0];

            if((firstChar != '#') && (firstChar != '['))
                configData[splited_line[0]] = stringToType(splited_line[2])!;
        }

        return new Config(
            getValue<string>(configData, "package_name"),
            getValue<string>(configData, "repository"),
            getValue<string>(configData, "test_mode"),
            getValue<string>(configData, "package_version"),
            getValue<string>(configData, "output_file"),
            getValue<bool>(configData, "ascii_tree"),
            getValue<int>(configData, "max_depth")
        );
    }

    private static T getValue<T>(Dictionary<string, Object> data, string key)
    {
        if(!data.TryGetValue(key, out Object? value))
            throw new KeyNotFoundException($"Параметр '{key}' не найден в конфиге.");

        if(value is not T typed)
            throw new InvalidCastException(
                $"Параметр '{key}': ожидался тип {typeof(T).Name}");

        return typed;
    }

    private static Object stringToType(string s)
    {
        if(s == "true") { return true; }
        if(s == "false") { return false; }

        if(s.StartsWith('"') && s.EndsWith('"')) { return s.Trim('"'); }
        if(int.TryParse(s, out int number)) { return number; }

        return s;
    }
}
