namespace GraphIABook.Chains.Chapter10;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 10 — SK chain implementing a diamond pattern sequentially: A -> B -> Merge.
/// This intentionally executes the two logical branches in sequence to mirror chain limitations.
/// </summary>
public static class ChainChapter10
{
	public static readonly int BranchADurationMs = 90;
	public static readonly int BranchBDurationMs = 60;
	public static readonly int MergeDurationMs = 40;

	/// <summary>
/// Runs a sequential "diamond" where branch A then branch B are computed before a final merge.
/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var branchA = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(BranchADurationMs);
				var raw = a["input"]?.ToString() ?? string.Empty;
				a["a"] = $"A:{raw.ToUpperInvariant()}";
				return "A-ok";
			},
			"BranchA");

		var branchB = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(BranchBDurationMs);
				var raw = a["input"]?.ToString() ?? string.Empty;
				a["b"] = $"B:{new string(raw.Reverse().ToArray())}";
				return "B-ok";
			},
			"BranchB");

		var merge = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(MergeDurationMs);
				var av = a.TryGetValue("a", out var va) ? va?.ToString() ?? string.Empty : string.Empty;
				var bv = a.TryGetValue("b", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
				return $"answer(chain): merge({av}|{bv})";
			},
			"Merge");

		await kernel.InvokeAsync(branchA, args);
		await kernel.InvokeAsync(branchB, args);
		var result = await kernel.InvokeAsync(merge, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}



