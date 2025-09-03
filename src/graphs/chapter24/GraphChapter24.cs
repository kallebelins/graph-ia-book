namespace GraphIABook.Graphs.Chapter24;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 24 — SKG graph showcasing a recommended topology: preprocess -> five
/// independent branches in parallel -> deterministic aggregator merge.
/// Validates acyclicity and configures parallel execution.
/// </summary>
public static class GraphChapter24
{
    public static readonly int PreprocessMs = 5;
    public static readonly int[] BranchDurationsMs = new[] { 12, 10, 9, 8, 7 };
    public static readonly int AggregatorMs = 6;

    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateExecutor();
        GraphValidationUtils.EnsureAcyclic(executor);

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Creates the graph executor: preprocess -> (b1..b5 in parallel) -> aggregate.
    /// </summary>
    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["norm"] = text.Trim().ToLowerInvariant();
            await Task.Delay(PreprocessMs);
            return "preprocessed";
        }, "Preprocess"), nodeId: "preprocess");

        var b1 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["b1"] = string.Join('-', norm.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));
            await Task.Delay(BranchDurationsMs[0]);
            return "B1";
        }, "Branch1"), nodeId: "b1");

        var b2 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["b2"] = new string(norm.Reverse().ToArray());
            await Task.Delay(BranchDurationsMs[1]);
            return "B2";
        }, "Branch2"), nodeId: "b2");

        var b3 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["b3"] = norm.Length.ToString();
            await Task.Delay(BranchDurationsMs[2]);
            return "B3";
        }, "Branch3"), nodeId: "b3");

        var b4 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["b4"] = string.Concat(norm.OrderBy(c => c));
            await Task.Delay(BranchDurationsMs[3]);
            return "B4";
        }, "Branch4"), nodeId: "b4");

        var b5 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["b5"] = new string(norm.Distinct().ToArray());
            await Task.Delay(BranchDurationsMs[4]);
            return "B5";
        }, "Branch5"), nodeId: "b5");

        var aggregate = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var s1 = a.TryGetValue("b1", out var v1) ? v1?.ToString() ?? string.Empty : string.Empty;
            var s2 = a.TryGetValue("b2", out var v2) ? v2?.ToString() ?? string.Empty : string.Empty;
            var s3 = a.TryGetValue("b3", out var v3) ? v3?.ToString() ?? string.Empty : string.Empty;
            var s4 = a.TryGetValue("b4", out var v4) ? v4?.ToString() ?? string.Empty : string.Empty;
            var s5 = a.TryGetValue("b5", out var v5) ? v5?.ToString() ?? string.Empty : string.Empty;
            var answer = $"Answer: graph({s1}|{s2}|{s3}|{s4}|{s5})";
            await Task.Delay(AggregatorMs);
            return answer;
        }, "Aggregate"), nodeId: "merge");

        // Wire graph
        var executor = new GraphExecutor("ch24_fanout", "Parallel fan-out with aggregator (Chapter 24)");
        executor.AddNode(start)
            .AddNode(preprocess)
            .AddNode(b1)
            .AddNode(b2)
            .AddNode(b3)
            .AddNode(b4)
            .AddNode(b5)
            .AddNode(aggregate);
        executor.SetStartNode("start");

        executor.Connect("start", "preprocess");
        executor.Connect("preprocess", "b1");
        executor.Connect("preprocess", "b2");
        executor.Connect("preprocess", "b3");
        executor.Connect("preprocess", "b4");
        executor.Connect("preprocess", "b5");
        executor.Connect("b1", "merge");
        executor.Connect("b2", "merge");
        executor.Connect("b3", "merge");
        executor.Connect("b4", "merge");
        executor.Connect("b5", "merge");

        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = 5
        });

        return executor;
    }
}


