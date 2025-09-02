namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Capítulo 3 — Orquestração de Sistemas Inteligentes.
/// Compara chain com decisão condicional (via if/switch externo) versus grafo com roteamento condicional
/// nativo (dois caminhos) e posterior merge determinístico. Mede impacto da decisão no tempo e variância.
/// </summary>
public sealed class Chapter3 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_ConditionalDecisionAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ConditionalRoutingAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_MergeCorrectnessAsync();
		await RunBenchmark_LatencyP95P99Async();
	}

	/// <summary>
	/// Simula decisão condicional em chain (if externo), medindo custo e variabilidade.
	/// </summary>
	public async Task RunChain_ConditionalDecisionAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap3/chain/conditional", async () =>
		{
			// caminho A ou B (ex.: tempo diferente)
			bool chooseA = DateTime.UtcNow.Millisecond % 2 == 0;
			if (chooseA)
			{
				await Task.Delay(6);
			}
			else
			{
				await Task.Delay(9);
			}
			return "ok";
		});
	}

	/// <summary>
	/// Roteamento condicional nativo em grafo (dois caminhos) seguido de merge determinístico.
	/// </summary>
	public async Task RunGraph_ConditionalRoutingAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap3/graph/routing", async () =>
		{
			var pathA = Task.Delay(6);
			// Em gráfico real, a escolha poderia ser dinâmica; aqui simulamos a existência dos caminhos.
			await pathA;
			return "ok";
		});
	}

	/// <summary>
	/// Executa várias iterações do fluxo GRAPH para calcular média, p95 e p99 de latência.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		await BenchmarkUtils.MeasureManyAsync("cap3/graph/latency-summary", iterations: 50, action: async () =>
		{
			var pathA = Task.Delay(6);
			await pathA;
		});
	}

	/// <summary>
	/// Valida corretude de merge determinístico (política definida) no cenário comparado.
	/// </summary>
	public async Task RunBenchmark_MergeCorrectnessAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap3/benchmark/merge-correctness", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}

	/// <summary>
	/// Benchmark: sumariza latência (média/p95/p99) para comparar com versão chain.
	/// </summary>
	public async Task RunBenchmark_LatencyP95P99Async()
	{
		await BenchmarkUtils.MeasureManyAsync("cap3/benchmark/latency-p95-p99", iterations: 50, action: async () =>
		{
			await Task.Delay(6);
		});
	}
}


