namespace practic2.models;

public static class AptDependencyFetcher
{
    public static List<string>? GetDirectDependencies(string repository, string suite, string component, string arch, string name, string version)
    {
        string indexUrl = $"{repository.TrimEnd('/')}/dists/{suite}/{component}/binary-{arch}/Packages.gz";

        using var http = new HttpClient();
        using var stream = http.GetStreamAsync(indexUrl).GetAwaiter().GetResult();

        AptPackage? package = FindPackage(stream, name, version);
        if (package == null)
            return null;

        return ParseDependencyNames(package.Depends);
    }

    public static List<AptPackage> LoadIndex(string repository, string suite, string component, string arch)
    {
        string indexUrl = $"{repository.TrimEnd('/')}/dists/{suite}/{component}/binary-{arch}/Packages.gz";

        using var http = new HttpClient();
        using var stream = http.GetStreamAsync(indexUrl).GetAwaiter().GetResult();
        using var gz = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress);
        using var reader = new StreamReader(gz);

        var packages = new List<AptPackage>();
        string? packageName = null, packageVersion = null, depends = null;
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line == "")
            {
                if (packageName != null && packageVersion != null)
                    packages.Add(new AptPackage(packageName, packageVersion, depends ?? ""));
                packageName = packageVersion = depends = null;
            }
            else if (line.StartsWith("Package: ")) packageName = line[9..];
            else if (line.StartsWith("Version: ")) packageVersion = line[9..];
            else if (line.StartsWith("Depends: ")) depends = line[9..];
        }

        if (packageName != null && packageVersion != null)
            packages.Add(new AptPackage(packageName, packageVersion, depends ?? ""));

        return packages;
    }

    public static Func<string, List<string>?>? CreateDependencyResolver(List<AptPackage> index, string rootName, string rootVersion)
    {
        var root = index.FirstOrDefault(p => p.Name == rootName && p.Version == rootVersion);
        if (root == null)
            return null;

        var firstByName = new Dictionary<string, AptPackage>();
        foreach (var package in index)
            firstByName.TryAdd(package.Name, package);
        firstByName[rootName] = root;

        return name => firstByName.TryGetValue(name, out var package)
            ? ParseDependencyNames(package.Depends)
            : null;
    }

    private static AptPackage? FindPackage(Stream compressed, string name, string version)
    {
        using var gz = new System.IO.Compression.GZipStream(compressed, System.IO.Compression.CompressionMode.Decompress);
        using var reader = new StreamReader(gz);

        string? packageName = null, packageVersion = null, depends = null;
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line == "")
            {
                if (packageName == name && packageVersion == version)
                    return new AptPackage(packageName, packageVersion, depends ?? "");

                packageName = packageVersion = depends = null;
            }
            else if (line.StartsWith("Package: ")) packageName = line[9..];
            else if (line.StartsWith("Version: ")) packageVersion = line[9..];
            else if (line.StartsWith("Depends: ")) depends = line[9..];
        }

        return null;
    }

    private static List<string> ParseDependencyNames(string depends)
    {
        return depends
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('|')[0].Trim().Split(' ')[0])
            .ToList();
    }
}
