static class Validator
{
    public static void Reassignment(ref Object legacyValue, Object maybeNewValue)
    {
        if(maybeNewValue != null) { legacyValue = maybeNewValue; }
    }

    public static void CheckIniConfig(string path, ref string? vfs, ref string? startScript)
    {
        Dictionary<string, string>? iniData = IniReader.Read(path);
        if(iniData == null) { return; }

        if(iniData.TryGetValue("vfsPath", out string? dropString)) { vfs = dropString; }
        if(iniData.TryGetValue("startScriptPath", out dropString)) { startScript = dropString; }
    }
}