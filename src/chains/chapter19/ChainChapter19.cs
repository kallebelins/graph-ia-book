namespace GraphIABook.Chains.Chapter19;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 19 — SK chain demonstrating path-language acceptance over a labeled DAG (NFA semantics).
/// - Build a finite DAG with labeled edges (alphabet Σ = {a,b,c}).
/// - Check acceptance of the input string using NFA simulation (subset of states).
/// - Compute size of the determinized DFA (number of reachable subsets via subset construction).
/// - Merge results into a single output string.
/// Mirrors docs/book/26-capitulo-19.md (regular languages induced by DAGs).
/// </summary>
public static class ChainChapter19
{
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var build = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var g = BuildLabeledDag();
                a["G"] = g;
                a["Sigma"] = new[] { 'a', 'b', 'c' };
                a["Start"] = 0; // q0
                a["Finals"] = new HashSet<int> { 3 }; // q3
                return Task.FromResult("built-G");
            },
            "BuildDag");

        var accept = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var g = (Dictionary<int, List<(char label, int to)>>)a["G"]!;
                var start = (int)a["Start"]!;
                var finals = (HashSet<int>)a["Finals"]!;
                var s = a.TryGetValue("input", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                bool accepted = Accepts(g, start, finals, s);
                a["accepted"] = accepted;
                return Task.FromResult("accepted");
            },
            "Accept");

        var dfaSize = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var g = (Dictionary<int, List<(char label, int to)>>)a["G"]!;
                var sigma = (char[])a["Sigma"]!;
                var start = (int)a["Start"]!;
                int size = DeterminizedSize(g, sigma, start);
                a["dfaSize"] = size;
                return Task.FromResult("dfa-size");
            },
            "DfaSize");

        var merge = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                bool ok = a.TryGetValue("accepted", out var av) && av is bool b && b;
                int n = a.TryGetValue("dfaSize", out var nv) && nv is int i ? i : -1;
                var summary = $"answer(chain): accepted={ok}; dfaStates={n}";
                return Task.FromResult(summary);
            },
            "Merge");

        _ = await kernel.InvokeAsync(build, args);
        _ = await kernel.InvokeAsync(accept, args);
        _ = await kernel.InvokeAsync(dfaSize, args);
        var merged = await kernel.InvokeAsync(merge, args);
        return merged.GetValue<string>() ?? string.Empty;
    }

    private static Dictionary<int, List<(char label, int to)>> BuildLabeledDag()
    {
        // States: 0=q0 (start), 1=q1, 2=q2, 3=q3 (final)
        // Edges: q0 -a-> q1; q0 -b-> q2; q1 -c-> q3; q2 -c-> q3
        var g = new Dictionary<int, List<(char, int)>>
        {
            [0] = new List<(char, int)> { ('a', 1), ('b', 2) },
            [1] = new List<(char, int)> { ('c', 3) },
            [2] = new List<(char, int)> { ('c', 3) },
            [3] = new List<(char, int)>()
        };
        return g;
    }

    private static bool Accepts(Dictionary<int, List<(char label, int to)>> g, int start, HashSet<int> finals, string input)
    {
        var current = new HashSet<int> { start };
        foreach (var ch in input)
        {
            var next = new HashSet<int>();
            foreach (var q in current)
            {
                if (!g.TryGetValue(q, out var edges)) continue;
                foreach (var (label, to) in edges)
                {
                    if (label == ch) next.Add(to);
                }
            }
            current = next;
            if (current.Count == 0) break;
        }
        return current.Overlaps(finals);
    }

    private static int DeterminizedSize(Dictionary<int, List<(char label, int to)>> g, IReadOnlyList<char> sigma, int start)
    {
        // Subset construction: count reachable subsets from {start}
        var startSet = new HashSet<int> { start };
        var comparer = HashSetComparer<int>.Instance;
        var visited = new HashSet<HashSet<int>>(comparer);
        var queue = new Queue<HashSet<int>>();
        visited.Add(startSet);
        queue.Enqueue(startSet);

        while (queue.Count > 0)
        {
            var S = queue.Dequeue();
            foreach (var a in sigma)
            {
                var T = new HashSet<int>();
                foreach (var q in S)
                {
                    if (!g.TryGetValue(q, out var edges)) continue;
                    foreach (var (label, to) in edges)
                    {
                        if (label == a) T.Add(to);
                    }
                }
                if (T.Count == 0) continue;
                if (!visited.Contains(T))
                {
                    visited.Add(T);
                    queue.Enqueue(T);
                }
            }
        }

        return visited.Count;
    }

    private sealed class HashSetComparer<T> : IEqualityComparer<HashSet<T>> where T : notnull
    {
        public static readonly HashSetComparer<T> Instance = new();
        public bool Equals(HashSet<T>? x, HashSet<T>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.SetEquals(y);
        }
        public int GetHashCode(HashSet<T> obj)
        {
            unchecked
            {
                int h = 19;
                foreach (var v in obj.OrderBy(e => e)) h = h * 31 + v!.GetHashCode();
                return h;
            }
        }
    }
}


