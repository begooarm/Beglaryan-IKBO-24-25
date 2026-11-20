namespace Practic2.Models;

public class DependencyGraphTraversal
{
    public List<(string From, string To, int Depth)> Traverse(
        string root, Func<string, List<string>?> getDependencies, int maxDepth)
    {
        var result = new List<(string From, string To, int Depth)>();
        var seenEdges = new HashSet<(string, string)>();
        var bestDepth = new Dictionary<string, int>();
        var cache = new Dictionary<string, List<string>>();

        void Dfs(string node, int depth)
        {
            if (bestDepth.TryGetValue(node, out int knownDepth) && knownDepth <= depth)
                return;

            bestDepth[node] = depth;
            if (depth >= maxDepth)
                return;

            if (!cache.TryGetValue(node, out var deps))
                cache[node] = deps = getDependencies(node) ?? [];

            foreach (string dep in deps)
            {
                if (seenEdges.Add((node, dep)))
                    result.Add((node, dep, depth + 1));
                Dfs(dep, depth + 1);
            }
        }

        Dfs(root, 0);
        return result;
    }

    public List<string> GetLoadOrder(
        string root, Func<string, List<string>?> getDependencies, int maxDepth, List<string> cycles)
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
