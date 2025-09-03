namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter17;
using GraphIABook.Graphs.Chapter17;

/// <summary>
/// Capítulo 17 — Execução por Ordem Topológica e Caminho Crítico.
/// Consulte docs/book/23-capitulo-17.md para a fundamentação teórica.
/// Este capítulo compara chain (serial) vs grafo (paralelo com dependências explícitas)
/// e escreve um resumo teórico do makespan pelo caminho crítico.
/// </summary>
public sealed class Chapter17 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_TopologicalSchedulingAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_TopologicalSchedulingAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteSchedulingTheory();
	}

	/// <summary>
	/// Chain: executa o DAG em ordem topológica, porém serialmente (sem paralelismo).
	/// </summary>
	public async Task RunChain_TopologicalSchedulingAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap17/chain/topological", async () =>
		{
			var output = await ChainChapter17.RunAsync("Cap17 topological (chain)");
			return output;
		});
	}

	/// <summary>
	/// Graph: executa o DAG com dependências explícitas e paralelismo onde possível.
	/// </summary>
	public async Task RunGraph_TopologicalSchedulingAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap17/graph/topological", async () =>
		{
			var output = await GraphChapter17.RunAsync("Cap17 topological (graph)");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain (execução serial do DAG).
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap17/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter17.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo (execução paralela com dependências).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap17/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter17.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e graph utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap17/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter17.RunAsync(s); },
			async s => { _ = await GraphChapter17.RunAsync(s); });
	}

	/// <summary>
	/// Escreve validação/teoria: comparação T_chain (soma) vs T_graph (caminho crítico).
	/// </summary>
	public static void WriteSchedulingTheory()
	{
		int tA = GraphChapter17.DurationA;
		int tB = GraphChapter17.DurationB;
		int tC = GraphChapter17.DurationC;
		int tD = GraphChapter17.DurationD;
		int tE = GraphChapter17.DurationE;
		int tMerge = GraphChapter17.DurationMerge;

		int tChain = tA + tB + tC + tD + tE + tMerge;

		// Critical path on the DAG: B -> D -> E -> merge
		int pathBDE = tB + tD + tE + tMerge;
		int pathBCE = tB + tC + tE + tMerge;
		int pathACE = tA + tC + tE + tMerge;
		int tGraph = Math.Max(pathBDE, Math.Max(pathBCE, pathACE));

		BenchmarkUtils.WriteTheory("cap17/theory/scheduling-critical-path", new Dictionary<string, object>
		{
			["tA_ms"] = tA,
			["tB_ms"] = tB,
			["tC_ms"] = tC,
			["tD_ms"] = tD,
			["tE_ms"] = tE,
			["tMerge_ms"] = tMerge,
			["t_chain_sum_ms"] = tChain,
			["t_graph_critical_ms"] = tGraph,
			["paths_ms"] = new Dictionary<string, int>
			{
				["B->D->E->merge"] = pathBDE,
				["B->C->E->merge"] = pathBCE,
				["A->C->E->merge"] = pathACE
			},
			["relation"] = "T_chain = sum; T_graph = max over paths (critical path)"
		});
	}
}


