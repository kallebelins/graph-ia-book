namespace GraphIABook.Graphs.Chapter16;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 16 — SKG graph demonstrating expressivity: k independent modules executed in
/// parallel with a deterministic merge. Reflects T_DAG ≈ t + α for k equal-cost modules (t)
/// plus merge cost α. Optionally can emulate a reduction tree by increasing merge cost by
/// O(log k) levels, aligning with the formal notes in docs/book/22-capitulo-16.md.
/// </summary>
public static class GraphChapter16
{
	public static readonly int DefaultModules = 4;
	public static readonly int ModuleDurationMs = 60; // t
	public static readonly int MergeDurationMs = 40;  // α

	public static async Task<string> RunAsync(string input, int k = 0, int? moduleDurationMs = null, int? mergeDurationMs = null, bool emulateReductionTree = false)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var executor = CreateExecutor(k, moduleDurationMs, mergeDurationMs, emulateReductionTree);
		GraphValidationUtils.EnsureAcyclic(executor);

		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}

	/// <summary>
	/// Builds a parallel k-branch graph: start -> normalize -> (M1..Mk in parallel) -> merge.
	/// </summary>
	public static GraphExecutor CreateExecutor(int k = 0, int? moduleDurationMs = null, int? mergeDurationMs = null, bool emulateReductionTree = false)
	{
		int modules = k <= 0 ? DefaultModules : k;
		int t = moduleDurationMs ?? ModuleDurationMs;
		int alpha = mergeDurationMs ?? MergeDurationMs;

		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var normalize = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["norm"] = raw.Trim().ToLowerInvariant();
			return Task.FromResult("normalized");
		}, "Normalize"), nodeId: "normalize");

		var moduleNodes = new List<FunctionGraphNode>(capacity: modules);
		for (int i = 1; i <= modules; i++)
		{
			int idx = i;
			var node = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
			{
				await Task.Delay(t);
				var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
				a[$"m{idx}"] = $"M{idx}:{norm}";
				return $"M{idx}";
			}, $"Module{idx}"), nodeId: $"m{idx}");
			moduleNodes.Add(node);
		}

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			// Emulate reduction tree by multiplying α by ceil(log2 k) if requested
			int levels = emulateReductionTree && modules > 1 ? (int)Math.Ceiling(Math.Log2(modules)) : 1;
			await Task.Delay(alpha * levels);
			var parts = new List<string>(capacity: modules);
			for (int i = 1; i <= modules; i++)
			{
				if (a.TryGetValue($"m{i}", out var vi)) parts.Add(vi?.ToString() ?? string.Empty);
			}
			return $"answer(graph): merge({string.Join('|', parts)})";
		}, "Merge"), nodeId: "merge");

		// Wire graph
		var executor = new GraphExecutor("ch16_parallel_k", "k parallel modules with deterministic merge");
		executor.AddNode(start)
			.AddNode(normalize)
			.AddNode(merge);
		foreach (var n in moduleNodes) executor.AddNode(n);

		executor.SetStartNode("start");
		executor.Connect("start", "normalize");
		foreach (var n in moduleNodes) executor.Connect("normalize", n.NodeId);
		foreach (var n in moduleNodes) executor.Connect(n.NodeId, "merge");

		executor.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = Math.Max(2, modules)
		});

		return executor;
	}
}




