namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter4;
using GraphIABook.Graphs.Chapter4;

/// <summary>
/// Capítulo 4 — Explosão de Estados e Modularidade.
/// Demonstra chain com manuseio externo de estados (acoplamento) versus grafo com convergência
/// modular em um nó compartilhado de tratamento. Mede média/p95/p99 e registra teoria (k^n vs convergência).
/// Consulte docs/book/06-capitulo-4.md.
/// </summary>
public sealed class Chapter4 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_StateExplosionAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ModularityConvergenceAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteTheory_StateCounts();
	}

	/// <summary>
	/// Executa o chain de 5 estágios com decisões/handling externos (explosão de estados conceitual).
	/// </summary>
	public async Task RunChain_StateExplosionAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap4/chain/state-explosion", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter4.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do chain (média/p95/p99).
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap4/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter4.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa o grafo com convergência em nó compartilhado de tratamento.
	/// </summary>
	public async Task RunGraph_ModularityConvergenceAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await BenchmarkUtils.MeasureManyAsync("cap4/graph/modularity-convergence", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter4.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo (média/p95/p99).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap4/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter4.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B de latência entre chain e graph.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap4/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter4.RunAsync(s); },
			async s => { _ = await GraphChapter4.RunAsync(s); });
	}

	/// <summary>
	/// Registra uma saída teórica simples: estados em chain k^n (k=3,n=5) e anotação de convergência em grafo.
	/// </summary>
	public static void WriteTheory_StateCounts()
	{
		int k = 3, n = 5;
		long chainStates = (long)Math.Pow(k, n);
		BenchmarkUtils.WriteTheory("cap4/theory/state-counts", new Dictionary<string, object>
		{
			["k_branching_per_stage"] = k,
			["n_stages"] = n,
			["chain_states_upper_bound"] = chainStates,
			["graph_convergence_note"] = "shared handler reduces effective states by reuse/convergence"
		});
	}
}


