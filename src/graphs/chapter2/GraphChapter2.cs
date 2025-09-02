namespace GraphIABook.Graphs.Chapter2;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 2 — SKG graph focusing on expressivity: alternative and parallel branches
/// with a deterministic merge. Demonstrates that multiple paths exist and can run
/// concurrently when independent, contrasting the linear chain.
/// </summary>
public static class GraphChapter2
{
    public static readonly int NormalizeMs = 4;
    public static readonly int BranchAMs = 5;
    public static readonly int BranchBMs = 6;
    public static readonly int MergeMs = 4;

    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateExecutor();
        var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
        if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 2 experiment.");

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Creates the graph executor with two parallel branches and a deterministic merge.
    /// </summary>
    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var normalize = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["norm"] = text.Trim().ToLowerInvariant();
            await Task.Delay(NormalizeMs);
            return "normalized";
        }, "Normalize"), nodeId: "normalize");

        var branchA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["a"] = string.Join('-', norm.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));
            await Task.Delay(BranchAMs);
            return "A";
        }, "BranchA"), nodeId: "a");

        var branchB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["b"] = new string(norm.Reverse().ToArray());
            await Task.Delay(BranchBMs);
            return "B";
        }, "BranchB"), nodeId: "b");

        var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var av = a.TryGetValue("a", out var va) ? va?.ToString() ?? string.Empty : string.Empty;
            var bv = a.TryGetValue("b", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
            var answer = $"Answer: graph({av}|{bv})";
            await Task.Delay(MergeMs);
            return answer;
        }, "Merge"), nodeId: "merge");

        start.ConnectTo(normalize);
        normalize.ConnectTo(branchA);
        normalize.ConnectTo(branchB);
        branchA.ConnectTo(merge);
        branchB.ConnectTo(merge);

        var executor = new GraphExecutor("ch2_parallel_branches", "Two parallel branches with deterministic merge");
        executor.AddNode(start)
            .AddNode(normalize)
            .AddNode(branchA)
            .AddNode(branchB)
            .AddNode(merge);
        executor.SetStartNode("start");
        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = 4
        });
        return executor;
    }
}


