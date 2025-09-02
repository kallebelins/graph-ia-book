namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter6;
using GraphIABook.Graphs.Chapter6;

/// <summary>
/// Capítulo 6 — A definir. Consulte docs/book/09-capitulo-6.md.
/// </summary>
public sealed class Chapter6 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_FanoutSequentialAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_FanoutParallelAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteCriticalPathTheory();
	}

	/// <summary>
	/// Executa o chain com fan-out lógico (5 ramos) em sequência e mede tempo de execução.
	/// </summary>
	public async Task RunChain_FanoutSequentialAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap6/chain/fanout-seq", async () =>
		{
			var output = await ChainChapter6.RunAsync("cap6-input");
			return output;
		});
	}

	/// <summary>
	/// Executa o grafo com fan-out (5 ramos paralelos) e merge determinístico; mede tempo de execução.
	/// </summary>
	public async Task RunGraph_FanoutParallelAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap6/graph/fanout-parallel", async () =>
		{
			var output = await GraphChapter6.RunAsync("cap6-input");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain sequencial com cinco ramos.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap6/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter6.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo paralelo com cinco ramos.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap6/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter6.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e grafo utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap6/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter6.RunAsync(s); },
			async s => { _ = await GraphChapter6.RunAsync(s); });
	}

	/// <summary>
	/// Escreve teoria do caminho crítico vs soma sequencial para o fan-out de 5 ramos.
	/// </summary>
	public static void WriteCriticalPathTheory()
	{
		int preprocess = GraphChapter6.PreprocessMs;
		int aggregator = GraphChapter6.AggregatorMs;
		int[] branches = GraphChapter6.BranchDurationsMs;
		int chainSum = preprocess + branches.Sum() + aggregator;
		int graphCritical = preprocess + branches.Max() + aggregator;
		double speedup = graphCritical == 0 ? 0 : (double)chainSum / graphCritical;

		BenchmarkUtils.WriteTheory("cap6/theory/critical-path", new Dictionary<string, object>
		{
			["preprocess_ms"] = preprocess,
			["branch_durations_ms"] = string.Join(",", branches),
			["aggregator_ms"] = aggregator,
			["t_chain_sum_ms"] = chainSum,
			["t_graph_critical_ms"] = graphCritical,
			["speedup_chain_over_graph"] = speedup.ToString("F2")
		});
	}
}


