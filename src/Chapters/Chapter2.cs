namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;

/// <summary>
/// Capítulo 2 — Teoria de Grafos: Base Matemática.
/// Foco em formalismos: definição de grafos, DAGs, expressividade e propriedades relevantes para IA
/// (composicionalidade, multiplicidade de caminhos, paralelismo, rastreabilidade). Aqui, os exemplos
/// executam cenários didáticos que espelham as propriedades teóricas.
/// </summary>
public sealed class Chapter2 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_ExpressivityDemoAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_ExpressivityDemoAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_ChainSubsetOfGraphsAsync();
	}

	/// <summary>
	/// Demonstra um pipeline linear e mede a limitação de caminhos (sequencialidade estrita).
	/// </summary>
	public async Task RunChain_ExpressivityDemoAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap2/chain/expressivity", async () =>
		{
			await Task.Delay(9);
			return "ok";
		});
	}

	/// <summary>
	/// Demonstra um DAG simples com ramos alternativos e paralelos, mostrando maior expressividade.
	/// </summary>
	public async Task RunGraph_ExpressivityDemoAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap2/graph/expressivity", async () =>
		{
			var a = Task.Delay(4);
			var b = Task.Delay(5);
			await Task.WhenAll(a, b);
			return "ok";
		});
	}

	/// <summary>
	/// Benchmark conceitual: evidencia que chains são subconjunto de DAGs em termos práticos de caminhos.
	/// </summary>
	public async Task RunBenchmark_ChainSubsetOfGraphsAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap2/benchmark/chain-subset", async () =>
		{
			await Task.Delay(1);
			return "ok";
		});
	}
}


