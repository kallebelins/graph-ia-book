namespace GraphIABook.Chains.Chapter7;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 7 — SK chain with injected failure at stage 2 that aborts the pipeline.
/// Used to contrast with SKG fallback and checkpointing behavior.
/// </summary>
public static class ChainChapter7
{
	public static readonly int Stage1Ms = 4;
	public static readonly int Stage2Ms = 6;
	public static readonly int Stage3Ms = 3;

	/// <summary>
	/// Runs a 3-stage sequential pipeline and injects a failure in stage 2.
	/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();

		var stage1 = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(Stage1Ms);
				a["s1"] = (a["input"]?.ToString() ?? string.Empty).Trim();
				return "s1-ok";
			},
			"Stage1");

		var stage2 = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(Stage2Ms);
				throw new InvalidOperationException("Injected failure in stage 2");
			},
			"Stage2_Fails");

		var stage3 = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				await Task.Delay(Stage3Ms);
				return "unreachable";
			},
			"Stage3");

		var args = new KernelArguments { ["input"] = input };

		await kernel.InvokeAsync(stage1, args);
		await kernel.InvokeAsync(stage2, args); // throws and aborts
		// stage3 not reached

		return string.Empty;
	}
}


