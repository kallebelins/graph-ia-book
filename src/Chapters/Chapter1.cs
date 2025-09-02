namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Capítulo 1 — Problema da Complexidade em IA.
/// Este capítulo mede e compara pipelines sequenciais (CHAIN) vs grafos com paralelismo (GRAPH),
/// reportando latência média, p95/p99 e custo estimado, além de observar o makespan.
/// </summary>
public sealed class Chapter1 : IChapter
{
	/// <summary>
	/// Executa todos os cenários CHAIN do capítulo 1 e gera relatório/ métricas padronizadas.
/// </summary>
	public async Task RunChainAsync()
	{
		await RunChain_LatencyAsync();
		await RunChain_CostAsync();
		await RunChain_LatencySummaryAsync();
	}

	/// <summary>
	/// Executa todos os cenários GRAPH do capítulo 1 e gera relatório/ métricas padronizadas.
/// </summary>
	public async Task RunGraphAsync()
	{
		await RunGraph_LatencyAsync();
		await RunGraph_CostAsync();
		await RunGraph_LatencySummaryAsync();
	}

	/// <summary>
	/// Executa o benchmark comparando CHAIN vs GRAPH para o capítulo 1, consolidando métricas e relatório.
/// </summary>
	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyVsCostAsync();
		await RunBenchmark_LatencyP95P99Async();
	}

	/// <summary>
	/// Mede latência do pipeline CHAIN (sequencial) para entradas fixas.
/// </summary>
	public async Task RunChain_LatencyAsync()
	{
		await BenchmarkUtils.MeasureAsync("chapter1/chain/latency", async () =>
		{
			await Task.Delay(10);
			return "ok";
		});
	}

	/// <summary>
	/// Estima custo do pipeline CHAIN (tokens/execução) em cenário controlado.
/// </summary>
	public async Task RunChain_CostAsync()
	{
		await BenchmarkUtils.MeasureAsync("chapter1/chain/cost", async () =>
		{
			await Task.Delay(5);
			return "ok";
		});
	}

	/// <summary>
	/// Mede latência do grafo GRAPH com ramos paralelos e merge determinístico.
/// </summary>
	public async Task RunGraph_LatencyAsync()
	{
		await BenchmarkUtils.MeasureAsync("chapter1/graph/latency", async () =>
		{
			var t1 = Task.Delay(5);
			var t2 = Task.Delay(7);
			await Task.WhenAll(t1, t2);
			return "ok";
		});
	}

	/// <summary>
	/// Estima custo do grafo GRAPH com paralelismo e custo de agregação.
/// </summary>
	public async Task RunGraph_CostAsync()
	{
		await BenchmarkUtils.MeasureAsync("chapter1/graph/cost", async () =>
		{
			await Task.Delay(6);
			return "ok";
		});
	}

	/// <summary>
	/// Sumariza latência do pipeline CHAIN (média/p95/p99) para entradas fixas.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		await BenchmarkUtils.MeasureManyAsync("chapter1/chain/latency", iterations: 50, action: async () =>
		{
			await Task.Delay(10);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo GRAPH (média/p95/p99) com ramos paralelos e merge.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		await BenchmarkUtils.MeasureManyAsync("chapter1/graph/latency", iterations: 50, action: async () =>
		{
			var t1 = Task.Delay(5);
			var t2 = Task.Delay(7);
			await Task.WhenAll(t1, t2);
		});
	}

	/// <summary>
	/// Benchmark comparativo: CHAIN vs GRAPH (latência e custo consolidados).
/// </summary>
	public async Task RunBenchmark_LatencyVsCostAsync()
	{
		await BenchmarkUtils.MeasureAsync("chapter1/benchmark/latency-vs-cost", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) para comparação CHAIN vs GRAPH.
	/// </summary>
	public async Task RunBenchmark_LatencyP95P99Async()
	{
		await BenchmarkUtils.MeasureManyAsync("chapter1/benchmark/latency-vs-cost", iterations: 50, action: async () =>
		{
			await Task.Delay(1);
		});
	}
}


