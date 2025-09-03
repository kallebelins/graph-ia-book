namespace GraphIABook.Graphs.Chapter17;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 17 — SKG graph modeling a layered DAG with explicit dependencies:
/// Normalize -> {A,B} in parallel -> C depends on A & B, D depends on B ->
/// E depends on {C,D} -> Merge. Demonstrates critical-path limited makespan.
/// </summary>
public static class GraphChapter17
{
	public static readonly int DurationA = 100; // ms
	public static readonly int DurationB = 130; // ms
	public static readonly int DurationC = 90;  // ms
	public static readonly int DurationD = 110; // ms
	public static readonly int DurationE = 70;  // ms
	public static readonly int DurationMerge = 50; // ms

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
		var normalize = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["norm"] = raw.Trim().ToLowerInvariant();
			return Task.FromResult("normalized");
		}, "Normalize"), nodeId: "normalize");

		var nodeA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(DurationA);
			a["A"] = $"A:{a["norm"]}";
			return "A";
		}, "A"), nodeId: "A");

		var nodeB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(DurationB);
			a["B"] = $"B:{a["norm"]}";
			return "B";
		}, "B"), nodeId: "B");

		var nodeC = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(DurationC);
			var s = (a.TryGetValue("A", out var va) ? va?.ToString() : string.Empty) + "|" + (a.TryGetValue("B", out var vb) ? vb?.ToString() : string.Empty);
			a["C"] = $"C:({s})";
			return "C";
		}, "C"), nodeId: "C");

		var nodeD = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(DurationD);
			var s = (a.TryGetValue("B", out var vb) ? vb?.ToString() : string.Empty);
			a["D"] = $"D:({s})";
			return "D";
		}, "D"), nodeId: "D");

		var nodeE = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(DurationE);
			var s = (a.TryGetValue("C", out var vc) ? vc?.ToString() : string.Empty) + "|" + (a.TryGetValue("D", out var vd) ? vd?.ToString() : string.Empty);
			a["E"] = $"E:({s})";
			return "E";
		}, "E"), nodeId: "E");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(DurationMerge);
			var payload = a.TryGetValue("E", out var ve) ? ve?.ToString() ?? string.Empty : string.Empty;
			return $"answer(graph): merge({payload})";
		}, "Merge"), nodeId: "merge");

		var executor = new GraphExecutor("ch17_layered_dag", "Layered DAG with critical-path limited makespan");
		executor.AddNode(start)
			.AddNode(normalize)
			.AddNode(nodeA)
			.AddNode(nodeB)
			.AddNode(nodeC)
			.AddNode(nodeD)
			.AddNode(nodeE)
			.AddNode(merge);

		executor.SetStartNode("start");
		executor.Connect("start", "normalize");
		executor.Connect("normalize", "A");
		executor.Connect("normalize", "B");
		executor.Connect("A", "C");
		executor.Connect("B", "C");
		executor.Connect("B", "D");
		executor.Connect("C", "E");
		executor.Connect("D", "E");
		executor.Connect("E", "merge");

		executor.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = 4
		});

		return executor;
	}
}


