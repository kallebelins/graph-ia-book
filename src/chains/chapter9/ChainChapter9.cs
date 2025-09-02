namespace GraphIABook.Chains.Chapter9;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 9 — SK chain modeling basic governance checks.
/// Limitations: single linear path; cannot branch to anonymizer before processing.
/// Stages: PolicyEval -> Processor (blocks when violation is detected).
/// </summary>
public static class ChainChapter9
{
    public static readonly int PolicyEvalMs = 15;
    public static readonly int ProcessorMs = 25;

    /// <summary>
    /// Runs a sequential pipeline with a basic policy evaluation followed by processing.
    /// If a violation is detected, the chain blocks instead of routing to anonymization.
    /// </summary>
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var policyEval = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                await Task.Delay(PolicyEvalMs);
                var raw = a["input"]?.ToString() ?? string.Empty;
                var violates = raw.Contains("cpf:", StringComparison.OrdinalIgnoreCase)
                               || raw.Contains("email:", StringComparison.OrdinalIgnoreCase);
                a["violates_policy"] = violates;
                return violates ? "violation" : "ok";
            },
            "PolicyEval");

        var processor = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                await Task.Delay(ProcessorMs);
                var violates = a.TryGetValue("violates_policy", out var v) && v is bool b && b;
                if (violates)
                {
                    return "blocked(chain): policy violation";
                }
                var content = (a["input"]?.ToString() ?? string.Empty).Trim();
                return $"answer(chain): {content.ToLowerInvariant()}";
            },
            "Processor");

        await kernel.InvokeAsync(policyEval, args);
        var result = await kernel.InvokeAsync(processor, args);
        return result.GetValue<string>() ?? string.Empty;
    }
}



