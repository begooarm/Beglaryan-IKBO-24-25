namespace practic2.models;

public static class GraphVisualizer
{
    public static string ToDot(string root, List<(string From, string To, int Depth)> edges)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("digraph deps {");
        sb.AppendLine($"    \"{root}\";");
        foreach (var (from, to, _) in edges)
            sb.AppendLine($"    \"{from}\" -> \"{to}\";");
        sb.Append('}');
        return sb.ToString();
    }

    public static void SaveSvg(string dot, string outputFile)
    {
        string dotFile = Path.ChangeExtension(outputFile, ".dot");
        File.WriteAllText(dotFile, dot);
        Console.WriteLine($"\nОписание графа сохранено в {dotFile}");

        try
        {
            var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "dot",
                ArgumentList = { "-Tsvg", dotFile, "-o", outputFile },
                RedirectStandardError = true,
                UseShellExecute = false
            })!;
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine($"Ошибка Graphviz: {error}");
                return;
            }
            Console.WriteLine($"Изображение графа сохранено в {outputFile}");
        }
        catch (System.ComponentModel.Win32Exception)
        {
            Console.Error.WriteLine("Не найдена программа dot: установите Graphviz и добавьте его в PATH");
        }
    }

    public static void PrintAsciiTree(string node, Dictionary<string, List<string>> graph,
        string indent, int depth, int maxDepth, HashSet<string> onPath)
    {
        if (depth >= maxDepth || !graph.TryGetValue(node, out var deps))
            return;

        foreach (string dep in deps)
        {
            Console.WriteLine($"{indent}|---- {dep}");
            if (onPath.Add(dep))
            {
                PrintAsciiTree(dep, graph, indent + "|     ", depth + 1, maxDepth, onPath);
                onPath.Remove(dep);
            }
        }
    }
}
