namespace GraphIABook.Chains.Chapter20;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 20 — SK chain demonstrating hierarchical subpipeline (sequential inner steps).
/// Mirrors the SKG version that uses a subgraph with parallel branches.
/// - Build input data of size n
/// - Compute FeatureA and FeatureB sequentially (simulating inner pipeline)
/// - Merge/format final result
/// </summary>
public static class ChainChapter20
{
	public static async Task<string> RunAsync(int n)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["n"] = n };

		var build = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				var size = a.TryGetValue("n", out var v) && v is int i ? i : 0;
				var data = Enumerable.Range(1, Math.Max(0, size)).ToArray();
				a["data"] = data;
				return Task.FromResult("built");
			},
			"Build");

		var featureA = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				var data = (int[])a["data"]!;
				int sum = 0;
				foreach (var v in data)
				{
					await Task.Delay(1).ConfigureAwait(false);
					sum += v;
				}
				a["featureA"] = sum;
				return "featureA";
			},
			"FeatureA");

		var featureB = KernelFunctionFactory.CreateFromMethod(
			async (KernelArguments a) =>
			{
				var data = (int[])a["data"]!;
				int sumSquares = 0;
				foreach (var v in data)
				{
					await Task.Delay(1).ConfigureAwait(false);
					sumSquares += v * v;
				}
				a["featureB"] = sumSquares;
				return "featureB";
			},
			"FeatureB");

		var merge = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				int fa = a.TryGetValue("featureA", out var fav) && fav is int ia ? ia : 0;
				int fb = a.TryGetValue("featureB", out var fbv) && fbv is int ib ? ib : 0;
				var summary = $"answer(chain20): A={fa}; B={fb}";
				return Task.FromResult(summary);
			},
			"Merge");

		_ = await kernel.InvokeAsync(build, args).ConfigureAwait(false);
		_ = await kernel.InvokeAsync(featureA, args).ConfigureAwait(false);
		_ = await kernel.InvokeAsync(featureB, args).ConfigureAwait(false);
		var merged = await kernel.InvokeAsync(merge, args).ConfigureAwait(false);
		return merged.GetValue<string>() ?? string.Empty;
	}
}


