namespace GraphIABook.Graphs.Chapter4;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 4 — SKG graph illustrating modularity and convergence to contain state explosion.
/// Five stages; failures (partial/critical) converge to a shared handler node, reducing effective
/// state space by reusing subgraphs. Final merge aggregates either the straight success path or
/// the handled path.
/// </summary>
public static class GraphChapter4
{
	public static readonly int PreprocessMs = 3;
	public static readonly int StageMs = 4;
	public static readonly int HandleMs = 3;
	public static readonly int MergeMs = 3;

	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };
		var executor = CreateExecutor();
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 4.");
		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}

	/// <summary>
	/// start -> preprocess -> s1 -> cond1 -> (next | handle)
	///   -> s2 -> cond2 -> (next | handle) -> ... -> s5 -> merge -> end
	/// Both conds route failures to a single shared handler node to demonstrate convergence.
	/// </summary>
	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

		var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			var clean = text.Trim();
			a["clean"] = clean;
			a["base"] = clean.Length;
			await Task.Delay(PreprocessMs);
			return "pre";
		}, "Preprocess"), nodeId: "preprocess");

		FunctionGraphNode MakeStage(int idx)
		{
			return new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
			{
				var basis = a.TryGetValue("base", out var bv) ? Convert.ToInt32(bv) : 0;
				var outcome = (basis + idx) % 3 switch
				{
					0 => "success",
					1 => "partial",
					_ => "critical"
				};
				a[$"outcome_{idx}"] = outcome;
				await Task.Delay(StageMs);
				return outcome;
			}, $"Stage{idx}"), nodeId: $"s{idx}");
		}

		var s1 = MakeStage(1);
		var s2 = MakeStage(2);
		var s3 = MakeStage(3);
		var s4 = MakeStage(4);
		var s5 = MakeStage(5);

		ConditionalGraphNode MakeCond(int idx)
		{
			return new ConditionalGraphNode(state =>
			{
				var outcome = state.GetValue<string>($"outcome_{idx}");
				return outcome == "success"; // true => continue; false => handle
			}, nodeId: $"cond{idx}", name: $"IfSuccess{idx}", description: "Continue if success; else handle");
		}

		var c1 = MakeCond(1);
		var c2 = MakeCond(2);
		var c3 = MakeCond(3);
		var c4 = MakeCond(4);
		var c5 = MakeCond(5);

		var handle = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			await Task.Delay(HandleMs);
			return "handled";
		}, "Handle"), nodeId: "handle");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var last = new[] { 1, 2, 3, 4, 5 }
				.Select(i => a.TryGetValue($"outcome_{i}", out var v) ? v?.ToString() ?? string.Empty : string.Empty)
				.ToArray();
			var summary = string.Join(",", last);
			await Task.Delay(MergeMs);
			return $"GraphFinal([{summary}])";
		}, "Merge"), nodeId: "merge");

		// Wiring
		start.ConnectTo(preprocess);
		preprocess.ConnectTo(s1);
		s1.ConnectTo(c1);
		c1.AddTrueNode(s2);
		c1.AddFalseNode(handle);

		s2.ConnectTo(c2);
		c2.AddTrueNode(s3);
		c2.AddFalseNode(handle);

		s3.ConnectTo(c3);
		c3.AddTrueNode(s4);
		c3.AddFalseNode(handle);

		s4.ConnectTo(c4);
		c4.AddTrueNode(s5);
		c4.AddFalseNode(handle);

		s5.ConnectTo(c5);
		c5.AddTrueNode(merge);   // all-success path goes to merge
		c5.AddFalseNode(handle); // any failure also converges to handler

		handle.ConnectTo(merge);

		var executor = new GraphExecutor("ch4_modularity_convergence", "Shared handler convergence to contain state space");
		executor.AddNode(start)
			.AddNode(preprocess)
			.AddNode(s1)
			.AddNode(c1)
			.AddNode(s2)
			.AddNode(c2)
			.AddNode(s3)
			.AddNode(c3)
			.AddNode(s4)
			.AddNode(c4)
			.AddNode(s5)
			.AddNode(c5)
			.AddNode(handle)
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




