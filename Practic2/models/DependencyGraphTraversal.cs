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

    public List<string> GetLoadOrder(string root, Func<string, List<string>?> getDependencies, int maxDepth, List<string> cycles)
    {
        var order = new List<string>();
        var visited = new HashSet<string>();
        var onPath = new List<string>();

        void Dfs(string node, int depth)
        {
            visited.Add(node);
            onPath.Add(node);

            if (depth < maxDepth)
            {
                foreach (var dep in getDependencies(node) ?? [])
                {
                    int index = onPath.IndexOf(dep);
                    if (index >= 0)
                    {
                        cycles.Add(string.Join(" -> ", onPath[index..].Append(dep)));
                        continue;
                    }

                    if (!visited.Contains(dep))
                        Dfs(dep, depth + 1);
                }
            }

            onPath.RemoveAt(onPath.Count - 1);
            order.Add(node);
        }

        Dfs(root, 0);
        return order;
    }
}
