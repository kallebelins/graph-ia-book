namespace GraphIABook.Graphs.Chapter3;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 3 — SKG graph with native conditional routing (ConditionalGraphNode) to two paths and merge.
/// Demonstrates built-in orchestration where decision and paths are part of the graph specification.
/// </summary>
public static class GraphChapter3
{
	public static readonly int PreprocessMs = 3;
	public static readonly int PathAMs = 6;
	public static readonly int PathBMs = 9;
	public static readonly int MergeMs = 4;

	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var executor = CreateExecutor();
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 3.");

		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}

	/// <summary>
	/// Creates a graph with: start -> preprocess -> conditional(A|B) -> merge -> end.
/// Condition: even cleaned input length routes to A, else to B.
/// </summary>
	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			var clean = text.Trim();
			a["clean"] = clean;
			a["isEvenLen"] = clean.Length % 2 == 0;
			await Task.Delay(PreprocessMs);
			return "preprocessed";
		}, "Preprocess"), nodeId: "preprocess");

		var cond = new ConditionalGraphNode(state =>
		{
			var isEven = state.GetValue<bool>("isEvenLen");
			return isEven;
		}, nodeId: "cond", name: "IfEvenLen", description: "Routes to A if even length, else B");

		var pathA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var clean = a.TryGetValue("clean", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
			a["branch"] = "A";
			a["branchValue"] = $"alpha({clean})";
			await Task.Delay(PathAMs);
			return "A";
		}, "PathA"), nodeId: "pathA");

		var pathB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var clean = a.TryGetValue("clean", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
			a["branch"] = "B";
			a["branchValue"] = $"beta({clean})";
			await Task.Delay(PathBMs);
			return "B";
		}, "PathB"), nodeId: "pathB");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var branch = a.TryGetValue("branch", out var b) ? b?.ToString() ?? string.Empty : string.Empty;
			var value = a.TryGetValue("branchValue", out var bv) ? bv?.ToString() ?? string.Empty : string.Empty;
			await Task.Delay(MergeMs);
			return $"Final({branch}:{value})";
		}, "Merge"), nodeId: "merge");

		start.ConnectTo(preprocess);
		preprocess.ConnectTo(cond);
		cond.AddTrueNode(pathA);
		cond.AddFalseNode(pathB);
		pathA.ConnectTo(merge);
		pathB.ConnectTo(merge);

		var executor = new GraphExecutor("ch3_conditional_routing", "Conditional routing with merge");
		executor.AddNode(start)
			.AddNode(preprocess)
			.AddNode(cond)
			.AddNode(pathA)
			.AddNode(pathB)
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




