namespace GraphIABook.Graphs.Chapter19;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 19 — SKG graph for path-language acceptance (regular languages induced by labeled DAGs).
/// Parallel branches compute acceptance (NFA) and determinized DFA size, then merge results.
/// Mirrors docs/book/26-capitulo-19.md.
/// </summary>
public static class GraphChapter19
{
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };
        var executor = CreateExecutor();
        GraphValidationUtils.EnsureAcyclic(executor);
        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var build = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var g = BuildLabeledDag();
            a["G"] = g;
            a["Sigma"] = new[] { 'a', 'b', 'c' };
            a["Start"] = 0;
            a["Finals"] = new HashSet<int> { 3 };
            return Task.FromResult("built-G");
        }, "BuildDag"), nodeId: "build");

        var accept = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var g = (Dictionary<int, List<(char label, int to)>>)a["G"]!;
            var startState = (int)a["Start"]!;
            var finals = (HashSet<int>)a["Finals"]!;
            var s = a.TryGetValue("input", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            bool ok = Accepts(g, startState, finals, s);
            a["accepted"] = ok;
            return Task.FromResult("accepted");
        }, "Accept"), nodeId: "accept");

        var dfa = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var g = (Dictionary<int, List<(char label, int to)>>)a["G"]!;
            var sigma = (char[])a["Sigma"]!;
            var startState = (int)a["Start"]!;
            int size = DeterminizedSize(g, sigma, startState);
            a["dfaSize"] = size;
            return Task.FromResult("dfa-size");
        }, "DfaSize"), nodeId: "dfa");

        var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            bool ok = a.TryGetValue("accepted", out var av) && av is bool b && b;
            int n = a.TryGetValue("dfaSize", out var nv) && nv is int i ? i : -1;
            var summary = $"answer(graph): accepted={ok}; dfaStates={n}";
            return Task.FromResult(summary);
        }, "Merge"), nodeId: "merge");

        var executor = new GraphExecutor("ch19_automata_paths", "Path-language acceptance over labeled DAG (parallel compute)");
        executor.AddNode(start)
            .AddNode(build)
            .AddNode(accept)
            .AddNode(dfa)
            .AddNode(merge);

        executor.SetStartNode("start");
        executor.Connect("start", "build");
        executor.Connect("build", "accept");
        executor.Connect("build", "dfa");
        executor.Connect("accept", "merge");
        executor.Connect("dfa", "merge");

        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = 2
        });

        return executor;
    }

    private static Dictionary<int, List<(char label, int to)>> BuildLabeledDag()
    {
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


