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

        return package.Depends
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Split('|')[0].Trim().Split(' ')[0])
            .ToList();
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
}
