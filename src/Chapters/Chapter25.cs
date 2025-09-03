namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter25;
using GraphIABook.Graphs.Chapter25;

/// <summary>
/// Capítulo 25 — Orquestração vs. Graph Neural Networks (GNN).
/// Este runner executa: (1) chain baseline (média histórica) para seleção de rota;
/// (2) grafo com preditor tipo GNN (features estruturais) informando a rota;
/// (3) sumários de latência (média/p95/p99) e (4) benchmark A/B.
/// Consulte docs/book/33-capitulo-25.md.
/// </summary>
public sealed class Chapter25 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_BaselineAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_GnnAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
	}

	/// <summary>
	/// Chain: baseline heurístico (média histórica) para seleção de rota.
	/// </summary>
	public async Task RunChain_BaselineAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap25/chain/baseline", async () =>
		{
			var output = await ChainChapter25.RunAsync("Cap25 Baseline (chain)");
			return output;
		});
	}

	/// <summary>
	/// Graph: preditor tipo GNN usando features estruturais para escolher rota.
	/// </summary>
	public async Task RunGraph_GnnAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap25/graph/gnn", async () =>
		{
			var output = await GraphChapter25.RunAsync("Cap25 GNN (graph)");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência do chain baseline.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap25/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter25.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo com preditor GNN-like.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap25/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter25.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Benchmark A/B (latência): chain baseline vs graph com preditor GNN-like.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap25/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter25.RunAsync(s); },
			async s => { _ = await GraphChapter25.RunAsync(s); });
	}
}


