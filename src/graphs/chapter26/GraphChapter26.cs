namespace GraphIABook.Graphs.Chapter26;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using SemanticKernel.Graph.State;

/// <summary>
/// Chapter 26 — SKG graph with a predictor informing route selection.
/// The predictor uses structural features (degree, betweenness) to score routes
/// and chooses a route with the lowest predicted latency. We then simulate the
/// execution latency for the chosen route. Baseline chain uses historical mean.
/// </summary>
public static class GraphChapter26
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };
		var executor = CreateExecutor();
		var result = await executor.ExecuteAsync(kernel, args).ConfigureAwait(false);
		return result.GetValue<string>() ?? string.Empty;
	}

	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

		var build = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			// Same synthetic world as chain, but we will choose route using a predictor.
			var degree = new Dictionary<string, int> { ["A"] = 2, ["B"] = 3, ["C"] = 1 };
			var betweenness = new Dictionary<string, double> { ["A"] = 0.10, ["B"] = 0.20, ["C"] = 0.05 };
			var histMeanMs = new Dictionary<string, int> { ["A"] = 130, ["B"] = 100, ["C"] = 150 };
			var actualMs = new Dictionary<string, int> { ["A"] = 90, ["B"] = 120, ["C"] = 150 };

			a["degree"] = degree;
			a["betweenness"] = betweenness;
			a["histMeanMs"] = histMeanMs;
			a["actualMs"] = actualMs;
			return Task.FromResult("built");
		}, "BuildFeatures"), nodeId: "build");

		// Predictor: predicts latency score = w1*degree + w2*betweenness*1000 (ms proxy)
		var predict = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var deg = (Dictionary<string, int>)a["degree"]!;
			var btw = (Dictionary<string, double>)a["betweenness"]!;
			double w1 = 20.0;    // ms per degree unit
			double w2 = 60.0;    // ms per betweenness*1000 units
			var score = new Dictionary<string, double>();
			foreach (var k in deg.Keys)
			{
				var s = w1 * deg[k] + w2 * (btw[k] * 1000.0);
				score[k] = s;
			}
			a["predictedScoreMs"] = score;
			string chosen = score.OrderBy(kv => kv.Value).First().Key;
			a["chosenRoute"] = chosen;
			return Task.FromResult(chosen);
		}, "PredictRoute"), nodeId: "predict");

		var execute = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			string route = (string)a["chosenRoute"]!;
			var actuals = (Dictionary<string, int>)a["actualMs"]!;
			int t = actuals[route];
			await Task.Delay(t).ConfigureAwait(false);
			return $"answer(graph26): route={route}; actualMs={t}";
		}, "ExecuteRoute"), nodeId: "exec");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			string route = (string)a["chosenRoute"]!;
			var hist = (Dictionary<string, int>)a["histMeanMs"]!;
			var score = (Dictionary<string, double>)a["predictedScoreMs"]!;
			int expectedBaseline = hist[route];
			double predicted = score[route];
			return Task.FromResult($"chosen={route}; baselineMeanMs={expectedBaseline}; predictedScoreMs={predicted:0}");
		}, "MergeMeta"), nodeId: "merge");

		var exec = new GraphExecutor("ch26_predictor_routing", "Predictor informs orchestration routing");
		exec.AddNode(start).AddNode(build).AddNode(predict).AddNode(execute).AddNode(merge);
		exec.SetStartNode("start");
		exec.Connect("start", "build");
		exec.Connect("build", "predict");
		exec.Connect("predict", "exec");
		exec.Connect("exec", "merge");

		return exec;
	}
}




