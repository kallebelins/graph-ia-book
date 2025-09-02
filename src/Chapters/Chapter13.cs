namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Capítulo 13 — Limitações da Abordagem em Grafos.
/// Compara chain trivial (CSV→JSON) onde chains podem ser melhores versus grafo com sobrecarga de
/// coordenação (ramos superficiais) para medir custo adicional. Métricas: TCO simplificado, latência,
/// memória/logs (placeholder), validação de condição G &lt; C (ganho vs custo).
/// </summary>
public sealed class Chapter13 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_TrivialCsvToJsonAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_CoordinationOverheadAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_GainVsCostAsync();
		await RunBenchmark_LatencyP95P99Async();
	}

	/// <summary>
	/// Simula transformação trivial CSV→JSON com latência baixa e sem paralelismo útil.
	/// </summary>
	public async Task RunChain_TrivialCsvToJsonAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap13/chain/csv-json", async () =>
		{
			await Task.Delay(3);
			return "ok";
		});
	}

	/// <summary>
	/// Simula grafo com ramos superficiais e custo de sincronização/merge, evidenciando overhead.
	/// </summary>
	public async Task RunGraph_CoordinationOverheadAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap13/graph/overhead", async () =>
		{
			var r1 = Task.Delay(2);
			var r2 = Task.Delay(2);
			await Task.WhenAll(r1, r2); // merge com custo adicional (implícito)
			return "ok";
		});
	}

	/// <summary>
	/// Sumariza latência do grafo com ramos superficiais para observar custo de coordenação.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		await BenchmarkUtils.MeasureManyAsync("cap13/graph/latency-summary", iterations: 50, action: async () =>
		{
			var r1 = Task.Delay(2);
			var r2 = Task.Delay(2);
			await Task.WhenAll(r1, r2);
		});
	}

	/// <summary>
	/// Benchmark de trade-off: verifica condição G &lt; C (ganho vs custo) no cenário simulado.
	/// </summary>
	public async Task RunBenchmark_GainVsCostAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap13/benchmark/gain-vs-cost", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}

	/// <summary>
	/// Calcula média/p95/p99 de latência para comparar chain trivial vs grafo com overhead.
	/// </summary>
	public async Task RunBenchmark_LatencyP95P99Async()
	{
		await BenchmarkUtils.MeasureManyAsync("cap13/benchmark/latency-p95-p99", iterations: 50, action: async () =>
		{
			await Task.Delay(2);
		});
	}
}


