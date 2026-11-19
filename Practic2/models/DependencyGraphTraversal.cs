namespace practic2.models;

public class DependencyGraphTraversal
{
    public List<(string From, string To, int Depth)> Traverse(
        string root, Func<string, List<string>?> getDependencies, int maxDepth)
    {
        var result = new List<(string From, string To, int Depth)>();
        var seenEdges = new HashSet<(string, string)>();
        var bestDepth = new Dictionary<string, int>();
        var onPath = new HashSet<string>();
        var cache = new Dictionary<string, List<string>>();

        List<string> DepsOf(string name)
        {
            if (!cache.TryGetValue(name, out var deps))
                cache[name] = deps = getDependencies(name) ?? [];
            return deps;
        }

        void Dfs(string node, int depth)
        {
            bestDepth[node] = depth;
            onPath.Add(node);

            if (depth < maxDepth)
            {
                foreach (var dep in DepsOf(node))
                {
                    if (seenEdges.Add((node, dep)))
                        result.Add((node, dep, depth + 1));

                    if (onPath.Contains(dep))
                        continue;

                    if (bestDepth.TryGetValue(dep, out int known) && known <= depth + 1)
                        continue;

                    Dfs(dep, depth + 1);
                }
            }

            onPath.Remove(node);
        }

        Dfs(root, 0);
        return result;
    }
}
