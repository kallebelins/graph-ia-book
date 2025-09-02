namespace GraphIABook.Chains.Chapter1;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 1 — SK chain with 4 sequential stages simulating coupling:
/// preprocess → retrieve → reason → answer. Deterministic, no external services.
/// Stage durations are simulated to enable makespan comparisons vs graph.
/// </summary>
public static class ChainChapter1
{
	public static readonly int PreprocessMs = 3;
	public static readonly int RetrieveMs = 20;
	public static readonly int ReasonMs = 15;
	public static readonly int AnswerMs = 5;

	/// <summary>
	/// Runs the 4-stage sequential pipeline and returns the final answer.
	/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();

		var preprocess = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
				var cleaned = text.Trim().ToLowerInvariant();
				a["clean"] = cleaned;
				return "preprocessed";
			},
			"Preprocess");

		var retrieve = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var clean = a.TryGetValue("clean", out var cv) ? cv?.ToString() ?? string.Empty : string.Empty;
				var tokens = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				a["retrieved"] = string.Join(' ', tokens.Take(Math.Min(3, tokens.Length)));
				return "retrieved";
			},
			"Retrieve");

		var reason = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var r = a.TryGetValue("retrieved", out var rv) ? rv?.ToString() ?? string.Empty : string.Empty;
				a["reason"] = $"reasoning({r})";
				return "reasoned";
			},
			"Reason");

		var answer = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var rs = a.TryGetValue("reason", out var rsv) ? rsv?.ToString() ?? string.Empty : string.Empty;
				return $"Answer: {rs}";
			},
			"Answer");

		var args = new KernelArguments { ["input"] = input };

		// Simulate stage durations between sequential invocations
		await kernel.InvokeAsync(preprocess, args);
		await Task.Delay(PreprocessMs);

		await kernel.InvokeAsync(retrieve, args);
		await Task.Delay(RetrieveMs);

		await kernel.InvokeAsync(reason, args);
		await Task.Delay(ReasonMs);

		var result = await kernel.InvokeAsync(answer, args);
		await Task.Delay(AnswerMs);

		return result.GetValue<string>() ?? string.Empty;
	}
}



