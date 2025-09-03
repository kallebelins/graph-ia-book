namespace GraphIABook.Graphs.Chapter15;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 15 — SKG graph implementing a 3-branch parallel aggregation (fan-out/fan-in).
/// Preprocess -> {A, B, C} in parallel -> Merge.
/// </summary>
public static class GraphChapter15
{
	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(5);
			a["pre"] = true;
			return "pre-ok";
		}, "Preprocess"), nodeId: "pre");

		var nodeA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var ms = a.TryGetValue("aMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
			await Task.Delay(ms);
			a["a"] = $"A({ms}ms)";
			return "A-ok";
		}, "BranchA"), nodeId: "A");

		var nodeB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var ms = a.TryGetValue("bMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
			await Task.Delay(ms);
			a["b"] = $"B({ms}ms)";
			return "B-ok";
		}, "BranchB"), nodeId: "B");

		var nodeC = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var ms = a.TryGetValue("cMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
			await Task.Delay(ms);
			a["c"] = $"C({ms}ms)";
			return "C-ok";
		}, "BranchC"), nodeId: "C");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(10);
			var av = a.TryGetValue("a", out var va) ? va?.ToString() ?? string.Empty : string.Empty;
			var bv = a.TryGetValue("b", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
			var cv = a.TryGetValue("c", out var vc) ? vc?.ToString() ?? string.Empty : string.Empty;
			return $"answer(graph): merge({av}|{bv}|{cv})";
		}, "Merge"), nodeId: "merge");

		start.ConnectTo(preprocess);
		preprocess.ConnectTo(nodeA);
		preprocess.ConnectTo(nodeB);
		preprocess.ConnectTo(nodeC);
		nodeA.ConnectTo(merge);
		nodeB.ConnectTo(merge);
		nodeC.ConnectTo(merge);

		var executor = new GraphExecutor("ch15_parallel_graph", "Three-branch parallel aggregation");
		executor.AddNode(start)
			.AddNode(preprocess)
			.AddNode(nodeA)
			.AddNode(nodeB)
			.AddNode(nodeC)
			.AddNode(merge);
		executor.SetStartNode("start");
		executor.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = 8
		});
		return executor;
	}

	public static async Task<string> RunAsync(int aDurationMs, int bDurationMs, int cDurationMs)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments
		{
			["aMs"] = aDurationMs,
			["bMs"] = bDurationMs,
			["cMs"] = cDurationMs
		};
		var executor = CreateExecutor();
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 15.");
		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}




