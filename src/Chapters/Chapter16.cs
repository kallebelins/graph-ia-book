namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter16;
using GraphIABook.Graphs.Chapter16;

/// <summary>
/// Capítulo 16 — A definir. Consulte docs/book/22-capitulo-16.md.
/// </summary>
public sealed class Chapter16 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_ExpressivityAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ExpressivityAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteExpressivityProofsTheory();
	}

	/// <summary>
	/// Chain: forced serialization of k independent modules → demonstrates T_chain = k*t + α.
	/// </summary>
	public async Task RunChain_ExpressivityAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap16/chain/expressivity", async () =>
		{
			var output = await ChainChapter16.RunAsync("Cap16 expressividade (chain)", k: 6);
			return output;
		});
	}

	/// <summary>
	/// Graph: k parallel modules with deterministic merge → demonstrates T_DAG ≈ t + α.
	/// </summary>
	public async Task RunGraph_ExpressivityAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap16/graph/expressivity", async () =>
		{
			var output = await GraphChapter16.RunAsync("Cap16 expressividade (graph)", k: 6);
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain serializado do capítulo 16.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap16/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter16.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo paralelo do capítulo 16.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap16/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter16.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e graph utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap16/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter16.RunAsync(s); },
			async s => { _ = await GraphChapter16.RunAsync(s); });
	}

	/// <summary>
	/// Escreve validação/teoria: comparação T_chain vs T_DAG e redução O(log k).
	/// </summary>
	public static void WriteExpressivityProofsTheory()
	{
		int k = 6;
		int t = GraphChapter16.ModuleDurationMs;
		int alpha = GraphChapter16.MergeDurationMs;
		int tChain = k * t + alpha;
		int tDag = t + alpha;
		int levels = (int)Math.Ceiling(Math.Log2(Math.Max(2, k)));
		int tReduce = levels * t + alpha;

		BenchmarkUtils.WriteTheory("cap16/theory/expressivity-proofs", new Dictionary<string, object>
		{
			["k_modules"] = k,
			["t_module_ms"] = t,
			["alpha_merge_ms"] = alpha,
			["t_chain_sum_ms"] = tChain,
			["t_dag_parallel_ms"] = tDag,
			["t_reduce_tree_ms"] = tReduce,
			["levels_log2_k"] = levels,
			["relation"] = "Chain: k*t+α; DAG: t+α; Reduce: ceil(log2 k)*t+α"
		});
	}
}


