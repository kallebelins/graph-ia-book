namespace GraphIABook.Glossary;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Minimal, self-contained glossary examples demonstrating:
/// - DAG construction (acyclic graph)
/// - Topological order retrieval
/// - Betweenness centrality (unweighted, directed)
/// - Critical path computation (longest path on DAG with node durations)
/// </summary>
public static class GlossaryExamples
{
    /// <summary>
    /// Creates a small DAG used across the glossary examples.
    /// Topology (directed):
    ///   start -> a -> c -> end
    ///   start -> b -> c
    /// </summary>
    public static GraphExecutor CreateDagExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var a = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async () => { await Task.Delay(2); return "A"; }, "A"), nodeId: "a");
        var b = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async () => { await Task.Delay(2); return "B"; }, "B"), nodeId: "b");
        var c = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async () => { await Task.Delay(3); return "C"; }, "C"), nodeId: "c");
        var end = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "end", "End"), nodeId: "end");

        start.ConnectTo(a);
        start.ConnectTo(b);
        a.ConnectTo(c);
        b.ConnectTo(c);
        c.ConnectTo(end);

        var executor = new GraphExecutor("glossary_dag", "Minimal DAG for glossary examples");
        executor.AddNode(start).AddNode(a).AddNode(b).AddNode(c).AddNode(end);
        executor.SetStartNode("start");
        executor.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 4 });
        GraphIABook.Benchmark._common.GraphValidationUtils.EnsureAcyclic(executor);
        return executor;
    }

    /// <summary>
    /// Returns the topological order of the current DAG.
    /// </summary>
    public static IReadOnlyList<string> GetTopologicalOrder(GraphExecutor executor)
    {
        var plan = GraphPlanCompiler.Compile(executor);
        if (plan.HasCycles || plan.TopologicalOrder is null)
        {
            throw new InvalidOperationException("Graph must be acyclic to have a topological order.");
        }
        return plan.TopologicalOrder;
    }

    /// <summary>
    /// Computes simple betweenness centrality for a directed, unweighted graph using Brandes' algorithm.
    /// Returns a map nodeId -> centrality score (normalized to [0, 1] for small graphs).
    /// </summary>
    public static IReadOnlyDictionary<string, double> ComputeBetweennessCentrality(GraphExecutor executor)
    {
        var plan = GraphPlanCompiler.Compile(executor);
        var nodes = plan.NodeIds;
        var adjacency = plan.Adjacency;

        var centrality = nodes.ToDictionary(n => n, _ => 0.0, StringComparer.Ordinal);

        foreach (var s in nodes)
        {
            var stack = new Stack<string>();
            var predecessors = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            var sigma = new Dictionary<string, double>(StringComparer.Ordinal); // number of shortest paths
            var dist = new Dictionary<string, int>(StringComparer.Ordinal);

            foreach (var v in nodes)
            {
                predecessors[v] = new List<string>();
                sigma[v] = 0.0;
                dist[v] = -1; // -1 denotes infinite distance
            }

            sigma[s] = 1.0;
            dist[s] = 0;

            var queue = new Queue<string>();
            queue.Enqueue(s);
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                stack.Push(v);
                if (!adjacency.TryGetValue(v, out var outs)) continue;
                foreach (var w in outs)
                {
                    if (dist[w] < 0)
                    {
                        queue.Enqueue(w);
                        dist[w] = dist[v] + 1;
                    }
                    if (dist[w] == dist[v] + 1)
                    {
                        sigma[w] += sigma[v];
                        predecessors[w].Add(v);
                    }
                }
            }

            var dependency = nodes.ToDictionary(v => v, _ => 0.0, StringComparer.Ordinal);
            while (stack.Count > 0)
            {
                var w = stack.Pop();
                foreach (var v in predecessors[w])
                {
                    if (sigma[w] > 0)
                    {
                        dependency[v] += (sigma[v] / sigma[w]) * (1.0 + dependency[w]);
                    }
                }
                if (!string.Equals(w, s, StringComparison.Ordinal))
                {
                    centrality[w] += dependency[w];
                }
            }
        }

        // Optional normalization for directed graphs: divide by (n-1)(n-2)
        var n = nodes.Count;
        var norm = (n > 2) ? (1.0 / ((n - 1.0) * (n - 2.0))) : 1.0;
        var normalized = centrality.ToDictionary(kv => kv.Key, kv => kv.Value * norm, StringComparer.Ordinal);
        return normalized;
    }

    /// <summary>
    /// Computes the critical path (longest path) length and path nodes on a DAG given node durations.
    /// Durations are per-node; the total duration is the sum along the path.
    /// </summary>
    public static (int TotalDuration, IReadOnlyList<string> Path) ComputeCriticalPath(GraphExecutor executor, IReadOnlyDictionary<string, int> nodeDurations)
    {
        var plan = GraphPlanCompiler.Compile(executor);
        if (plan.HasCycles || plan.TopologicalOrder is null)
        {
            throw new InvalidOperationException("Critical path requires a DAG with a topological order.")
            ;
        }

        // Build reverse adjacency (predecessors)
        var predecessors = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var id in plan.NodeIds)
        {
            predecessors[id] = new List<string>();
        }
        foreach (var (src, outs) in plan.Adjacency)
        {
            foreach (var t in outs)
            {
                predecessors[t].Add(src);
            }
        }

        // DP over topological order to compute longest time finishing at each node
        var finishTime = new Dictionary<string, int>(StringComparer.Ordinal);
        var parentOnCritical = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var v in plan.TopologicalOrder)
        {
            var duration = nodeDurations.TryGetValue(v, out var d) ? d : 0;
            var bestPred = (string?)null;
            var bestTime = 0;
            if (predecessors[v].Count > 0)
            {
                foreach (var p in predecessors[v])
                {
                    var cand = finishTime.TryGetValue(p, out var ft) ? ft : 0;
                    if (cand > bestTime)
                    {
                        bestTime = cand;
                        bestPred = p;
                    }
                }
            }
            finishTime[v] = bestTime + duration;
            parentOnCritical[v] = bestPred;
        }

        // Identify sink with maximum finish time
        var endNode = plan.NodeIds[0];
        var maxTime = int.MinValue;
        foreach (var (node, time) in finishTime)
        {
            if (time > maxTime)
            {
                maxTime = time;
                endNode = node;
            }
        }

        // Reconstruct path backwards
        var path = new List<string>();
        var cur = endNode;
        while (cur != null)
        {
            path.Add(cur);
            cur = parentOnCritical[cur];
        }
        path.Reverse();
        return (maxTime, path);
    }

    /// <summary>
    /// Convenience demo that builds the DAG, computes topology, centrality and critical path.
    /// </summary>
    public static async Task<GlossaryDemoResult> RunDagDemoAsync()
    {
        var kernel = Kernel.CreateBuilder().Build();
        var executor = CreateDagExecutor();
        var order = GetTopologicalOrder(executor);
        var centrality = ComputeBetweennessCentrality(executor);

        var durations = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["start"] = 1,
            ["a"] = 2,
            ["b"] = 2,
            ["c"] = 3,
            ["end"] = 1
        };
        var (total, path) = ComputeCriticalPath(executor, durations);

        // Execute once to validate the graph runs
        var args = new KernelArguments();
        _ = await executor.ExecuteAsync(kernel, args);

        return new GlossaryDemoResult(order, centrality, total, path);
    }
}

/// <summary>
/// Aggregated demo result for the glossary DAG.
/// </summary>
public sealed record GlossaryDemoResult(
    IReadOnlyList<string> TopologicalOrder,
    IReadOnlyDictionary<string, double> BetweennessCentrality,
    int CriticalPathDuration,
    IReadOnlyList<string> CriticalPathNodes
);


