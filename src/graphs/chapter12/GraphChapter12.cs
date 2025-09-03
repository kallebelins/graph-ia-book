namespace GraphIABook.Graphs.Chapter12;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 12 — SKG graph modeling an autonomous agent with dynamic routing.
/// Scenario: support agent can route to FAQ, Code Search, or Escalation based on state.
/// Demonstrates structural autonomy: decisions are part of the graph and change the path.
/// </summary>
public static class GraphChapter12
{
    public static readonly int AnalyzeGoalMs = 5;
    public static readonly int RouteMs = 2;
    public static readonly int FAQMs = 8;
    public static readonly int CodeSearchMs = 12;
    public static readonly int EscalateMs = 4;
    public static readonly int MergeMs = 5;

    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateExecutor();
        var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
        if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 12.");

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Graph: start -> analyze -> cond(FAQ|Code|Escalate) -> merge -> end.
    /// Condition derives from input content flags.
    /// </summary>
    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var analyze = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            var clean = text.Trim();
            a["clean"] = clean;
            a["needsFAQ"] = clean.Contains("preço", StringComparison.OrdinalIgnoreCase) || clean.Contains("faq", StringComparison.OrdinalIgnoreCase);
            a["needsCode"] = clean.Contains("erro", StringComparison.OrdinalIgnoreCase) || clean.Contains("exception", StringComparison.OrdinalIgnoreCase);
            await Task.Delay(AnalyzeGoalMs);
            return "analyzed";
        }, "Analyze"), nodeId: "analyze");

        var cond = new ConditionalGraphNode(state =>
        {
            // true path will be FAQ, false will be further split using another condition node
            var needsFaq = state.GetValue<bool>("needsFAQ");
            return needsFaq;
        }, nodeId: "condFaq", name: "IfFAQ", description: "Route to FAQ when applicable");

        var condCode = new ConditionalGraphNode(state =>
        {
            // If not FAQ, check Code vs Escalate
            var needsCode = state.GetValue<bool>("needsCode");
            return needsCode;
        }, nodeId: "condCode", name: "IfCode", description: "Route to CodeSearch else Escalate");

        var faq = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var clean = a.TryGetValue("clean", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["branch"] = "FAQ";
            a["branchValue"] = $"faq-ans({clean.Length})";
            await Task.Delay(FAQMs);
            return "faq";
        }, "FAQ"), nodeId: "faq");

        var code = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var clean = a.TryGetValue("clean", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["branch"] = "CodeSearch";
            a["branchValue"] = $"code-hit({Math.Max(1, clean.Split(' ').Length / 3)})";
            await Task.Delay(CodeSearchMs);
            return "code";
        }, "CodeSearch"), nodeId: "code");

        var escalate = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["branch"] = "Escalate";
            a["branchValue"] = "human";
            await Task.Delay(EscalateMs);
            return "escalate";
        }, "Escalate"), nodeId: "escalate");

        var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var b = a.TryGetValue("branch", out var bv) ? bv?.ToString() ?? string.Empty : string.Empty;
            var val = a.TryGetValue("branchValue", out var vv) ? vv?.ToString() ?? string.Empty : string.Empty;
            await Task.Delay(MergeMs);
            return $"Final({b}:{val})";
        }, "Merge"), nodeId: "merge");

        start.ConnectTo(analyze);
        analyze.ConnectTo(cond);
        cond.AddTrueNode(faq);
        cond.AddFalseNode(condCode);
        condCode.AddTrueNode(code);
        condCode.AddFalseNode(escalate);
        faq.ConnectTo(merge);
        code.ConnectTo(merge);
        escalate.ConnectTo(merge);

        var executor = new GraphExecutor("ch12_agent_autonomy", "Autonomous agent with dynamic routing (FAQ/Code/Escalate)");
        executor.AddNode(start)
            .AddNode(analyze)
            .AddNode(cond)
            .AddNode(condCode)
            .AddNode(faq)
            .AddNode(code)
            .AddNode(escalate)
            .AddNode(merge);
        executor.SetStartNode("start");
        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = 4
        });
        return executor;
    }
}



