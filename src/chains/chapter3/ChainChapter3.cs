namespace GraphIABook.Chains.Chapter3;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 3 — SK chain with external conditional decision (if/switch outside SK functions).
/// Demonstrates limitation: orchestration logic is not part of the pipeline description.
/// Two alternative paths (A/B) are chosen by external code and then merged deterministically.
/// </summary>
public static class ChainChapter3
{
	public static readonly int PreprocessMs = 3;
	public static readonly int PathAMs = 6;
	public static readonly int PathBMs = 9;
	public static readonly int MergeMs = 4;

	/// <summary>
	/// Runs the conditional pipeline using an external if/switch to route between Path A and Path B.
/// Decision policy: even input length -> Path A; odd -> Path B. Deterministic for benchmarking.
/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var preprocess = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
				var clean = text.Trim();
				a["clean"] = clean;
				a["isEvenLen"] = clean.Length % 2 == 0;
				return "preprocessed";
			},
			"Preprocess");

		var pathA = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var clean = a.TryGetValue("clean", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
				a["branch"] = "A";
				a["branchValue"] = $"alpha({clean})";
				return "pathA";
			},
			"PathA");

		var pathB = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var clean = a.TryGetValue("clean", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
				a["branch"] = "B";
				a["branchValue"] = $"beta({clean})";
				return "pathB";
			},
			"PathB");

		var merge = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var branch = a.TryGetValue("branch", out var b) ? b?.ToString() ?? string.Empty : string.Empty;
				var value = a.TryGetValue("branchValue", out var bv) ? bv?.ToString() ?? string.Empty : string.Empty;
				return $"Final({branch}:{value})";
			},
			"Merge");

		await kernel.InvokeAsync(preprocess, args);
		await Task.Delay(PreprocessMs);

		var isEven = args.TryGetValue("isEvenLen", out var ev) && ev is bool b && b;
		if (isEven)
		{
			await kernel.InvokeAsync(pathA, args);
			await Task.Delay(PathAMs);
		}
		else
		{
			await kernel.InvokeAsync(pathB, args);
			await Task.Delay(PathBMs);
		}

		var result = await kernel.InvokeAsync(merge, args);
		await Task.Delay(MergeMs);

		return result.GetValue<string>() ?? string.Empty;
	}
}




