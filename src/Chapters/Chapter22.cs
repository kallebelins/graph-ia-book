namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter22;
using GraphIABook.Graphs.Chapter22;

/// <summary>
/// Capítulo 22 — Resiliência Probabilística e Fallback. Consulte docs/book/29-capitulo-22.md.
/// Este runner executa: (1) chain de fallback sequencial; (2) grafo com OR paralelo;
/// (3) sumários de latência (média/p95/p99) e (4) benchmark A/B; (5) escrita de teoria.
/// </summary>
public sealed class Chapter22 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_FallbackAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ORAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteFallbackTheory();
	}

	/// <summary>
	/// Chain: fallback sequencial entre três alternativas. Mede saída e latência.
	/// </summary>
	public async Task RunChain_FallbackAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap22/chain/fallback", async () =>
		{
			var output = await ChainChapter22.RunAsync("Cap22 Fallback (chain)");
			return output;
		});
	}

	/// <summary>
	/// Graph: OR paralelo entre três alternativas independentes, merge determinístico.
	/// </summary>
	public async Task RunGraph_ORAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap22/graph/or", async () =>
		{
			var output = await GraphChapter22.RunAsync("Cap22 Parallel OR (graph)");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain de fallback sequencial.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap22/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter22.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo com OR paralelo.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap22/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter22.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain (fallback) e graph (OR paralelo).
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap22/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter22.RunAsync(s); },
			async s => { _ = await GraphChapter22.RunAsync(s); });
	}

	/// <summary>
	/// Escreve resumo teórico para fallback sequencial e OR paralelo.
	/// </summary>
	public static void WriteFallbackTheory()
	{
		// Parâmetros do capítulo: três alternativas, p e t (ms)
		double[] p = new[] { 0.6, 0.5, 0.4 };
		int[] tMs = new[] { 200, 120, 80 };
		// Ordem por p/t descendente: 1 -> 0 -> 2
		int[] order = new[] { 1, 0, 2 };

		// p_total (sequencial e OR são iguais sob independência): 1 - Π(1 - p_i)
		double failProd = 1.0;
		for (int k = 0; k < p.Length; k++) failProd *= (1.0 - p[k]);
		double pTotal = 1.0 - failProd;

		// E[T]_seq = t1 + (1-p1) t2 + (1-p1)(1-p2) t3, na ordem escolhida
		double expectedSeq = 0.0;
		double prefixFail = 1.0;
		for (int k = 0; k < order.Length; k++)
		{
			int idx = order[k];
			expectedSeq += prefixFail * tMs[idx];
			prefixFail *= (1.0 - p[idx]);
		}

		// Aproximação simples para OR paralelo com cancelamento: E[T]_or ≈ p_total * min(t) + (1-p_total) * max(t)
		int tMin = tMs.Min();
		int tMax = tMs.Max();
		double expectedOrApprox = pTotal * tMin + (1.0 - pTotal) * tMax;

		BenchmarkUtils.WriteTheory("cap22/theory/fallback", new Dictionary<string, object>
		{
			["p"] = p,
			["tMs"] = tMs,
			["order"] = order,
			["p_total"] = pTotal,
			["E_T_seq_ms"] = expectedSeq,
			["E_T_or_ms_approx"] = expectedOrApprox,
			["notes"] = "Seq: E[T]=t1+(1-p1)t2+(1-p1)(1-p2)t3; OR: p=1-Π(1-p_i); aproximação para tempo esperado com cancelamento."
		});
	}
}


