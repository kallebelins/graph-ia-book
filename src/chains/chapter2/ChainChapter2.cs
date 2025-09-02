namespace GraphIABook.Chains.Chapter2;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 2 — SK chain focusing on expressivity limitations of linear pipelines.
/// Demonstrates a strictly sequential flow with single path of execution to contrast
/// against the graph version which has parallel/alternative paths.
/// All stages are deterministic and include small simulated delays for benchmarking.
/// </summary>
public static class ChainChapter2
{
    public static readonly int Stage1Ms = 4;
    public static readonly int Stage2Ms = 6;
    public static readonly int Stage3Ms = 7;

    /// <summary>
    /// Runs a linear 3-stage pipeline and returns a final answer string.
    /// </summary>
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();

        var stage1 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
                a["s1"] = text.Trim().ToLowerInvariant();
                await Task.Delay(Stage1Ms);
                return "s1";
            },
            "Stage1");

        var stage2 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var s1 = a.TryGetValue("s1", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["s2"] = new string(s1.Reverse().ToArray());
                await Task.Delay(Stage2Ms);
                return "s2";
            },
            "Stage2");

        var stage3 = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var s2 = a.TryGetValue("s2", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["answer"] = $"Answer: linear({s2})";
                await Task.Delay(Stage3Ms);
                return a["answer"]?.ToString() ?? string.Empty;
            },
            "Stage3");

        var args = new KernelArguments { ["input"] = input };

        await kernel.InvokeAsync(stage1, args);
        await kernel.InvokeAsync(stage2, args);
        var result = await kernel.InvokeAsync(stage3, args);

        return result.GetValue<string>() ?? string.Empty;
    }
}


