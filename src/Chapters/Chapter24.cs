namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter24;
using GraphIABook.Graphs.Chapter24;

/// <summary>
/// Capítulo 24 — Topologias e Anti-padrões. Consulte docs/book/32-capitulo-24.md.
/// Compara chain (fan-out sequencial) vs grafo (fan-out paralelo) com agregação determinística.
/// </summary>
public sealed class Chapter24 : IChapter
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
		WriteTopologiesMakespanTheory();
	}

	/// <summary>
	/// Executa o chain com fan-out lógico (5 ramos) em sequência e mede tempo de execução.
	/// </summary>
	public async Task RunChain_FanoutSequentialAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap24/chain/fanout-seq", async () =>
		{
			var output = await ChainChapter24.RunAsync("cap24-input");
			return output;
		});
	}

	/// <summary>
	/// Executa o grafo com fan-out (5 ramos paralelos) e merge determinístico; mede tempo de execução.
	/// </summary>
	public async Task RunGraph_FanoutParallelAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap24/graph/fanout-parallel", async () =>
		{
			var output = await GraphChapter24.RunAsync("cap24-input");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain sequencial com cinco ramos.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap24/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter24.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo paralelo com cinco ramos.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap24/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter24.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e grafo utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap24/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter24.RunAsync(s); },
			async s => { _ = await GraphChapter24.RunAsync(s); });
	}

	/// <summary>
	/// Escreve a teoria de makespan: chain = preprocess + sum(ramos) + aggregator;
	/// graph = preprocess + max(ramos) + aggregator; inclui speedup.
	/// </summary>
	public static void WriteTopologiesMakespanTheory()
	{
		int preprocess = GraphChapter24.PreprocessMs;
		int aggregator = GraphChapter24.AggregatorMs;
		int[] branches = GraphChapter24.BranchDurationsMs;
		int chainSum = preprocess + branches.Sum() + aggregator;
		int graphCritical = preprocess + branches.Max() + aggregator;
		double speedup = graphCritical == 0 ? 0 : (double)chainSum / graphCritical;

		BenchmarkUtils.WriteTheory("cap24/theory/topologies-makespan", new Dictionary<string, object>
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


