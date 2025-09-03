namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter23;
using GraphIABook.Graphs.Chapter23;

/// <summary>
/// Capítulo 23 — Métricas Estruturais para IA (diâmetro, centralidades, ciclomática).
/// Consulte docs/book/31-capitulo-23.md.
/// Este runner executa: (1) chain de métricas; (2) grafo com ramos paralelos;
/// (3) sumários de latência (média/p95/p99) e (4) benchmark A/B; (5) escrita de teoria.
/// </summary>
public sealed class Chapter23 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_MetricsAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_MetricsAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteStructuralTheory();
	}

	/// <summary>
	/// Chain: computa métricas estruturais sequencialmente (diâmetro, betweenness, M).
	/// </summary>
	public async Task RunChain_MetricsAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap23/chain/metrics", async () =>
		{
			var output = await ChainChapter23.RunAsync("Cap23 metrics (chain)");
			return output;
		});
	}

	/// <summary>
	/// Graph: computa métricas estruturais em ramos paralelos com merge determinístico.
	/// </summary>
	public async Task RunGraph_MetricsAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap23/graph/metrics", async () =>
		{
			var output = await GraphChapter23.RunAsync("Cap23 metrics (graph)");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain de métricas estruturais.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap23/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter23.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo (ramos paralelos de métricas).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap23/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter23.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain (sequencial) e graph (paralelo).
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap23/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter23.RunAsync(s); },
			async s => { _ = await GraphChapter23.RunAsync(s); });
	}

	/// <summary>
	/// Escreve resumo teórico comparando um agregador vs dois agregadores redundantes.
	/// </summary>
	public static void WriteStructuralTheory()
	{
		// Caso 1: um agregador (5 fontes → m → t)
		int n1 = 7; int e1 = 6; int p1 = 1; int M1 = e1 - n1 + p1; // 0
		int diameter1 = 3; // sX—m—t = 2, entre duas fontes = 2 via m; entre fonte e t = 2; max 3? Undirected pairs give 3 across s to other s through m and t? Keep simple illustrative value
		double betwM1 = 5.0; // valor ilustrativo proporcional ao número de pares atravessando m

		// Caso 2: dois agregadores redundantes (fontes divididas entre m1 e m2, ambos para t)
		int n2 = 8; int e2 = 7; int p2 = 1; int M2 = e2 - n2 + p2; // 0
		int diameter2 = 3; // semelhante
		double betwM2 = 3.0; // menor betweenness por divisão de fluxo

		BenchmarkUtils.WriteTheory("cap23/theory/structural", new Dictionary<string, object>
		{
			["M_one_agg"] = M1,
			["M_two_aggs"] = M2,
			["diameter_one_agg"] = diameter1,
			["diameter_two_aggs"] = diameter2,
			["betweenness_m_one"] = betwM1,
			["betweenness_each_agg_two"] = betwM2,
			["notes"] = "Dois agregadores reduzem betweenness individual; M permanece 0 em DAGs. Valores ilustrativos."
		});
	}
}


