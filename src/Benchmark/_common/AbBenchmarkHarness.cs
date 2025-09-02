namespace GraphIABook.Benchmark._common;

using System.Collections.Concurrent;

/// <summary>
/// Harness de benchmark A/B para comparar pipelines SK (chain) vs SKG (graph)
/// sob as mesmas entradas determinísticas.
/// </summary>
public static class AbBenchmarkHarness
{
	/// <summary>
	/// Executa medições de latência para CHAIN e GRAPH usando o mesmo conjunto de entradas determinísticas.
	/// Resultados individuais são exportados para JSON/Markdown via BenchmarkUtils com sufixos "_chain_latency" e "_graph_latency".
	/// </summary>
	/// <param name="name">Nome base do experimento (ex.: "00e_benchmark_latency").</param>
	/// <param name="inputs">Entradas determinísticas a serem processadas em cada iteração.</param>
	/// <param name="runChainPerInput">Função que executa o pipeline CHAIN para uma entrada.</param>
	/// <param name="runGraphPerInput">Função que executa o pipeline GRAPH para uma entrada.</param>
	/// <param name="resultsDirectory">Diretório opcional de saída; se nulo, usa o diretório padrão em src/Benchmark/results.</param>
	public static async Task RunLatencyABAsync(
		string name,
		IReadOnlyList<string> inputs,
		Func<string, Task> runChainPerInput,
		Func<string, Task> runGraphPerInput,
		string? resultsDirectory = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		ArgumentNullException.ThrowIfNull(inputs);
		ArgumentNullException.ThrowIfNull(runChainPerInput);
		ArgumentNullException.ThrowIfNull(runGraphPerInput);

		// CHAIN
		var chainIndex = 0;
		await BenchmarkUtils.MeasureManyAsync(
			$"{name}_chain_latency",
			iterations: inputs.Count,
			action: async () =>
			{
				var i = Interlocked.Increment(ref chainIndex) - 1;
				var idx = Math.Clamp(i, 0, inputs.Count - 1);
				await runChainPerInput(inputs[idx]);
			},
			resultsDirectory: resultsDirectory);

		// GRAPH
		var graphIndex = 0;
		await BenchmarkUtils.MeasureManyAsync(
			$"{name}_graph_latency",
			iterations: inputs.Count,
			action: async () =>
			{
				var i = Interlocked.Increment(ref graphIndex) - 1;
				var idx = Math.Clamp(i, 0, inputs.Count - 1);
				await runGraphPerInput(inputs[idx]);
			},
			resultsDirectory: resultsDirectory);
	}

	/// <summary>
	/// Executa ações em paralelo para "aquecimento" (warmup) antes das medições, sem coletar métricas.
	/// </summary>
	public static async Task WarmupAsync(
		IEnumerable<string> sampleInputs,
		Func<string, Task> runChainPerInput,
		Func<string, Task> runGraphPerInput,
		int parallelism = 2)
	{
		ArgumentNullException.ThrowIfNull(sampleInputs);
		ArgumentNullException.ThrowIfNull(runChainPerInput);
		ArgumentNullException.ThrowIfNull(runGraphPerInput);

		var inputs = sampleInputs.ToArray();
		var tasks = new List<Task>();
		var limiter = new SemaphoreSlim(parallelism);
		for (var i = 0; i < inputs.Length; i++)
		{
			await limiter.WaitAsync();
			var input = inputs[i];
			tasks.Add(Task.Run(async () =>
			{
				try
				{
					await runChainPerInput(input);
					await runGraphPerInput(input);
				}
				finally
				{
					limiter.Release();
				}
			}));
		}
		await Task.WhenAll(tasks);
	}
}


