namespace GraphIABook.Chains.Chapter14;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 14 — SK chain with a single step that incurs a variable latency.
/// Used to compare against an adaptive graph that can introduce an alternate path.
/// </summary>
public static class ChainChapter14
{
	/// <summary>
	/// Executes a single linear step that simulates the observed latency for node B.
	/// </summary>
	public static async Task<string> RunOnceAsync(int latencyMs)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments
		{
			["latencyMs"] = latencyMs
		};

		var processB = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var ms = a.TryGetValue("latencyMs", out var v) ? (v is int i ? i : int.Parse(v!.ToString()!)) : 0;
			await Task.Delay(ms);
			return $"B({ms}ms)";
		}, "ProcessB");

		var result = await kernel.InvokeAsync(processB, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}



