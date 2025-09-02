namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Capítulo 5 — A definir. Consulte docs/book/08-capitulo-5.md.
/// </summary>
public sealed class Chapter5 : IChapter
{
	public async Task RunChainAsync() => await RunChain_DefaultAsync();
	public async Task RunGraphAsync() => await RunGraph_DefaultAsync();
	public async Task RunBenchmarkAsync() => await RunBenchmark_DefaultAsync();

	public async Task RunChain_DefaultAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap5/chain/default", async () =>
		{
			await Task.Delay(5);
			return "ok";
		});
	}

	public async Task RunGraph_DefaultAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap5/graph/default", async () =>
		{
			await Task.Delay(5);
			return "ok";
		});
	}

	public async Task RunBenchmark_DefaultAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap5/benchmark/default", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}
}


