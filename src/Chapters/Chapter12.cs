namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter12;
using GraphIABook.Graphs.Chapter12;

/// <summary>
/// Capítulo 12 — Agentes Autônomos com Grafos. Consulte docs/book/16-capitulo-12.md.
/// Demonstra chain (pipeline rígido com decisão imperativa) vs graph (roteamento dinâmico)
/// no cenário de agente de suporte que escolhe entre FAQ, Code Search ou Escalonamento.
/// </summary>
public sealed class Chapter12 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_AgentDemoAsync();
		await RunChain_LatencySummaryAsync();
	}
	public async Task RunGraphAsync()
	{
		await RunGraph_AgentDemoAsync();
		await RunGraph_LatencySummaryAsync();
	}
	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteAgentAutonomyTheory();
	}

	/// <summary>
	/// Demonstra o pipeline SK (chain) com decisão imperativa, sem alteração estrutural.
	/// </summary>
	public async Task RunChain_AgentDemoAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap12/chain/agent-demo", async () =>
		{
			var output = await ChainChapter12.RunAsync("qual é o preço do plano enterprise?");
			return output;
		});
	}

	/// <summary>
	/// Demonstra o grafo SKG com roteamento dinâmico entre FAQ, Code e Escalonamento.
	/// </summary>
	public async Task RunGraph_AgentDemoAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap12/graph/agent-demo", async () =>
		{
			var output = await GraphChapter12.RunAsync("estou vendo uma exception NullReference em produção");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain (pipeline rígido) do capítulo 12.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap12/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter12.RunAsync(inputs[idx]);
		});
	}
	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo (roteamento dinâmico) do capítulo 12.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap12/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter12.RunAsync(inputs[idx]);
		});
	}
	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e graph utilizando os mesmos inputs.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap12/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter12.RunAsync(s); },
			async s => { _ = await GraphChapter12.RunAsync(s); });
	}
	/// <summary>
	/// Escreve teoria/validação: grafo é acíclico, possui múltiplos caminhos (FAQ/Code/Escalate),
	/// e explicita autonomia estrutural (decisão integrada ao grafo) vs chain (decisão imperativa).
	/// </summary>
	public static void WriteAgentAutonomyTheory()
	{
		var graph = GraphChapter12.CreateExecutor();
		var (isAcyclic, topo) = GraphValidationUtils.Analyze(graph);
		var chainPathCount = 1;
		var graphAlternativePaths = 3;
		BenchmarkUtils.WriteTheory("cap12/theory/autonomy", new Dictionary<string, object>
		{
			["isAcyclic"] = isAcyclic,
			["topologicalOrder"] = topo is null ? "" : string.Join(" -> ", topo),
			["chain_path_count"] = chainPathCount,
			["graph_alternative_paths"] = graphAlternativePaths,
			["autonomy_relation"] = "chain = sequência rígida; graph = espaço de caminhos com decisão nativa"
		});
	}
}


