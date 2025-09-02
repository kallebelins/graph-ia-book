namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter1;
using GraphIABook.Graphs.Chapter1;

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
		WriteMakespanTheory();
	}

	/// <summary>
	/// Mede latência do pipeline CHAIN (sequencial) para entradas fixas.
/// </summary>
	public async Task RunChain_LatencyAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 10);
		await AbBenchmarkHarness.WarmupAsync(inputs.Take(2),
			async s => { _ = await ChainChapter1.RunAsync(s); },
			async s => { _ = await GraphChapter1.RunAsync(s); });

		await BenchmarkUtils.MeasureManyAsync("chapter1/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Environment.TickCount % inputs.Count;
			_ = await ChainChapter1.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Estima custo do pipeline CHAIN (tokens/execução) em cenário controlado.
/// </summary>
	public async Task RunChain_CostAsync()
	{
		// Simplified cost proxy: sum of stage delays as a stand-in for token cost.
		await BenchmarkUtils.MeasureManyAsync("chapter1/chain/cost", iterations: 20, action: async () =>
		{
			await Task.Delay(ChainChapter1.PreprocessMs + ChainChapter1.RetrieveMs + ChainChapter1.ReasonMs + ChainChapter1.AnswerMs);
		});
	}

	/// <summary>
	/// Mede latência do grafo GRAPH com ramos paralelos e merge determinístico.
/// </summary>
	public async Task RunGraph_LatencyAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 10);
		await BenchmarkUtils.MeasureManyAsync("chapter1/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter1.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Estima custo do grafo GRAPH com paralelismo e custo de agregação.
/// </summary>
	public async Task RunGraph_CostAsync()
	{
		// Simplified cost proxy: max of parallel branches + merge + preprocess overhead
		await BenchmarkUtils.MeasureManyAsync("chapter1/graph/cost", iterations: 20, action: async () =>
		{
			var parallel = Math.Max(GraphChapter1.RetrieveMs, GraphChapter1.VerifyMs);
			await Task.Delay(GraphChapter1.PreprocessMs + parallel + GraphChapter1.ReasonMs + GraphChapter1.MergeMs);
		});
	}

	/// <summary>
	/// Sumariza latência do pipeline CHAIN (média/p95/p99) para entradas fixas.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("chapter1/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter1.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo GRAPH (média/p95/p99) com ramos paralelos e merge.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("chapter1/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter1.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Benchmark comparativo: CHAIN vs GRAPH (latência e custo consolidados).
/// </summary>
	public async Task RunBenchmark_LatencyVsCostAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"chapter1/benchmark/latency-vs-cost",
			inputs,
			async s => { _ = await ChainChapter1.RunAsync(s); },
			async s => { _ = await GraphChapter1.RunAsync(s); });
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) para comparação CHAIN vs GRAPH.
	/// </summary>
	public async Task RunBenchmark_LatencyP95P99Async()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("chapter1/benchmark/latency-vs-cost", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter1.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Writes a theory file estimating makespan: chain=sum vs graph=max(parallel branches)+overheads.
	/// Also emits the topological order of the graph for verification.
	/// </summary>
	public static void WriteMakespanTheory()
	{
		var graph = GraphChapter1.CreateExecutor();
		var (isAcyclic, topo) = GraphValidationUtils.Analyze(graph);
		var chainSum = ChainChapter1.PreprocessMs + ChainChapter1.RetrieveMs + ChainChapter1.ReasonMs + ChainChapter1.AnswerMs;
		var graphParallel = Math.Max(GraphChapter1.RetrieveMs, GraphChapter1.VerifyMs);
		var graphSum = GraphChapter1.PreprocessMs + graphParallel + GraphChapter1.ReasonMs + GraphChapter1.MergeMs;
		BenchmarkUtils.WriteTheory("chapter1/theory/makespan", new Dictionary<string, object>
		{
			["isAcyclic"] = isAcyclic,
			["topologicalOrder"] = topo is null ? "" : string.Join(" -> ", topo),
			["chain_ms_sum"] = chainSum,
			["graph_ms_sum"] = graphSum,
			["graph_parallel_branch_ms"] = graphParallel,
			["brent_bound"] = graphSum // here it equals critical path due to static durations
		});
	}
}


