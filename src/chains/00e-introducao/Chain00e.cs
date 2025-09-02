namespace GraphIABook.Chains._00eIntroducao;

using Microsoft.SemanticKernel;

/// <summary>
/// Minimal SK chain: input → "LLM" → response.
/// Uses a deterministic in-memory function to avoid external dependencies.
/// </summary>
public static class Chain00e
{
	/// <summary>
	/// Executes the minimal chain returning a concise response for the given input.
/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var echoFn = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments args) =>
			{
				var text = args.ContainsKey("input") ? args["input"]?.ToString() ?? string.Empty : string.Empty;
				// Simulate a concise LLM response deterministically
				var response = text.Length <= 80 ? text : text.Substring(0, 80) + "...";
				return $"Resposta concisa: {response}";
			},
			"DeterministicLLM",
			"Returns a concise response deterministically");

		var result = await kernel.InvokeAsync(echoFn, new() { ["input"] = input });
		return result.GetValue<string>() ?? string.Empty;
	}
}


