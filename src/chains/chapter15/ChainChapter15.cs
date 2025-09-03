namespace GraphIABook.Chains.Chapter15;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 15 — SK chain implementing a 3-branch aggregation sequentially.
/// This models a synthesis pipeline where three analyses are executed in sequence
/// before a deterministic merge, reflecting chain limitations.
/// </summary>
public static class ChainChapter15
{
	/// <summary>
	/// Runs a sequential pipeline: Preprocess -> A -> B -> C -> Merge.
	/// Branch durations are controlled via parameters to enable deterministic benchmarking.
	/// </summary>
	public static async Task<string> RunAsync(int aDurationMs, int bDurationMs, int cDurationMs)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments
		{
			["aMs"] = aDurationMs,
			["bMs"] = bDurationMs,
			["cMs"] = cDurationMs
		};

		var preprocess = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(5);
				a["pre"] = true;
				return "pre-ok";
			},
			"Preprocess");

		var branchA = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				var ms = a.TryGetValue("aMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
				await Task.Delay(ms);
				a["a"] = $"A({ms}ms)";
				return "A-ok";
			},
			"BranchA");

		var branchB = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				var ms = a.TryGetValue("bMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
				await Task.Delay(ms);
				a["b"] = $"B({ms}ms)";
				return "B-ok";
			},
			"BranchB");

		var branchC = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				var ms = a.TryGetValue("cMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
				await Task.Delay(ms);
				a["c"] = $"C({ms}ms)";
				return "C-ok";
			},
			"BranchC");

		var merge = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(10);
				var av = a.TryGetValue("a", out var va) ? va?.ToString() ?? string.Empty : string.Empty;
				var bv = a.TryGetValue("b", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
				var cv = a.TryGetValue("c", out var vc) ? vc?.ToString() ?? string.Empty : string.Empty;
				return $"answer(chain): merge({av}|{bv}|{cv})";
			},
			"Merge");

		await kernel.InvokeAsync(preprocess, args);
		await kernel.InvokeAsync(branchA, args);
		await kernel.InvokeAsync(branchB, args);
		await kernel.InvokeAsync(branchC, args);
		var result = await kernel.InvokeAsync(merge, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}




