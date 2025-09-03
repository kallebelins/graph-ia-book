namespace GraphIABook.Chains.Chapter22;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 22 — SK chain modeling sequential fallback among three alternatives.
/// Computes overall success probability p_total and expected time E[T]
/// for a fixed order chosen by descending p/t as discussed in the chapter text.
/// Mirrors the content in docs/book/29-capitulo-22.md.
/// </summary>
public static class ChainChapter22
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var build = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				// Three alternatives from the chapter example
				double[] p = new[] { 0.6, 0.5, 0.4 };           // success probabilities
				int[] tMs = new[] { 200, 120, 80 };             // latencies in milliseconds
				// Order by descending p/t: indices 1 -> 0 -> 2
				int[] order = new[] { 1, 0, 2 };
				a["p"] = p;
				a["tMs"] = tMs;
				a["order"] = order;
				return Task.FromResult("built-params");
			},
			"BuildParams");

		var compute = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var p = (double[])a["p"]!;
				var t = (int[])a["tMs"]!;
				var order = (int[])a["order"]!;

				// success probability for independent attempts: 1 - Π(1 - p_i)
				double failProd = 1.0;
				foreach (var idx in order) failProd *= (1.0 - p[idx]);
				double pTotal = 1.0 - failProd;

				// Expected time for sequential fallback: E[T] = t1 + (1-p1) t2 + (1-p1)(1-p2) t3
				double expectedMs = 0.0;
				double prefixFail = 1.0;
				for (int k = 0; k < order.Length; k++)
				{
					int idx = order[k];
					expectedMs += prefixFail * t[idx];
					prefixFail *= (1.0 - p[idx]);
				}

				a["pTotal"] = pTotal;
				a["expectedMs"] = expectedMs;
				return Task.FromResult("computed");
			},
			"ComputeSeq");

		var merge = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var order = (int[])a["order"]!;
				double pTotal = (double)a["pTotal"]!;
				double expectedMs = (double)a["expectedMs"]!;
				string ordStr = string.Join("->", order.Select(i => (i + 1).ToString()));
				string summary = $"answer(chain22): p={pTotal:0.###}; E[T]={expectedMs:0}ms; order=[{ordStr}]";
				return Task.FromResult(summary);
			},
			"Merge");

		_ = await kernel.InvokeAsync(build, args);
		_ = await kernel.InvokeAsync(compute, args);
		var merged = await kernel.InvokeAsync(merge, args);
		return merged.GetValue<string>() ?? string.Empty;
	}
}
