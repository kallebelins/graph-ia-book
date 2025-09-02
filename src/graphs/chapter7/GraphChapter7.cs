namespace GraphIABook.Graphs.Chapter7;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Extensions;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 7 — SKG graph with fallback (v2 → v2'), checkpoints, and partial re-execution.
/// Demonstrates recovery patterns using checkpointing options. Stages: v1 -> (v2 primary or v2' fallback) -> v3.
/// </summary>
public static class GraphChapter7
{
	public static readonly int V1Ms = 3;
	public static readonly int V2Ms = 8;
	public static readonly int V2FallbackMs = 3;
	public static readonly int V3Ms = 2;

	public static async Task<string> RunAsync(string input)
	{
		var builder = Kernel.CreateBuilder()
			.AddGraphSupport()
			.AddGraphMemory()
			.AddCheckpointSupport();

		var kernel = builder.Build();
		var args = new KernelArguments { ["input"] = input };

		var executor = CreateExecutorWithCheckpointing(builder);
		// Analyze acyclicity using GraphPlanCompiler requires GraphExecutor. Create a lightweight twin for analysis.
		var analysisExecutor = new GraphExecutor("cap7_fallback_graph_analysis", "analysis twin");
		foreach (var n in executor.Nodes.Values) analysisExecutor.AddNode(n);
		foreach (var e in executor.Edges) analysisExecutor.AddEdge(e);
		if (executor.StartNode is not null) analysisExecutor.SetStartNode(executor.StartNode.NodeId);
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(analysisExecutor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 7.");

		try
		{
			var result = await executor.ExecuteAsync(kernel, args);
			return result.GetValue<string>() ?? string.Empty;
		}
		catch
		{
			// Attempt recovery from latest checkpoint
			var execId = executor.LastExecutionId ?? args.GetOrCreateGraphState().StateId;
			var checkpoints = await executor.GetExecutionCheckpointsAsync(execId);
			if (checkpoints.Count > 0)
			{
				var latest = checkpoints.First();
				var recovered = await executor.ResumeFromCheckpointAsync(latest.CheckpointId, kernel);
				return recovered.GetValueAsString() ?? string.Empty;
			}
			throw;
		}
	}

	/// <summary>
	/// Creates a checkpointing-enabled executor with primary and fallback branches for v2.
	/// </summary>
	public static CheckpointingGraphExecutor CreateExecutorWithCheckpointing(IKernelBuilder builder)
	{
		var options = new CheckpointingOptions
		{
			CreateInitialCheckpoint = true,
			CreateFinalCheckpoint = true,
			CreateErrorCheckpoints = true,
			CheckpointInterval = 1,
		};

		var executor = builder.CreateCheckpointingGraph("cap7_fallback_graph", opts =>
		{
			opts.CreateInitialCheckpoint = options.CreateInitialCheckpoint;
			opts.CreateFinalCheckpoint = options.CreateFinalCheckpoint;
			opts.CreateErrorCheckpoints = options.CreateErrorCheckpoints;
			opts.CheckpointInterval = options.CheckpointInterval;
			opts.CriticalNodes.Add("v1");
			opts.CriticalNodes.Add("v2");
			opts.CriticalNodes.Add("v2_fallback");
			opts.CriticalNodes.Add("v3");
		});

		// Nodes
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var v1 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["v1"] = text.Trim();
			await Task.Delay(V1Ms);
			return "v1-ok";
		}, "V1"), nodeId: "v1");

		var v2 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(V2Ms);
			// Simulate failure 100% to force fallback path for demonstration
			throw new InvalidOperationException("Primary v2 failed");
		}, "V2_Primary"), nodeId: "v2");

		var v2Fallback = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(V2FallbackMs);
			a["v2_result"] = "v2'";
			return "v2-fallback-ok";
		}, "V2_Fallback"), nodeId: "v2_fallback");

		var v3 = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var v2r = a.TryGetValue("v2_result", out var rv) ? rv?.ToString() ?? string.Empty : "";
			await Task.Delay(V3Ms);
			return $"ok({v2r})";
		}, "V3"), nodeId: "v3");

		// Wiring: start -> v1 -> v2 (primary) and also availability to v2_fallback if primary fails -> v3
		start.ConnectTo(v1);
		v1.ConnectTo(v2);
		v1.ConnectTo(v2Fallback); // both available; recovery resumes from latest checkpoint and continues through whichever completed
		v2.ConnectTo(v3);
		v2Fallback.ConnectTo(v3);

		executor.AddNode(start)
			.AddNode(v1)
			.AddNode(v2)
			.AddNode(v2Fallback)
			.AddNode(v3);
		executor.SetStartNode("start");

		return executor;
	}
}


