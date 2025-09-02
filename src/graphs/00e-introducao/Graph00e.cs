namespace GraphIABook.Graphs._00eIntroducao;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// SKG graph with two parallel branches (summarization/extraction simulated) converging on a join.
/// Deterministic, no external services required.
/// </summary>
public static class Graph00e
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var summarize = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			var summary = text.Length <= 40 ? text : text.Substring(0, 40) + "...";
			a["summary"] = summary;
			return "summarized";
		}, "Summarize"), nodeId: "summarize");

		var extract = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
			var tokens = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			a["keywords"] = string.Join(',', tokens.Take(Math.Min(5, tokens.Length)));
			return "extracted";
		}, "Extract"), nodeId: "extract");

		var join = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var s = a.TryGetValue("summary", out var sv) ? sv?.ToString() ?? string.Empty : string.Empty;
			var k = a.TryGetValue("keywords", out var kv) ? kv?.ToString() ?? string.Empty : string.Empty;
			return $"Join[summary={s}; keywords={k}]";
		}, "Join"), nodeId: "join");

		start.ConnectTo(summarize);
		start.ConnectTo(extract);
		summarize.ConnectTo(join);
		extract.ConnectTo(join);

		var executor = new GraphExecutor("00e_ForkJoin", "Intro fork/join example");
		executor.AddNode(start).AddNode(summarize).AddNode(extract).AddNode(join);
		executor.SetStartNode("start");
		executor.ConfigureConcurrency(new GraphConcurrencyOptions
		{
			EnableParallelExecution = true,
			MaxDegreeOfParallelism = 2
		});

		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}


