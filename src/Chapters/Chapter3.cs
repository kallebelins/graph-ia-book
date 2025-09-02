namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter3;
using GraphIABook.Graphs.Chapter3;

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
		await RunChain_LatencyP95P99Async();
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
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap3/chain/conditional", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter3.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do pipeline CHAIN com decisão condicional externa (média/p95/p99).
	/// </summary>
	public async Task RunChain_LatencyP95P99Async()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap3/chain/latency-p95-p99", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter3.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Roteamento condicional nativo em grafo (dois caminhos) seguido de merge determinístico.
	/// </summary>
	public async Task RunGraph_ConditionalRoutingAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap3/graph/routing", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter3.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa várias iterações do fluxo GRAPH para calcular média, p95 e p99 de latência.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap3/graph/latency-summary", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter3.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Valida corretude de merge determinístico (política definida) no cenário comparado.
	/// </summary>
	public async Task RunBenchmark_MergeCorrectnessAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap3/benchmark/merge-correctness", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			var c = await ChainChapter3.RunAsync(inputs[idx]);
			var g = await GraphChapter3.RunAsync(inputs[idx]);
			if (!string.Equals(c, g, StringComparison.Ordinal))
			{
				throw new InvalidOperationException($"Merge mismatch: chain='{c}' graph='{g}'");
			}
		});
	}

	/// <summary>
	/// Benchmark: sumariza latência (média/p95/p99) para comparar com versão chain.
	/// </summary>
	public async Task RunBenchmark_LatencyP95P99Async()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap3/benchmark/latency-p95-p99",
			inputs,
			async s => { _ = await ChainChapter3.RunAsync(s); },
			async s => { _ = await GraphChapter3.RunAsync(s); });
	}
}


