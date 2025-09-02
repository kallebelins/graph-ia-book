namespace GraphIABook.Graphs.Chapter9;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;
using System.Text.RegularExpressions;

/// <summary>
/// Chapter 9 — SKG graph demonstrating governance with PolicyGuard and Anonymizer routing.
/// Flow: Start -> PolicyEval -> (Anonymizer if violation) -> Processor -> Answer.
/// </summary>
public static class GraphChapter9
{
    public static readonly int PolicyEvalMs = 15;
    public static readonly int AnonymizerMs = 20;
    public static readonly int ProcessorMs = 25;

    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateExecutor();
        GraphValidationUtils.EnsureAcyclic(executor);

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Builds the governance graph with policy evaluation and safe routing through anonymization when needed.
    /// </summary>
    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var policyEval = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(PolicyEvalMs);
            var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            var violates = raw.Contains("cpf:", StringComparison.OrdinalIgnoreCase)
                           || raw.Contains("email:", StringComparison.OrdinalIgnoreCase);
            a["violates_policy"] = violates;
            return violates ? "violation" : "ok";
        }, "PolicyEval"), nodeId: "policy");

        var anonymizer = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(AnonymizerMs);
            var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            var sanitized = raw;
            sanitized = Regex.Replace(sanitized, @"(?i)cpf:[^\s]+", "cpf:[redacted]");
            sanitized = Regex.Replace(sanitized, @"(?i)email:[^\s]+", "email:[redacted]");
            a["input"] = sanitized;
            a["violates_policy"] = false; // cleared after anonymization
            return "anonymized";
        }, "Anonymizer"), nodeId: "anon");

        var processor = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(ProcessorMs);
            var content = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            return $"answer(graph): {content.ToLowerInvariant()}";
        }, "Processor"), nodeId: "proc");

        var executor = new GraphExecutor("cap9_governance_graph", "Policy guard with anonymization and safe routing");
        executor.AddNode(start)
            .AddNode(policyEval)
            .AddNode(anonymizer)
            .AddNode(processor);

        executor.SetStartNode("start");
        executor.Connect("start", "policy");
        executor.ConnectWhen("policy", "anon", args => args.TryGetValue("violates_policy", out var v) && v is bool b && b, "to_anon");
        executor.ConnectWhen("policy", "proc", args => !(args.TryGetValue("violates_policy", out var v) && v is bool b && b), "to_proc");
        executor.Connect("anon", "proc");

        return executor;
    }
}



