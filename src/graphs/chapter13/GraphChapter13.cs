namespace GraphIABook.Graphs.Chapter13;

using System.Text.Json;
using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 13 — SKG graph with shallow branches to demonstrate coordination overhead.
/// Two trivial branches (header parse and body parse) run in parallel and then merge.
/// This illustrates scenarios where graph coordination adds overhead without real gains.
/// </summary>
public static class GraphChapter13
{
	public static readonly int HeaderParseMs = 1;
	public static readonly int BodyParseMs = 1;
	public static readonly int MergeMs = 2; // explicit coordination/aggregation cost

	public static async Task<string> RunAsync(string csv)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["csv"] = csv };
		var executor = CreateExecutor();
		var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
		if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 13.");
		var result = await executor.ExecuteAsync(kernel, args);
		return result.GetValue<string>() ?? string.Empty;
	}

	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
		var normalize = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var raw = a.TryGetValue("csv", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
			a["norm"] = raw.Replace("\r\n", "\n").Replace('\r', '\n');
			await Task.Delay(1);
			return "normalized";
		}, "Normalize"), nodeId: "normalize");

		var parseHeader = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var norm = a.TryGetValue("norm", out var nv) ? nv?.ToString() ?? string.Empty : string.Empty;
			var firstLine = norm.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
			a["headers"] = firstLine.Split(',', StringSplitOptions.TrimEntries);
			await Task.Delay(HeaderParseMs);
			return "header";
		}, "ParseHeader"), nodeId: "parseHeader");

!		var parseBody = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var norm = a.TryGetValue("norm", out var nv) ? nv?.ToString() ?? string.Empty : string.Empty;
			var lines = norm.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			var body = lines.Skip(1).ToArray();
			a["body"] = body;
			await Task.Delay(BodyParseMs);
			return "body";
		}, "ParseBody"), nodeId: "parseBody");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var headers = (a["headers"] as string[]) ?? Array.Empty<string>();
			var body = (a["body"] as string[]) ?? Array.Empty<string>();
			var list = new List<Dictionary<string, string>>();
			foreach (var line in body)
			{
				var cells = line.Split(',', StringSplitOptions.TrimEntries);
				var map = new Dictionary<string, string>(capacity: headers.Length);
				for (var c = 0; c < headers.Length && c < cells.Length; c++)
				{
					map[headers[c]] = cells[c];
				}
				list.Add(map);
			}
			await Task.Delay(MergeMs);
			return JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = false });
		}, "Merge"), nodeId: "merge");

		start.ConnectTo(normalize);
		normalize.ConnectTo(parseHeader);
		normalize.ConnectTo(parseBody);
		parseHeader.ConnectTo(merge);
		parseBody.ConnectTo(merge);

		var executor = new GraphExecutor("ch13_shallow_overhead", "Shallow branches with coordination overhead");
		executor.AddNode(start)
			.AddNode(normalize)
			.AddNode(parseHeader)
			.AddNode(parseBody)
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




