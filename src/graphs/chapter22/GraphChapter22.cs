namespace GraphIABook.Graphs.Chapter22;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 22 — SKG graph modeling parallel OR among three independent alternatives.
/// Computes overall success probability p_or = 1 - Π(1 - p_i) and expected time
/// as the minimum return time among branches under a simple cancellation policy.
/// For reproducibility and simplicity, we approximate E[T_min] using a deterministic
/// surrogate with ordered latencies and independent success flags.
/// </summary>
public static class GraphChapter22
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };
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
			// Three alternatives from the chapter example
			double[] p = new[] { 0.6, 0.5, 0.4 };
			int[] tMs = new[] { 200, 120, 80 };
			a["p"] = p; a["tMs"] = tMs;
			return Task.FromResult("built-params");
		}, "BuildParams"), nodeId: "build");

		var prob = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var p = (double[])a["p"]!;
			double failProd = 1.0;
			for (int k = 0; k < p.Length; k++) failProd *= (1.0 - p[k]);
			a["p_or"] = 1.0 - failProd;
			return Task.FromResult("p_or");
		}, "ComputeProbOR"), nodeId: "por");

		var tmin = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var p = (double[])a["p"]!;
			var t = (int[])a["tMs"]!;
			// Deterministic surrogate for E[min T_i | any success]: choose the smallest latency branch
			// as a lower-bound proxy when at least one succeeds.
			int tMin = t.Min();
			// Expected time approximation with a simple mixture: E[T] ≈ p_or * t_min + (1-p_or) * max(t)
			double pOr = (double)a["p_or"]!;
			int tMax = t.Max();
			double expectedMs = pOr * tMin + (1.0 - pOr) * tMax;
			a["expectedMs_or"] = expectedMs;
			return Task.FromResult("tmin");
		}, "ApproxExpectedTime"), nodeId: "tmin");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			double pOr = (double)a["p_or"]!;
			double expectedMs = (double)a["expectedMs_or"]!;
			return Task.FromResult($"answer(graph22): p_or={pOr:0.###}; E[T]_approx={expectedMs:0}ms");
		}, "Merge"), nodeId: "merge");

		var exec = new GraphExecutor("ch22_parallel_or", "Parallel OR with independent branches and deterministic merge");
		exec.AddNode(start).AddNode(build).AddNode(prob).AddNode(tmin).AddNode(merge);
		exec.SetStartNode("start");
		exec.Connect("start", "build");
		exec.Connect("build", "por");
		exec.Connect("build", "tmin");
		exec.Connect("por", "merge");
		exec.Connect("tmin", "merge");
		exec.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 2 });
		return exec;
	}
}
