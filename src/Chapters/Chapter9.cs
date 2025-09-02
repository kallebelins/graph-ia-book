namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter9;
using GraphIABook.Graphs.Chapter9;

/// <summary>
/// Capítulo 9 — Governança e Segurança em Grafos. Consulte docs/book/12-capitulo-9.md.
/// Demonstra limitação do chain (bloqueio) vs grafo com PolicyGuard + Anonymizer e desvio seguro.
/// </summary>
public sealed class Chapter9 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_PolicyEnforcementAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_GovernanceAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteGovernanceTheory();
	}

	/// <summary>
	/// Executa chain com avaliação de política que bloqueia saída em caso de violação.
	/// </summary>
	public async Task RunChain_PolicyEnforcementAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap9/chain/policy-enforcement", async () =>
		{
			var output = await ChainChapter9.RunAsync("Pergunta com cpf:12345678901 e email:user@example.com");
			return output;
		});
	}

	/// <summary>
	/// Executa grafo com PolicyGuard e Anonymizer para desvio seguro e processamento.
	/// </summary>
	public async Task RunGraph_GovernanceAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap9/graph/governance-routing", async () =>
		{
			var output = await GraphChapter9.RunAsync("Pergunta com cpf:12345678901 e email:user@example.com");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência do chain (média/p95/p99) sob entradas com e sem violações.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var baseInputs = TestFixtures.GetFixedTextInputs(count: 30).ToArray();
		var inputs = baseInputs.Select((s, i) => i % 3 == 0 ? $"{s} cpf:12345678901" : (i % 5 == 0 ? $"{s} email:user@example.com" : s)).ToArray();
		await BenchmarkUtils.MeasureManyAsync("cap9/chain/latency", iterations: inputs.Length, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Length;
			_ = await ChainChapter9.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo (média/p95/p99) sob entradas com e sem violações.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var baseInputs = TestFixtures.GetFixedTextInputs(count: 30).ToArray();
		var inputs = baseInputs.Select((s, i) => i % 3 == 0 ? $"{s} cpf:12345678901" : (i % 5 == 0 ? $"{s} email:user@example.com" : s)).ToArray();
		await BenchmarkUtils.MeasureManyAsync("cap9/graph/latency", iterations: inputs.Length, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Length;
			_ = await GraphChapter9.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B de latência entre chain (bloqueio) e grafo (anônimo + processa).
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var baseInputs = TestFixtures.GetFixedTextInputs(count: 24).ToArray();
		var inputs = baseInputs.Select((s, i) => i % 3 == 0 ? $"{s} cpf:12345678901" : (i % 5 == 0 ? $"{s} email:user@example.com" : s)).ToArray();
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap9/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter9.RunAsync(s); },
			async s => { _ = await GraphChapter9.RunAsync(s); });
	}

	/// <summary>
	/// Escreve validação teórica simples: chain possui 2 pontos de monitoramento; graph tem governança por nó.
	/// </summary>
	public static void WriteGovernanceTheory()
	{
		BenchmarkUtils.WriteTheory("cap9/theory/governance", new Dictionary<string, object>
		{
			["chain_monitor_points"] = 2,
			["graph_monitor_nodes"] = 3,
			["supports_safe_routing"] = true,
			["supports_local_policies"] = true
		});
	}
}


