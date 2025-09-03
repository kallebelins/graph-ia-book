namespace GraphIABook.Chains.Chapter12;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 12 — SK chain (linear) modeling an autonomous agent with rigid flow.
/// Demonstrates the limitation: no dynamic path selection, fixed sequence of steps.
/// Scenario: support agent decides among FAQ, Code Search, or Escalation — but here,
/// the decision is simulated imperatively and the pipeline remains linear.
/// </summary>
public static class ChainChapter12
{
    public static readonly int AnalyzeGoalMs = 5;
    public static readonly int SelectToolMs = 5;
    public static readonly int ExecuteMs = 12;
    public static readonly int ReviewMs = 6;

    /// <summary>
    /// Runs a rigid pipeline: analyze -> decide (if/else) -> execute -> review.
    /// The decision does not alter the pipeline structure (still sequential).
    /// </summary>
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();

        var analyzeGoal = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
                a["goal"] = text.Trim();
                return "analyzed";
            },
            "AnalyzeGoal");

        var decideTool = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var goal = a.TryGetValue("goal", out var gv) ? gv?.ToString() ?? string.Empty : string.Empty;
                // Imperative decision: simple heuristic
                string tool = goal.Contains("erro", StringComparison.OrdinalIgnoreCase) ||
                              goal.Contains("exception", StringComparison.OrdinalIgnoreCase)
                    ? "CodeSearch"
                    : (goal.Contains("preço", StringComparison.OrdinalIgnoreCase) || goal.Contains("faq", StringComparison.OrdinalIgnoreCase)
                        ? "FAQ"
                        : "Escalate");
                a["tool"] = tool;
                return tool;
            },
            "DecideTool");

        var execute = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var tool = a.TryGetValue("tool", out var tv) ? tv?.ToString() ?? string.Empty : string.Empty;
                a["result"] = tool switch
                {
                    "FAQ" => "Resposta encontrada na FAQ",
                    "CodeSearch" => "Snippet localizado na base de código",
                    _ => "Encaminhado para humano"
                };
                return "executed";
            },
            "Execute");

        var review = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var res = a.TryGetValue("result", out var rv) ? rv?.ToString() ?? string.Empty : string.Empty;
                return $"Final: {res}";
            },
            "Review");

        var args = new KernelArguments { ["input"] = input };

        await kernel.InvokeAsync(analyzeGoal, args);
        await Task.Delay(AnalyzeGoalMs);

        await kernel.InvokeAsync(decideTool, args);
        await Task.Delay(SelectToolMs);

        await kernel.InvokeAsync(execute, args);
        await Task.Delay(ExecuteMs);

        var result = await kernel.InvokeAsync(review, args);
        await Task.Delay(ReviewMs);

        return result.GetValue<string>() ?? string.Empty;
    }
}



