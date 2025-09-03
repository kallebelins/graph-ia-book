namespace GraphIABook.Chains.Chapter16;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 16 — SK chain demonstrating expressivity limits: k independent modules are
/// executed serially (forced serialization) followed by a deterministic merge.
/// This mirrors the formal content in docs/book/22-capitulo-16.md where
/// T_chain = k * t + α for k equal-cost modules of duration t and a merge cost α.
/// </summary>
public static class ChainChapter16
{
	public static readonly int DefaultModules = 4;
	public static readonly int ModuleDurationMs = 60; // t
	public static readonly int MergeDurationMs = 40;  // α

	/// <summary>
	/// Runs a serialized evaluation of k independent modules, then merges.
/// </summary>
	public static async Task<string> RunAsync(string input, int k = 0, int? moduleDurationMs = null, int? mergeDurationMs = null)
	{
		int modules = k <= 0 ? DefaultModules : k;
		int t = moduleDurationMs ?? ModuleDurationMs;
		int alpha = mergeDurationMs ?? MergeDurationMs;

		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var module = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(t);
				var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
				int idx = a.TryGetValue("_idx", out var v) && int.TryParse(v?.ToString(), out var vi) ? vi : 0;
				a[$"m{idx}"] = $"M{idx}:{raw}";
				return $"M{idx}-ok";
			},
			"Module");

		var merge = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(alpha);
				var parts = new List<string>(capacity: modules);
				for (int i = 1; i <= modules; i++)
				{
					if (a.TryGetValue($"m{i}", out var vi)) parts.Add(vi?.ToString() ?? string.Empty);
				}
				return $"answer(chain): merge({string.Join('|', parts)})";
			},
			"Merge");

		// Forced serialization: run modules one after another
		for (int i = 1; i <= modules; i++)
		{
			args["_idx"] = i;
			await kernel.InvokeAsync(module, args);
		}

		var result = await kernel.InvokeAsync(merge, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}




