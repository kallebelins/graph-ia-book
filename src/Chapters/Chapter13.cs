namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter13;
using GraphIABook.Graphs.Chapter13;

/// <summary>
/// Capítulo 13 — Limitações da Abordagem em Grafos.
/// Compara chain trivial (CSV→JSON) onde chains podem ser melhores versus grafo com sobrecarga de
/// coordenação (ramos superficiais) para medir custo adicional. Métricas: TCO simplificado, latência,
/// memória/logs (snapshot simples), validação da condição G &lt; C (ganho vs custo) com varredura paramétrica.
/// </summary>
public sealed class Chapter13 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_TrivialCsvToJsonAsync();
		await RunChain_LatencySummaryAsync();
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
	/// CSV fixo mínimo para o experimento do capítulo 13.
	/// </summary>
	private static string GetSampleCsv()
	{
		return "a,b\n1,2\n3,4\n5,6";
	}

	/// <summary>
	/// Executa transformação trivial CSV→JSON (CHAIN) e mede uma execução.
	/// </summary>
	public async Task RunChain_TrivialCsvToJsonAsync()
	{
		var csv = GetSampleCsv();
		await BenchmarkUtils.MeasureAsync("cap13/chain/csv-json", async () =>
		{
			var json = await ChainChapter13.RunAsync(csv);
			return json.Length > 0 ? "ok" : "empty";
		});
	}

	/// <summary>
	/// Executa o grafo com ramos superficiais e merge (GRAPH), destacando custo de coordenação.
	/// </summary>
	public async Task RunGraph_CoordinationOverheadAsync()
	{
		var csv = GetSampleCsv();
		await BenchmarkUtils.MeasureAsync("cap13/graph/overhead", async () =>
		{
			var json = await GraphChapter13.RunAsync(csv);
			return json.Length > 0 ? "ok" : "empty";
		});
	}

	/// <summary>
	/// Sumariza latência do grafo (GRAPH) para observar custo de coordenação.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var csv = GetSampleCsv();
		await BenchmarkUtils.MeasureManyAsync("cap13/graph/latency-summary", iterations: 50, action: async () =>
		{
			_ = await GraphChapter13.RunAsync(csv);
		});
	}

	/// <summary>
	/// Sumariza latência do chain trivial (CHAIN) para comparação.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var csv = GetSampleCsv();
		await BenchmarkUtils.MeasureManyAsync("cap13/chain/latency-summary", iterations: 50, action: async () =>
		{
			_ = await ChainChapter13.RunAsync(csv);
		});
	}

	/// <summary>
	/// Benchmark de trade-off: verifica condição G &lt; C (ganho vs custo) no cenário simulado com varredura de overhead.
	/// Também registra snapshot simples de memória antes/depois como proxy.
	/// </summary>
	public async Task RunBenchmark_GainVsCostAsync()
	{
		// Custos (proxies) baseados nos tempos simulados nos exemplos CHAIN/GRAPH
		int chainCost = ChainChapter13.PreprocessMs + ChainChapter13.ParseMs + ChainChapter13.SerializeMs;
		int graphBase = Math.Max(GraphChapter13.HeaderParseMs, GraphChapter13.BodyParseMs) + GraphChapter13.MergeMs;

		// Varre overhead adicional de coordenação (0..6 ms) e verifica G < C
		var entries = new Dictionary<string, object>();
		var results = new List<Dictionary<string, object>>();
		for (int overhead = 0; overhead <= 6; overhead++)
		{
			int chainLatency = chainCost; // sem paralelismo útil
			int graphLatency = Math.Max(GraphChapter13.HeaderParseMs, GraphChapter13.BodyParseMs) + GraphChapter13.MergeMs + overhead;
			int gain = chainLatency - (graphLatency - GraphChapter13.MergeMs); // ganho bruto por paralelismo raso
			int cost = GraphChapter13.MergeMs + overhead; // custo de coordenação
			bool condition = gain < cost; // condição G < C
			results.Add(new Dictionary<string, object>
			{
				["overhead_ms"] = overhead,
				["chain_latency_ms"] = chainLatency,
				["graph_latency_ms"] = graphLatency,
				["gain_ms"] = gain,
				["cost_ms"] = cost,
				["G_less_than_C"] = condition
			});
		}

		long memBefore = GC.GetTotalMemory(forceFullCollection: false);
		_ = await ChainChapter13.RunAsync(GetSampleCsv());
		_ = await GraphChapter13.RunAsync(GetSampleCsv());
		long memAfter = GC.GetTotalMemory(forceFullCollection: false);

		entries["chain_cost_ms"] = chainCost;
		entries["graph_base_ms"] = graphBase;
		entries["param_sweep"] = results;
		entries["mem_before_bytes"] = memBefore;
		entries["mem_after_bytes"] = memAfter;
		entries["mem_delta_bytes"] = memAfter - memBefore;

		BenchmarkUtils.WriteTheory("cap13/benchmark/gain-vs-cost", entries);
	}

	/// <summary>
	/// Calcula média/p95/p99 de latência para ambos: chain trivial vs grafo com overhead.
	/// </summary>
	public async Task RunBenchmark_LatencyP95P99Async()
	{
		var csv = GetSampleCsv();
		await BenchmarkUtils.MeasureManyAsync("cap13/benchmark/chain-latency", iterations: 50, action: async () =>
		{
			_ = await ChainChapter13.RunAsync(csv);
		});
		await BenchmarkUtils.MeasureManyAsync("cap13/benchmark/graph-latency", iterations: 50, action: async () =>
		{
			_ = await GraphChapter13.RunAsync(csv);
		});
	}
}


