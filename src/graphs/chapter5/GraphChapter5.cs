namespace GraphIABook.Graphs.Chapter5;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using SemanticKernel.Graph.Streaming;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 5 — SKG graph focusing on explainability and auditability.
/// The graph uses explicit nodes: preprocess -> score -> conditional -> (A|B) -> aggregate.
/// We run it with StreamingGraphExecutor to emit a structured execution trace and enable metrics.
/// </summary>
public static class GraphChapter5
{
    public static readonly int PreprocessMs = 4;
    public static readonly int ScoringMs = 5;
    public static readonly int BranchAMs = 6;
    public static readonly int BranchBMs = 6;
    public static readonly int AggregateMs = 4;

    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateExecutor();
        GraphLoggingUtils.EnableMetrics(executor, production: false);
        GraphValidationUtils.EnsureAcyclic(executor);

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Runs with streaming to emit a trace to console.
    /// </summary>
    public static async Task RunWithTraceAsync(string input, CancellationToken cancellationToken = default)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var baseExecutor = CreateCoreExecutor();
        var streaming = new StreamingGraphExecutor(baseExecutor);

        var options = new StreamingExecutionOptions
        {
            EventTypesToEmit = new[]
            {
                GraphExecutionEventType.ExecutionStarted,
                GraphExecutionEventType.NodeStarted,
                GraphExecutionEventType.NodeCompleted,
                GraphExecutionEventType.ConditionEvaluated,
                GraphExecutionEventType.ExecutionCompleted
            }
        };

        var stream = streaming.ExecuteStreamAsync(kernel, args, options, cancellationToken);
        await foreach (var evt in stream.WithCancellation(cancellationToken))
        {
            switch (evt)
            {
                case GraphExecutionStartedEvent started:
                    Console.WriteLine($"[TRACE] START id={started.ExecutionId}");
                    break;
                case NodeExecutionStartedEvent ns:
                    Console.WriteLine($"[TRACE] NODE START id={ns.Node.NodeId} name={ns.Node.Name}");
                    break;
                case NodeExecutionCompletedEvent nc:
                    Console.WriteLine($"[TRACE] NODE DONE id={nc.Node.NodeId} durMs={nc.ExecutionDuration.TotalMilliseconds:F0}");
                    break;
                case ConditionEvaluatedEvent ce:
                    Console.WriteLine($"[TRACE] COND node={ce.NodeId} expr={ce.Expression} result={ce.Result}");
                    break;
                case GraphExecutionCompletedEvent done:
                    Console.WriteLine($"[TRACE] END durMs={done.TotalDuration.TotalMilliseconds:F0}");
                    break;
            }
        }
    }

    /// <summary>
    /// Creates the non-streaming executor for metrics/benchmarks.
    /// </summary>
    public static GraphExecutor CreateExecutor()
    {
        var baseExecutor = CreateCoreExecutor();
        return baseExecutor;
    }

    private static GraphExecutor CreateCoreExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var preprocess = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["norm"] = text.Trim().ToLowerInvariant();
            await Task.Delay(PreprocessMs);
            return "preprocessed";
        }, "Preprocess"), nodeId: "preprocess");

        var score = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            int vowels = norm.Count(c => "aeiou".Contains(c));
            double s = norm.Length == 0 ? 0 : (double)vowels / norm.Length;
            a["score"] = s;
            await Task.Delay(ScoringMs);
            return s.ToString("F2");
        }, "Score"), nodeId: "score");

        // Conditional routing node using template: score >= 0.5
        var cond = new ConditionalGraphNode("{{ gte score 0.5 }}", nodeId: "cond", name: "ScoreGate", description: "Route high/low confidence paths");

        var branchA = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["branch"] = "A";
            a["payload"] = string.Join('-', norm.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));
            await Task.Delay(BranchAMs);
            return "A";
        }, "BranchA"), nodeId: "a");

        var branchB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var norm = a.TryGetValue("norm", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["branch"] = "B";
            a["payload"] = new string(norm.Reverse().ToArray());
            await Task.Delay(BranchBMs);
            return "B";
        }, "BranchB"), nodeId: "b");

        var aggregate = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var branch = a.TryGetValue("branch", out var vb) ? vb?.ToString() ?? string.Empty : string.Empty;
            var payload = a.TryGetValue("payload", out var vp) ? vp?.ToString() ?? string.Empty : string.Empty;
            var answer = $"Answer: graph({branch}:{payload})";
            await Task.Delay(AggregateMs);
            return answer;
        }, "Aggregate"), nodeId: "merge");

        // Wire conditional paths
        cond.AddTrueNode(branchA).AddFalseNode(branchB);

        var executor = new GraphExecutor("ch5_explainability", "Explainability/Auditability with explicit conditional routing");
        executor.AddNode(start)
            .AddNode(preprocess)
            .AddNode(score)
            .AddNode(cond)
            .AddNode(branchA)
            .AddNode(branchB)
            .AddNode(aggregate);

        executor.SetStartNode("start");
        executor.Connect("start", "preprocess");
        executor.Connect("preprocess", "score");
        executor.Connect("score", "cond");
        executor.Connect("a", "merge");
        executor.Connect("b", "merge");

        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = false,
            MaxDegreeOfParallelism = 1
        });

        return executor;
    }
}



