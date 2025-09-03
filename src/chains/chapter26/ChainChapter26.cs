namespace GraphIABook.Chains.Chapter26;

using Microsoft.SemanticKernel;
using System.Collections.Generic;

/// <summary>
/// Chapter 26 — SK chain baseline (heuristic) for route selection.
/// Uses a simple historical mean to choose a route and simulates
/// execution latency for the chosen route.
/// </summary>
public static class ChainChapter26
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var build = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				// Synthetic features and historical latencies per route (A, B, C)
				var degree = new Dictionary<string, int> { ["A"] = 2, ["B"] = 3, ["C"] = 1 };
				var betweenness = new Dictionary<string, double> { ["A"] = 0.10, ["B"] = 0.20, ["C"] = 0.05 };
				var histMeanMs = new Dictionary<string, int> { ["A"] = 130, ["B"] = 100, ["C"] = 150 };
				var actualMs = new Dictionary<string, int> { ["A"] = 90, ["B"] = 120, ["C"] = 150 };

				a["degree"] = degree;
				a["betweenness"] = betweenness;
				a["histMeanMs"] = histMeanMs;
				a["actualMs"] = actualMs;
				return Task.FromResult("built");
			},
			"BuildFeatures");

		var baseline = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var hist = (Dictionary<string, int>)a["histMeanMs"]!;
				string chosen = hist.OrderBy(kv => kv.Value).First().Key;
				int expected = hist[chosen];
				a["chosenRoute"] = chosen;
				a["expectedMsBaseline"] = expected;
				return Task.FromResult(chosen);
			},
			"BaselineSelect");

		var execute = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				string route = (string)a["chosenRoute"]!;
				int expected = (int)a["expectedMsBaseline"]!;
				var actuals = (Dictionary<string, int>)a["actualMs"]!;
				int t = actuals[route];
				await Task.Delay(t).ConfigureAwait(false);
				return $"answer(chain26): route={route}; expectedMs={expected}; actualMs={t}";
			},
			"ExecuteRoute");

		_ = await kernel.InvokeAsync(build, args).ConfigureAwait(false);
		_ = await kernel.InvokeAsync(baseline, args).ConfigureAwait(false);
		var merged = await kernel.InvokeAsync(execute, args).ConfigureAwait(false);
		return merged.GetValue<string>() ?? string.Empty;
	}
}




