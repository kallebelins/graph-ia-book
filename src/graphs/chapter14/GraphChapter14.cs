namespace GraphIABook.Graphs.Chapter14;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 14 — Adaptive graph: can introduce an alternate path B' when observed latency exceeds a threshold.
/// This emulates a deterministic dynamic evolution rule as described in docs/book/19-capitulo-14.md.
/// </summary>
public static class GraphChapter14
{
	public static GraphExecutor CreateExecutor(bool includeAlternate)
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			// Pass-through; could normalize inputs
			await Task.Delay(1);
			return "preprocessed";
		}, "Preprocess"), nodeId: "preprocess");

		var processB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var ms = a.TryGetValue("latencyMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
			await Task.Delay(ms);
			a["lastNode"] = "B";
			return $"B({ms}ms)";
		}, "ProcessB"), nodeId: "B");

		var processBAlt = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			// Alternate path with bounded latency (e.g., cached or simplified routine)
			var altMs = a.TryGetValue("altLatencyMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 800;
			await Task.Delay(altMs);
			a["lastNode"] = "B'";
			return $"B'({altMs}ms)";
		}, "ProcessBAlt"), nodeId: "BPrime");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			// Merge simply returns which branch produced the output
			await Task.Delay(2);
			var last = a.TryGetValue("lastNode", out var ln) ? ln?.ToString() ?? string.Empty : string.Empty;
			return $"Final({last})";
		}, "Merge"), nodeId: "merge");

		start.ConnectTo(preprocess);
		preprocess.ConnectTo(processB);
		if (includeAlternate)
		{
			preprocess.ConnectTo(processBAlt);
			processB.ConnectTo(merge);
			processBAlt.ConnectTo(merge);
		}
		else
		{
			processB.ConnectTo(merge);
		}

		var executor = new GraphExecutor("ch14_adaptive_graph", includeAlternate ? "Adaptive with alternate path" : "Baseline without alternate path");
		executor.AddNode(start)
			.AddNode(preprocess)
			.AddNode(processB)
			.AddNode(merge);
		if (includeAlternate)
		{
			executor.AddNode(processBAlt);
		}
		executor.SetStartNode("start");
		executor.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = 4
		});
		return executor;
	}

	public static async Task<string> RunAsync(int latencyMs, bool includeAlternate, int? altLatencyMs = null)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments
		{
			["latencyMs"] = latencyMs
		};
		if (altLatencyMs is not null)
		{
			args["altLatencyMs"] = altLatencyMs.Value;
		}
		var executor = CreateExecutor(includeAlternate);
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 14.");
		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}



