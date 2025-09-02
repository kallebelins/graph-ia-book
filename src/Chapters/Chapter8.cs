namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter8;
using GraphIABook.Graphs.Chapter8;

/// <summary>
/// Capítulo 8 — Integração Multimodal e Híbrida. Consulte docs/book/11-capitulo-8.md.
/// Demonstra limitação do chain (voz apenas) vs grafo multimodal (voz+imagem) com fusão tardia.
/// </summary>
public sealed class Chapter8 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_VoiceOnlyAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_MultimodalFusionAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteFusionTheory();
	}

	/// <summary>
	/// Executa chain voz→texto→resposta (ignora imagem) para evidenciar limitação.
	/// </summary>
	public async Task RunChain_VoiceOnlyAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap8/chain/voice-only", async () =>
		{
			var output = await ChainChapter8.RunAsync("voice:Qual o preço? image:produto.png");
			return output;
		});
	}

	/// <summary>
	/// Executa grafo multimodal (voz+imagem) com fusão tardia.
	/// </summary>
	public async Task RunGraph_MultimodalFusionAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap8/graph/multimodal-fusion", async () =>
		{
			var output = await GraphChapter8.RunAsync("voice:Qual o preço? image:produto.png");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência do chain voz-only (média/p95/p99).
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30)
			.Select(s => $"voice:{s} image:produto.png").ToArray();
		await BenchmarkUtils.MeasureManyAsync("cap8/chain/latency", iterations: inputs.Length, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Length;
			_ = await ChainChapter8.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência do grafo multimodal (média/p95/p99).
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30)
			.Select(s => $"voice:{s} image:produto.png").ToArray();
		await BenchmarkUtils.MeasureManyAsync("cap8/graph/latency", iterations: inputs.Length, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Length;
			_ = await GraphChapter8.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B de latência sob as mesmas entradas entre chain voz-only e grafo multimodal.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 24)
			.Select(s => $"voice:{s} image:produto.png").ToArray();
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap8/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter8.RunAsync(s); },
			async s => { _ = await GraphChapter8.RunAsync(s); });
	}

	/// <summary>
	/// Escreve teoria simples de ganho estimado: chain = soma; graph ≈ max + fusão (late).
	/// </summary>
	public static void WriteFusionTheory()
	{
		int t_asr = GraphChapter8.SpeechToTextMs;
		int t_nlp = GraphChapter8.NlpMs;
		int t_vis = GraphChapter8.ImageAnalyzerMs;
		int t_fus = GraphChapter8.FusionMs;

		int chainSum = ChainChapter8.SpeechToTextMs + ChainChapter8.NlpMs + ChainChapter8.AnswerMs;
		int graphApprox = Math.Max(t_asr + t_nlp, t_vis) + t_fus;
		double speedup = graphApprox == 0 ? 0 : (double)chainSum / graphApprox;

		BenchmarkUtils.WriteTheory("cap8/theory/fusion", new Dictionary<string, object>
		{
			["t_asr_ms"] = t_asr,
			["t_nlp_ms"] = t_nlp,
			["t_vision_ms"] = t_vis,
			["t_fusion_ms"] = t_fus,
			["t_chain_sum_ms"] = chainSum,
			["t_graph_approx_ms"] = graphApprox,
			["speedup_chain_over_graph"] = speedup.ToString("F2")
		});
	}
}


