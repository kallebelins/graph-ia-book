namespace GraphIABook.Graphs.Chapter10;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 10 — SKG graph implementing a diamond pattern with parallel branches.
/// Structure: Start -> (A, B in parallel) -> Merge -> End.
/// Demonstrates that makespan ≈ max(tA, tB) + tMerge, contrasting with chain sum.
/// </summary>
public static class GraphChapter10
{
	public static readonly int BranchADurationMs = 90;
	public static readonly int BranchBDurationMs = 60;
	public static readonly int MergeDurationMs = 40;

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
/// Builds the diamond graph executor: start -> (A,B) -> merge.
/// </summary>
	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

		var branchA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(BranchADurationMs);
			var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["a"] = $"A:{raw.ToUpperInvariant()}";
			return "A-ok";
		}, "BranchA"), nodeId: "A");

		var branchB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(BranchBDurationMs);
			var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["b"] = $"B:{new string(raw.Reverse().ToArray())}";
			return "B-ok";
		}, "BranchB"), nodeId: "B");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(MergeDurationMs);
			var av = a.TryGetValue("a", out var va) ? va?.ToString() ?? string.Empty : string.Empty;
			var bv = a.TryGetValue("b", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
			return $"answer(graph): merge({av}|{bv})";
		}, "Merge"), nodeId: "merge");

		var executor = new GraphExecutor("ch10_diamond", "Diamond graph with parallel branches");
		executor.AddNode(start)
			.AddNode(branchA)
			.AddNode(branchB)
			.AddNode(merge);

		executor.SetStartNode("start");
		executor.Connect("start", "A");
		executor.Connect("start", "B");
		executor.Connect("A", "merge");
		executor.Connect("B", "merge");

		executor.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = 2
		});

		return executor;
	}
}



