namespace GraphIABook.Chains.Chapter24;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 24 — SK chain showcasing an anti-pattern (sequential fan-out) versus recommended
/// topology. Implements preprocess followed by five logically independent branches executed
/// sequentially, and a final deterministic aggregator.
/// </summary>
public static class ChainChapter24
{
    public static readonly int PreprocessMs = 5;
    public static readonly int[] BranchDurationsMs = new[] { 12, 10, 9, 8, 7 };
    public static readonly int AggregatorMs = 6;

    /// <summary>
    /// Runs a linear pipeline: preprocess -> b1 -> b2 -> b3 -> b4 -> b5 -> aggregate.
    /// All branches are logically independent but executed sequentially to illustrate
    /// the chain limitation compared to a parallel graph.
    /// </summary>
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();

        var preprocess = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
                a["norm"] = text.Trim().ToLowerInvariant();
                await Task.Delay(PreprocessMs);
                return "preprocessed";
            },
            "Preprocess");

        var branch1 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["b1"] = string.Join('-', norm.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));
                await Task.Delay(BranchDurationsMs[0]);
                return "B1";
            },
            "Branch1");

        var branch2 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["b2"] = new string(norm.Reverse().ToArray());
                await Task.Delay(BranchDurationsMs[1]);
                return "B2";
            },
            "Branch2");

        var branch3 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["b3"] = norm.Length.ToString();
                await Task.Delay(BranchDurationsMs[2]);
                return "B3";
            },
            "Branch3");

        var branch4 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["b4"] = string.Concat(norm.OrderBy(c => c));
                await Task.Delay(BranchDurationsMs[3]);
                return "B4";
            },
            "Branch4");

        var branch5 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["b5"] = new string(norm.Distinct().ToArray());
                await Task.Delay(BranchDurationsMs[4]);
                return "B5";
            },
            "Branch5");

        var aggregate = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var b1 = a.TryGetValue("b1", out var v1) ? v1?.ToString() ?? string.Empty : string.Empty;
                var b2 = a.TryGetValue("b2", out var v2) ? v2?.ToString() ?? string.Empty : string.Empty;
                var b3 = a.TryGetValue("b3", out var v3) ? v3?.ToString() ?? string.Empty : string.Empty;
                var b4 = a.TryGetValue("b4", out var v4) ? v4?.ToString() ?? string.Empty : string.Empty;
                var b5 = a.TryGetValue("b5", out var v5) ? v5?.ToString() ?? string.Empty : string.Empty;
                var answer = $"Answer: chain({b1}|{b2}|{b3}|{b4}|{b5})";
                a["answer"] = answer;
                await Task.Delay(AggregatorMs);
                return answer;
            },
            "Aggregate");

        var args = new KernelArguments { ["input"] = input };

        await kernel.InvokeAsync(preprocess, args);
        await kernel.InvokeAsync(branch1, args);
        await kernel.InvokeAsync(branch2, args);
        await kernel.InvokeAsync(branch3, args);
        await kernel.InvokeAsync(branch4, args);
        await kernel.InvokeAsync(branch5, args);
        var result = await kernel.InvokeAsync(aggregate, args);
        return result.GetValue<string>() ?? string.Empty;
    }
}


