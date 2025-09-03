namespace GraphIABook.Graphs.Chapter20;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 20 — SKG graph demonstrating hierarchical composition via SubgraphGraphNode.
/// Parent graph builds input and invokes a subgraph that computes FeatureA and FeatureB in parallel,
/// then merges into a deterministic result. Validates acyclicity and configures parallelism.
/// </summary>
public static class GraphChapter20
{
	public static async Task<string> RunAsync(int n)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["n"] = n };
		var executor = CreateExecutor();
		GraphValidationUtils.EnsureAcyclic(executor);
		var result = await executor.ExecuteAsync(kernel, args).ConfigureAwait(false);
		return result.GetValue<string>() ?? string.Empty;
	}

	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

		var build = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var size = a.TryGetValue("n", out var v) && v is int i ? i : 0;
			var data = Enumerable.Range(1, Math.Max(0, size)).ToArray();
			a["data"] = data;
			return Task.FromResult("built");
		}, "Build"), nodeId: "build");

		// Subgraph: computes FeatureA and FeatureB in parallel and merges
		var subgraph = CreateInnerSubgraph();
		var sub = new SubgraphGraphNode(
			subgraph,
			name: "Subgraph: Features",
			description: "Parallel compute of features",
			config: new SubgraphConfiguration
			{
				IsolationMode = SubgraphIsolationMode.IsolatedClone,
				InputMappings = { ["data"] = "data" },
				OutputMappings = { ["featureA"] = "featureA", ["featureB"] = "featureB" }
			},
			logger: null);

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			int fa = a.TryGetValue("featureA", out var fav) && fav is int ia ? ia : 0;
			int fb = a.TryGetValue("featureB", out var fbv) && fbv is int ib ? ib : 0;
			var summary = $"answer(graph20): A={fa}; B={fb}";
			return Task.FromResult(summary);
		}, "Merge"), nodeId: "merge");

		var exec = new GraphExecutor("ch20_hierarchical", "Hierarchical graph with subgraph parallel features");
		exec.AddNode(start)
			.AddNode(build)
			.AddNode(sub)
			.AddNode(merge);

		exec.SetStartNode("start");
		exec.Connect("start", "build");
		exec.Connect("build", sub.NodeId);
		exec.Connect(sub.NodeId, "merge");

		exec.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = 2
		});

		return exec;
	}

	private static GraphExecutor CreateInnerSubgraph()
	{
		var s = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "inner-start", "InnerStart"), nodeId: "s");

		var featureA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var data = (int[])a["data"]!;
			int sum = 0;
			foreach (var v in data)
			{
				await Task.Delay(1).ConfigureAwait(false);
				sum += v;
			}
			a["featureA"] = sum;
			return "fa";
		}, "FeatureA"), nodeId: "fa");

		var featureB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var data = (int[])a["data"]!;
			int sumSquares = 0;
			foreach (var v in data)
			{
				await Task.Delay(1).ConfigureAwait(false);
				sumSquares += v * v;
			}
			a["featureB"] = sumSquares;
			return "fb";
		}, "FeatureB"), nodeId: "fb");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) => Task.FromResult("inner-merged"), "InnerMerge"), nodeId: "m");

		var sub = new GraphExecutor("ch20_inner", "Inner subgraph: parallel features");
		sub.AddNode(s).AddNode(featureA).AddNode(featureB).AddNode(merge);
		sub.SetStartNode("s");
		sub.Connect("s", "fa");
		sub.Connect("s", "fb");
		sub.Connect("fa", "m");
		sub.Connect("fb", "m");
		sub.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 2 });
		return sub;
	}
}


