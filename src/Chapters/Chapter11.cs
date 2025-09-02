namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter11;
using GraphIABook.Graphs.Chapter11;

/// <summary>
/// Capítulo 11 — Aplicações Demonstrativas (Turismo, Finanças, Saúde).
/// Compara pipelines CHAIN lineares vs GRAPH paralelos/multimodais.
/// Mede latência média, p95/p99 e custo aproximado por cenário.
/// Consulte `docs/book/15-capitulo-11.md`.
/// </summary>
public sealed class Chapter11 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_TourismAsync();
		await RunChain_FinanceAsync();
		await RunChain_HealthAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_TourismAsync();
		await RunGraph_FinanceAsync();
		await RunGraph_HealthAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_TourismAsync();
		await RunBenchmark_FinanceAsync();
		await RunBenchmark_HealthAsync();
		WriteMakespanTheory();
	}

	public async Task RunChain_TourismAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap11/chain/tourism/latency", inputs.Count, async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter11.RunTourismAsync(inputs[idx]);
		});
	}

	public async Task RunChain_FinanceAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap11/chain/finance/latency", inputs.Count, async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter11.RunFinanceAsync(inputs[idx]);
		});
	}

	public async Task RunChain_HealthAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap11/chain/health/latency", inputs.Count, async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter11.RunHealthAsync(inputs[idx]);
		});
	}

	public async Task RunGraph_TourismAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap11/graph/tourism/latency", inputs.Count, async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter11.RunTourismAsync(inputs[idx]);
		});
	}

	public async Task RunGraph_FinanceAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap11/graph/finance/latency", inputs.Count, async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter11.RunFinanceAsync(inputs[idx]);
		});
	}

	public async Task RunGraph_HealthAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 20);
		await BenchmarkUtils.MeasureManyAsync("cap11/graph/health/latency", inputs.Count, async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter11.RunHealthAsync(inputs[idx]);
		});
	}

	public async Task RunBenchmark_TourismAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap11/benchmark/tourism",
			inputs,
			async s => { _ = await ChainChapter11.RunTourismAsync(s); },
			async s => { _ = await GraphChapter11.RunTourismAsync(s); });
	}

	public async Task RunBenchmark_FinanceAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap11/benchmark/finance",
			inputs,
			async s => { _ = await ChainChapter11.RunFinanceAsync(s); },
			async s => { _ = await GraphChapter11.RunFinanceAsync(s); });
	}

	public async Task RunBenchmark_HealthAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap11/benchmark/health",
			inputs,
			async s => { _ = await ChainChapter11.RunHealthAsync(s); },
			async s => { _ = await GraphChapter11.RunHealthAsync(s); });
	}

	/// <summary>
	/// Writes theory files estimating makespan for each case: chain=sum vs graph=max(parallel)+overheads.
	/// </summary>
	public static void WriteMakespanTheory()
	{
		// Tourism
		var tGraph = GraphChapter11.CreateTourismExecutor();
		var (tAcyclic, tTopo) = GraphValidationUtils.Analyze(tGraph);
		var tChain = Chains.Chapter11.ChainChapter11.TInterpretMs + Chains.Chapter11.ChainChapter11.THotelsMs + Chains.Chapter11.ChainChapter11.TWeatherMs + Chains.Chapter11.ChainChapter11.TEventsMs + Chains.Chapter11.ChainChapter11.TAnswerMs;
		var tParallel = Math.Max(Graphs.Chapter11.GraphChapter11.THotelsMs, Math.Max(Graphs.Chapter11.GraphChapter11.TWeatherMs, Graphs.Chapter11.GraphChapter11.TEventsMs));
		var tGraphSum = tParallel + Graphs.Chapter11.GraphChapter11.TFuseMs;
		BenchmarkUtils.WriteTheory("cap11/theory/tourism", new Dictionary<string, object>
		{
			["isAcyclic"] = tAcyclic,
			["topologicalOrder"] = tTopo is null ? "" : string.Join(" -> ", tTopo),
			["chain_ms_sum"] = tChain,
			["graph_ms_sum"] = tGraphSum,
			["graph_parallel_branch_ms"] = tParallel
		});

		// Finance
		var fGraph = GraphChapter11.CreateFinanceExecutor();
		var (fAcyclic, fTopo) = GraphValidationUtils.Analyze(fGraph);
		var fChain = Chains.Chapter11.ChainChapter11.FIngestMs + Chains.Chapter11.ChainChapter11.FGeoMs + Chains.Chapter11.ChainChapter11.FHistoryMs + Chains.Chapter11.ChainChapter11.FExternalMs + Chains.Chapter11.ChainChapter11.FDecisionMs;
		var fParallel = Math.Max(Graphs.Chapter11.GraphChapter11.FGeoMs, Math.Max(Graphs.Chapter11.GraphChapter11.FHistoryMs, Math.Max(Graphs.Chapter11.GraphChapter11.FExternalMs, Graphs.Chapter11.GraphChapter11.FMLMs)));
		var fGraphSum = fParallel + Graphs.Chapter11.GraphChapter11.FDecisionMs;
		BenchmarkUtils.WriteTheory("cap11/theory/finance", new Dictionary<string, object>
		{
			["isAcyclic"] = fAcyclic,
			["topologicalOrder"] = fTopo is null ? "" : string.Join(" -> ", fTopo),
			["chain_ms_sum"] = fChain,
			["graph_ms_sum"] = fGraphSum,
			["graph_parallel_branch_ms"] = fParallel
		});

		// Health
		var hGraph = GraphChapter11.CreateHealthExecutor();
		var (hAcyclic, hTopo) = GraphValidationUtils.Analyze(hGraph);
		var hChain = Chains.Chapter11.ChainChapter11.HSymptomsMs + Chains.Chapter11.ChainChapter11.HTextMs + Chains.Chapter11.ChainChapter11.HEmrMs + Chains.Chapter11.ChainChapter11.HImagingMs + Chains.Chapter11.ChainChapter11.HClassifyMs;
		var hParallel = Math.Max(Graphs.Chapter11.GraphChapter11.HTextMs, Math.Max(Graphs.Chapter11.GraphChapter11.HEmrMs, Graphs.Chapter11.GraphChapter11.HImagingMs));
		var hGraphSum = hParallel + Graphs.Chapter11.GraphChapter11.HFuseMs + Graphs.Chapter11.GraphChapter11.HClassifyMs;
		BenchmarkUtils.WriteTheory("cap11/theory/health", new Dictionary<string, object>
		{
			["isAcyclic"] = hAcyclic,
			["topologicalOrder"] = hTopo is null ? "" : string.Join(" -> ", hTopo),
			["chain_ms_sum"] = hChain,
			["graph_ms_sum"] = hGraphSum,
			["graph_parallel_branch_ms"] = hParallel
		});
	}
}


