namespace GraphIABook.Graphs.Chapter1;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 1 — SKG graph: parallel retrieve and verifier, then deterministic merge.
/// Simulated stage durations enable comparison with the sequential chain.
/// </summary>
public static class GraphChapter1
{
	public static readonly int PreprocessMs = 3;
	public static readonly int RetrieveMs = 20;
	public static readonly int VerifyMs = 6;
	public static readonly int ReasonMs = 15;
	public static readonly int MergeMs = 4;

	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var executor = CreateExecutor();
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 1 experiment.");

		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}

	/// <summary>
	/// Creates the configured executor for the Chapter 1 graph experiment.
	/// </summary>
	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["clean"] = text.Trim().ToLowerInvariant();
			await Task.Delay(PreprocessMs);
			return "preprocessed";
		}, "Preprocess"), nodeId: "preprocess");

		var retrieve = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var clean = a.TryGetValue("clean", out var cv) ? cv?.ToString() ?? string.Empty : string.Empty;
			var tokens = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			a["retrieved"] = string.Join(' ', tokens.Take(Math.Min(3, tokens.Length)));
			await Task.Delay(RetrieveMs);
			return "retrieved";
		}, "Retrieve"), nodeId: "retrieve");

		var verifier = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var clean = a.TryGetValue("clean", out var cv) ? cv?.ToString() ?? string.Empty : string.Empty;
			a["verdict"] = clean.Length % 2 == 0 ? "ok" : "ok"; // deterministic OK
			await Task.Delay(VerifyMs);
			return "verified";
		}, "Verifier"), nodeId: "verify");

		var reason = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var r = a.TryGetValue("retrieved", out var rv) ? rv?.ToString() ?? string.Empty : string.Empty;
			var v = a.TryGetValue("verdict", out var vv) ? vv?.ToString() ?? string.Empty : string.Empty;
			a["reason"] = $"reasoning({r}|{v})";
			await Task.Delay(ReasonMs);
			return "reasoned";
		}, "Reason"), nodeId: "reason");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var rs = a.TryGetValue("reason", out var rsv) ? rsv?.ToString() ?? string.Empty : string.Empty;
			await Task.Delay(MergeMs);
			return $"Answer: {rs}";
		}, "Merge"), nodeId: "merge");

		start.ConnectTo(preprocess);
		preprocess.ConnectTo(retrieve);
		preprocess.ConnectTo(verifier);
		retrieve.ConnectTo(reason);
		verifier.ConnectTo(reason);
		reason.ConnectTo(merge);

		var executor = new GraphExecutor("ch1_parallel_retrieve_verify", "Parallel retrieve + verify, then merge");
		executor.AddNode(start)
			.AddNode(preprocess)
			.AddNode(retrieve)
			.AddNode(verifier)
			.AddNode(reason)
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



