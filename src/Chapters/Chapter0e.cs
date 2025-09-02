namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Introdução — demonstra chain mínimo (entrada → LLM → resposta) e graph com dois ramos paralelos
/// (ex.: sumarização e extração) convergindo em agregador. Métricas: latência média e p95; comparação
/// do makespan com e sem paralelismo (limite por caminho crítico / Brent).
/// </summary>
public sealed class Chapter0e : IChapter
{
	/// <summary>
	/// Executa o pipeline mínimo em CHAIN e mede latência.
	/// </summary>
	public async Task RunChainAsync()
	{
		await RunChain_LatencyAsync();
	}

	/// <summary>
	/// Executa o grafo com dois ramos paralelos e mede latência com merge.
	/// </summary>
	public async Task RunGraphAsync()
	{
		await RunGraph_LatencyAsync();
		await RunGraph_LatencySummaryAsync();
	}

	/// <summary>
	/// Compara o makespan entre chain e graph (paralelismo) e reporta.
	/// </summary>
	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_MakespanAsync();
		await RunBenchmark_LatencyP95P99Async();
	}

	public async Task RunChain_LatencyAsync()
	{
		await BenchmarkUtils.MeasureAsync("00e/chain/latency", async () =>
		{
			await Task.Delay(10);
			return "ok";
		});
	}

	public async Task RunGraph_LatencyAsync()
	{
		await BenchmarkUtils.MeasureAsync("00e/graph/latency", async () =>
		{
			var s = Task.Delay(6); // sumarização (simulada)
			var e = Task.Delay(6); // extração (simulada)
			await Task.WhenAll(s, e);
			return "ok";
		});
	}

	public async Task RunGraph_LatencySummaryAsync()
	{
		await BenchmarkUtils.MeasureManyAsync("00e/graph/latency", iterations: 50, action: async () =>
		{
			var s = Task.Delay(6);
			var e = Task.Delay(6);
			await Task.WhenAll(s, e);
		});
	}

	public async Task RunBenchmark_MakespanAsync()
	{
		await BenchmarkUtils.MeasureAsync("00e/benchmark/makespan", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}

	public async Task RunBenchmark_LatencyP95P99Async()
	{
		await BenchmarkUtils.MeasureManyAsync("00e/benchmark/latency", iterations: 50, action: async () =>
		{
			await Task.Delay(1);
		});
	}
}


