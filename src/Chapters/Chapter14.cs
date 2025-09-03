namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter14;
using GraphIABook.Graphs.Chapter14;

/// <summary>
/// Capítulo 14 — Tendências Futuras da Orquestração em Grafos.
/// Demonstra grafo adaptativo que introduz caminho alternativo (B') ao exceder limite de latência.
/// Consulte docs/book/19-capitulo-14.md.
/// </summary>
public sealed class Chapter14 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_BaselineSummaryAsync();
		await RunGraph_AdaptiveSummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_DynamicRuleTheoryAsync();
	}

	/// <summary>
	/// Constrói sequência determinística de latências conforme a fundamentação do capítulo.
	/// </summary>
	private static IReadOnlyList<int> BuildLatencies(bool increaseTail)
	{
		// Base: 50x 800ms e 50x 1200ms → média 1000ms (não excede estritamente 1000)
		var list = new List<int>(capacity: 100);
		for (int i = 0; i < 50; i++) list.Add(800);
		for (int i = 0; i < 50; i++) list.Add(1200);

		if (increaseTail)
		{
			// Eleva 10 amostras de 800→1400 para média ≈ 1100ms (excede > 1000)
			int changed = 0;
			for (int i = 0; i < list.Count && changed < 10; i++)
			{
				if (list[i] == 800)
				{
					list[i] = 1400;
					changed++;
				}
			}
		}
		return list;
	}

	/// <summary>
	/// Mede a distribuição de latência no pipeline linear (CHAIN) com janela determinística.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var latencies = BuildLatencies(increaseTail: false);
		int index = 0;
		await BenchmarkUtils.MeasureManyAsync("cap14/chain/latency-summary", iterations: latencies.Count, action: async () =>
		{
			int ms = latencies[index++];
			_ = await ChainChapter14.RunOnceAsync(ms);
		});
	}

	/// <summary>
	/// Mede latência do grafo sem caminho alternativo (baseline), mesma sequência determinística.
	/// </summary>
	public async Task RunGraph_BaselineSummaryAsync()
	{
		var latencies = BuildLatencies(increaseTail: false);
		int index = 0;
		await BenchmarkUtils.MeasureManyAsync("cap14/graph/baseline-latency-summary", iterations: latencies.Count, action: async () =>
		{
			int ms = latencies[index++];
			_ = await GraphChapter14.RunAsync(ms, includeAlternate: false);
		});
	}

	/// <summary>
	/// Mede latência do grafo adaptativo com caminho alternativo (hedge/fallback paralelo) B'.
	/// </summary>
	public async Task RunGraph_AdaptiveSummaryAsync()
	{
		var latencies = BuildLatencies(increaseTail: true);
		int index = 0;
		await BenchmarkUtils.MeasureManyAsync("cap14/graph/adaptive-latency-summary", iterations: latencies.Count, action: async () =>
		{
			int ms = latencies[index++];
			// B' com latência limitada a 800ms (ex.: cache/rotina simplificada)
			_ = await GraphChapter14.RunAsync(ms, includeAlternate: true, altLatencyMs: 800);
		});
	}

	/// <summary>
	/// Escreve validação teórica do capítulo (regra determinística de evolução do grafo).
	/// </summary>
	public Task RunBenchmark_DynamicRuleTheoryAsync()
	{
		var baseLat = BuildLatencies(increaseTail: false);
		var newLat = BuildLatencies(increaseTail: true);

		double baseMean = baseLat.Average();
		double newMean = newLat.Average();

		var entries = new Dictionary<string, object>
		{
			["window_size"] = baseLat.Count,
			["threshold_ms_strict_greater"] = 1000,
			["baseline_mean_ms"] = baseMean,
			["new_mean_ms"] = newMean,
			["should_add_B_prime_baseline"] = baseMean > 1000.0,
			["should_add_B_prime_new"] = newMean > 1000.0,
			["expected_vertices_G0"] = new[] { "A", "B" },
			["expected_edges_G0"] = new[] { "A->B" },
			["expected_vertices_G1"] = new[] { "A", "B", "B'" },
			["expected_edges_G1"] = new[] { "A->B", "A->B'" }
		};

		BenchmarkUtils.WriteTheory("cap14/benchmark/dynamic-rule", entries);
		return Task.CompletedTask;
	}
}


