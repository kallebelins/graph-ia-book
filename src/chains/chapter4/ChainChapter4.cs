namespace GraphIABook.Chains.Chapter4;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 4 — SK chain illustrating state explosion in linear pipelines.
/// Five sequential modules; each produces one of three outcomes (success, partial, critical)
/// decided deterministically from input length. Handling logic is external to the pipeline,
/// simulating tight coupling and duplicated control flow typical of chains.
/// </summary>
public static class ChainChapter4
{
	public static readonly int PreprocessMs = 3;
	public static readonly int StageMs = 4;
	public static readonly int HandlePartialMs = 2;
	public static readonly int HandleCriticalMs = 3;

	/// <summary>
	/// Runs a 5-stage linear pipeline with external branching/handling per stage.
	/// Returns a compact summary string of per-stage outcomes and final status.
	/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var preprocess = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			var clean = text.Trim();
			a["clean"] = clean;
			a["base"] = clean.Length;
			await Task.Delay(PreprocessMs);
			return "pre";
		}, "Preprocess");

		var stage = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var idx = a.TryGetValue("idx", out var iv) ? Convert.ToInt32(iv) : 0;
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
		}, "Stage");

		var outcomes = new List<string>(capacity: 5);
		await kernel.InvokeAsync(preprocess, args);

		for (var i = 1; i <= 5; i++)
		{
			args["idx"] = i;
			var res = await kernel.InvokeAsync(stage, args);
			var outcome = res.GetValue<string>() ?? "success";
			outcomes.Add(outcome);

			// External handling logic per stage (coupled outside the pipeline)
			if (outcome == "partial")
			{
				await Task.Delay(HandlePartialMs);
				a["last_handle"] = $"recover_{i}";
			}
			else if (outcome == "critical")
			{
				await Task.Delay(HandleCriticalMs);
				a["last_handle"] = $"abort_{i}";
			}
			else
			{
				a.Remove("last_handle");
			}
		}

		var summary = string.Join(",", outcomes);
		return $"ChainFinal([{summary}])";
	}
}




