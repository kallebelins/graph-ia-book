namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Capítulo 4 — A definir. Consulte docs/book/06-capitulo-4.md.
/// Esqueleto com métodos de Chain, Graph e Benchmark para futuras métricas.
/// </summary>
public sealed class Chapter4 : IChapter
{
	public async Task RunChainAsync() => await RunChain_DefaultAsync();
	public async Task RunGraphAsync() => await RunGraph_DefaultAsync();
	public async Task RunBenchmarkAsync() => await RunBenchmark_DefaultAsync();

	public async Task RunChain_DefaultAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap4/chain/default", async () =>
		{
			await Task.Delay(5);
			return "ok";
		});
	}

	public async Task RunGraph_DefaultAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap4/graph/default", async () =>
		{
			await Task.Delay(5);
			return "ok";
		});
	}

	public async Task RunBenchmark_DefaultAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap4/benchmark/default", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}
}


