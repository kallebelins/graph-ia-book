namespace GraphIABook.Chains.Chapter5;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 5 — SK chain focusing on the explainability limitation of linear pipelines.
/// Implements a simple preprocess -> score -> branch -> aggregate flow where the branching
/// decision happens imperatively (if/else), without an explicit execution trace or audit trail.
/// </summary>
public static class ChainChapter5
{
    public static readonly int PreprocessMs = 4;
    public static readonly int ScoringMs = 5;
    public static readonly int BranchAMs = 6;
    public static readonly int BranchBMs = 6;
    public static readonly int AggregateMs = 4;

    /// <summary>
    /// Runs a linear 4-stage pipeline with an internal if/else decision.
    /// This chain does not emit a structured execution trace; only the final result is returned.
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

        var score = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                // Simple deterministic score for branching: fraction of vowels in string
                int vowels = norm.Count(c => "aeiou".Contains(c));
                double s = norm.Length == 0 ? 0 : (double)vowels / norm.Length;
                a["score"] = s;
                await Task.Delay(ScoringMs);
                return "scored";
            },
            "Score");

        var branchA = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["branch"] = "A";
                a["payload"] = string.Join('-', norm.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));
                await Task.Delay(BranchAMs);
                return "A";
            },
            "BranchA");

        var branchB = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["branch"] = "B";
                a["payload"] = new string(norm.Reverse().ToArray());
                await Task.Delay(BranchBMs);
                return "B";
            },
            "BranchB");

        var aggregate = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                var branch = a.TryGetValue("branch", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
                var payload = a.TryGetValue("payload", out var vp) ? vp?.ToString() ?? string.Empty : string.Empty;
                var answer = $"Answer: chain({branch}:{payload})";
                a["answer"] = answer;
                await Task.Delay(AggregateMs);
                return answer;
            },
            "Aggregate");

        var args = new KernelArguments { ["input"] = input };

        await kernel.InvokeAsync(preprocess, args);
        await kernel.InvokeAsync(score, args);

        // Imperative decision: not captured as an explicit execution path in the chain
        double sValue = args.TryGetValue("score", out var vs) && vs is double d ? d : 0.0;
        if (sValue >= 0.5)
        {
            await kernel.InvokeAsync(branchA, args);
        }
        else
        {
            await kernel.InvokeAsync(branchB, args);
        }

        var result = await kernel.InvokeAsync(aggregate, args);
        return result.GetValue<string>() ?? string.Empty;
    }
}



