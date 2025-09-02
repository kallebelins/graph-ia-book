namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter5;
using GraphIABook.Graphs.Chapter5;

/// <summary>
/// Capítulo 5 — Explicabilidade e Auditoria.
/// Demonstra um chain com decisão imperativa (sem trilha estruturada) versus grafo com nó condicional
/// e execução em streaming, emitindo uma trilha de auditoria explícita (eventos por nó/condição).
/// Consulte docs/book/08-capitulo-5.md.
/// </summary>
public sealed class Chapter5 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_ExplainabilityAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ExplainabilityAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteTheory_ExplainabilityNote();
	}

	/// <summary>
	/// Executa o chain com decisão imperativa interna (if/else), sem trilha explícita.
	/// </summary>
	public async Task RunChain_ExplainabilityAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap5/chain/decision", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter5.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do chain (média/p95/p99).
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap5/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter5.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa o grafo com nó condicional e registra trilha via execução padrão (não-streaming) para métricas.
	/// </summary>
	public async Task RunGraph_ExplainabilityAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await BenchmarkUtils.MeasureManyAsync("cap5/graph/conditional", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter5.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo (média/p95/p99).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 50);
		await BenchmarkUtils.MeasureManyAsync("cap5/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter5.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B de latência entre chain e graph.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap5/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter5.RunAsync(s); },
			async s => { _ = await GraphChapter5.RunAsync(s); });
	}

	/// <summary>
	/// Registra uma nota teórica simples sobre explicabilidade em grafos vs chains.
	/// </summary>
	public static void WriteTheory_ExplainabilityNote()
	{
		BenchmarkUtils.WriteTheory("cap5/theory/explainability", new Dictionary<string, object>
		{
			["chain_trace"] = "implicit (no explicit path)",
			["graph_trace"] = "explicit path with conditional evaluation events",
			["compliance_note"] = "graphs enable audit trails aligning with AI governance"
		});
	}
}


