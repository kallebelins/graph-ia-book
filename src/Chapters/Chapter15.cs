namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter15;
using GraphIABook.Graphs.Chapter15;

/// <summary>
/// Capítulo 15 — Síntese de múltiplas análises (fan-out/fan-in de 3 ramos).
/// Compara chain sequencial (A → B → C → Merge) com grafo paralelo {A,B,C} → Merge.
/// Consulte docs/book/35-capitulo-15.md.
/// </summary>
public sealed class Chapter15 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_MakespanTheoryAsync();
	}

	private static IReadOnlyList<(int a, int b, int c)> BuildDurations()
	{
		// Deterministic set of triplets to compute distribution (média/p95/p99)
		// Alternates the longest branch across A/B/C to exercise different critical paths in the graph.
		var list = new List<(int, int, int)>
		{
			(120, 80, 100), (80, 120, 100), (100, 80, 120),
			(140, 90, 110), (90, 140, 110), (110, 90, 140),
			(100, 100, 100), (130, 85, 95), (85, 130, 95), (95, 85, 130)
		};
		// Repeat to reach a reasonable iteration count for percentiles
		return Enumerable.Repeat(list, 5).SelectMany(x => x).ToList(); // 50 iterations
	}

	/// <summary>
	/// Mede distribuição de latência para pipeline CHAIN (sequencial A→B→C→Merge).
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var runs = BuildDurations();
		int index = 0;
		await BenchmarkUtils.MeasureManyAsync("cap15/chain/latency-summary", iterations: runs.Count, action: async () =>
		{
			var (a, b, c) = runs[index++];
			_ = await ChainChapter15.RunAsync(a, b, c);
		});
	}

	/// <summary>
	/// Mede distribuição de latência para GRAFO (ramos A/B/C em paralelo, merge determinístico).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var runs = BuildDurations();
		int index = 0;
		await BenchmarkUtils.MeasureManyAsync("cap15/graph/latency-summary", iterations: runs.Count, action: async () =>
		{
			var (a, b, c) = runs[index++];
			_ = await GraphChapter15.RunAsync(a, b, c);
		});
	}

	/// <summary>
	/// Escreve validação teórica: makespan do chain (soma) vs grafo (máximo + α_merge).
	/// Também fornece proxy simples de custo (tokens) proporcional aos tempos.
	/// </summary>
	public Task RunBenchmark_MakespanTheoryAsync()
	{
		const int mergeMs = 10; // custo determinístico de merge
		var samples = BuildDurations();
		int chainSum = 0;
		int graphSum = 0;
		foreach (var (a, b, c) in samples)
		{
			int chain = a + b + c + mergeMs;
			int graph = Math.Max(a, Math.Max(b, c)) + mergeMs;
			chainSum += chain;
			graphSum += graph;
		}
		double chainMean = (double)chainSum / samples.Count;
		double graphMean = (double)graphSum / samples.Count;
		double speedup = chainMean / graphMean;

		var entries = new Dictionary<string, object>
		{
			["samples"] = samples.Count,
			["merge_ms"] = mergeMs,
			["mean_chain_ms"] = Math.Round(chainMean, 2),
			["mean_graph_ms"] = Math.Round(graphMean, 2),
			["expected_speedup_chain_over_graph"] = Math.Round(speedup, 3),
			// Proxies de custo (tokens) proporcionais aos tempos totais
			["cost_proxy_chain"] = Math.Round(chainMean, 2),
			["cost_proxy_graph"] = Math.Round(graphMean, 2)
		};

		BenchmarkUtils.WriteTheory("cap15/theory/makespan-3branch", entries);
		return Task.CompletedTask;
	}
}


