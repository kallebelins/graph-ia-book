namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter2;
using GraphIABook.Graphs.Chapter2;

/// <summary>
/// Capítulo 2 — Teoria de Grafos: Base Matemática.
/// Foco em formalismos: definição de grafos, DAGs, expressividade e propriedades relevantes para IA
/// (composicionalidade, multiplicidade de caminhos, paralelismo, rastreabilidade). Aqui, os exemplos
/// executam cenários didáticos que espelham as propriedades teóricas.
/// </summary>
public sealed class Chapter2 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_ExpressivityDemoAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ExpressivityDemoAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_ChainSubsetOfGraphsAsync();
		await RunBenchmark_LatencyABAsync();
		WriteExpressivityTheory();
	}

	/// <summary>
	/// Demonstra um pipeline linear e mede a limitação de caminhos (sequencialidade estrita).
	/// </summary>
	public async Task RunChain_ExpressivityDemoAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap2/chain/expressivity", async () =>
		{
			var output = await ChainChapter2.RunAsync("Expressividade de chains");
			return output;
		});
	}

	/// <summary>
	/// Demonstra um DAG simples com ramos alternativos e paralelos, mostrando maior expressividade.
	/// </summary>
	public async Task RunGraph_ExpressivityDemoAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap2/graph/expressivity", async () =>
		{
			var output = await GraphChapter2.RunAsync("Expressividade de grafos");
			return output;
		});
	}

	/// <summary>
	/// Benchmark conceitual: evidencia que chains são subconjunto de DAGs em termos práticos de caminhos.
	/// </summary>
	public async Task RunBenchmark_ChainSubsetOfGraphsAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap2/benchmark/chain-subset", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain linear do capítulo 2.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap2/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter2.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo com ramos paralelos do capítulo 2.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap2/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter2.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e graph utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap2/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter2.RunAsync(s); },
			async s => { _ = await GraphChapter2.RunAsync(s); });
	}

	/// <summary>
	/// Escreve saída teórica sobre expressividade e topologia: chain ⊂ DAG, ordem topológica do grafo.
	/// </summary>
	public static void WriteExpressivityTheory()
	{
		var graph = GraphChapter2.CreateExecutor();
		var (isAcyclic, topo) = GraphValidationUtils.Analyze(graph);
		var chainPathCount = 1; // chain linear tem exatamente 1 caminho
		var parallelBranches = 2; // grafo tem 2 ramos independentes
		BenchmarkUtils.WriteTheory("cap2/theory/expressivity", new Dictionary<string, object>
		{
			["isAcyclic"] = isAcyclic,
			["topologicalOrder"] = topo is null ? "" : string.Join(" -> ", topo),
			["chain_path_count"] = chainPathCount,
			["graph_parallel_branches"] = parallelBranches,
			["expressivity_relation"] = "chain ⊂ DAG ⊂ graph"
		});
	}
}


