namespace GraphIABook.Chains.Chapter17;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 17 — SK chain modeling layered DAG scheduling but executed serially.
/// Demonstrates lack of parallelism: all nodes are run in sequence following
/// a valid topological order, so the makespan equals the sum of node durations
/// plus aggregation overhead.
/// Mirrors concepts in docs/book/23-capitulo-17.md about critical path and
/// topological execution.
/// </summary>
public static class ChainChapter17
{
	public static readonly int DurationA = 100; // ms
	public static readonly int DurationB = 130; // ms (dominant among sources)
	public static readonly int DurationC = 90;  // ms
	public static readonly int DurationD = 110; // ms
	public static readonly int DurationE = 70;  // ms
	public static readonly int DurationMerge = 50; // ms (aggregator)

	/// <summary>
	/// Executes a topological order of the layered DAG (A,B -> C,D -> E -> merge)
	/// entirely in sequence to emulate a chain without parallelism.
	/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var normalize = KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			a["norm"] = raw.Trim().ToLowerInvariant();
			return Task.FromResult("normalized");
		}, "Normalize");

		var a = KernelFunctionFactory.CreateFromMethod(async (KernelArguments ka) =>
		{
			await Task.Delay(DurationA);
			ka["A"] = $"A:{ka["norm"]}";
			return "A";
		}, "A");

		var b = KernelFunctionFactory.CreateFromMethod(async (KernelArguments kb) =>
		{
			await Task.Delay(DurationB);
			kb["B"] = $"B:{kb["norm"]}";
			return "B";
		}, "B");

		var c = KernelFunctionFactory.CreateFromMethod(async (KernelArguments kc) =>
		{
			await Task.Delay(DurationC);
			var src = (kc.TryGetValue("A", out var va) ? va?.ToString() : string.Empty) + "|" + (kc.TryGetValue("B", out var vb) ? vb?.ToString() : string.Empty);
			kc["C"] = $"C:({src})";
			return "C";
		}, "C");

		var d = KernelFunctionFactory.CreateFromMethod(async (KernelArguments kd) =>
		{
			await Task.Delay(DurationD);
			var src = (kd.TryGetValue("B", out var vb) ? vb?.ToString() : string.Empty);
			kd["D"] = $"D:({src})";
			return "D";
		}, "D");

		var e = KernelFunctionFactory.CreateFromMethod(async (KernelArguments ke) =>
		{
			await Task.Delay(DurationE);
			var src = (ke.TryGetValue("C", out var vc) ? vc?.ToString() : string.Empty) + "|" + (ke.TryGetValue("D", out var vd) ? vd?.ToString() : string.Empty);
			ke["E"] = $"E:({src})";
			return "E";
		}, "E");

		var merge = KernelFunctionFactory.CreateFromMethod(async (KernelArguments km) =>
		{
			await Task.Delay(DurationMerge);
			var payload = km.TryGetValue("E", out var ve) ? ve?.ToString() ?? string.Empty : string.Empty;
			return $"answer(chain): merge({payload})";
		}, "Merge");

		// Forced serialization: normalize, A, B, C, D, E, merge
		_ = await kernel.InvokeAsync(normalize, args);
		_ = await kernel.InvokeAsync(a, args);
		_ = await kernel.InvokeAsync(b, args);
		_ = await kernel.InvokeAsync(c, args);
		_ = await kernel.InvokeAsync(d, args);
		_ = await kernel.InvokeAsync(e, args);
		var result = await kernel.InvokeAsync(merge, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}


