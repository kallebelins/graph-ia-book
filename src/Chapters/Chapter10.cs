namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter10;
using GraphIABook.Graphs.Chapter10;

/// <summary>
/// Capítulo 10 — Comparativo diamond: chain sequencial vs grafo paralelo.
/// Consulte docs/book/14-capitulo-10.md.
/// </summary>
public sealed class Chapter10 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_DiamondSequentialAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_DiamondParallelAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteDiamondTheory();
	}

	/// <summary>
	/// Executa o chain no padrão diamante (A->B sequenciais) e mede o tempo.
	/// </summary>
	public async Task RunChain_DiamondSequentialAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap10/chain/diamond-seq", async () =>
		{
			var output = await ChainChapter10.RunAsync("cap10-input");
			return output;
		});
	}

	/// <summary>
	/// Executa o grafo no padrão diamante (A,B paralelos -> merge) e mede o tempo.
	/// </summary>
	public async Task RunGraph_DiamondParallelAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap10/graph/diamond-parallel", async () =>
		{
			var output = await GraphChapter10.RunAsync("cap10-input");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain sequencial (diamante).
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap10/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter10.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo paralelo (diamante).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap10/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter10.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e grafo utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap10/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter10.RunAsync(s); },
			async s => { _ = await GraphChapter10.RunAsync(s); });
	}

	/// <summary>
	/// Escreve a teoria do makespan do diamante: chain = A+B+Merge; graph = max(A,B)+Merge.
	/// </summary>
	public static void WriteDiamondTheory()
	{
		int a = GraphChapter10.BranchADurationMs;
		int b = GraphChapter10.BranchBDurationMs;
		int merge = GraphChapter10.MergeDurationMs;
		int tChain = a + b + merge;
		int tGraph = Math.Max(a, b) + merge;
		double speedup = tGraph == 0 ? 0 : (double)tChain / tGraph;

		BenchmarkUtils.WriteTheory("cap10/theory/diamond-makespan", new Dictionary<string, object>
		{
			["branch_a_ms"] = a,
			["branch_b_ms"] = b,
			["merge_ms"] = merge,
			["t_chain_sum_ms"] = tChain,
			["t_graph_max_ms"] = tGraph,
			["speedup_chain_over_graph"] = speedup.ToString("F2")
		});
	}
}


