namespace GraphIABook.Benchmark._common;

using System.Diagnostics;
using System.Text;
using System.Text.Json;

/// <summary>
/// Utilitários de benchmark: medição única e agregada (média, p95, p99) com exportação JSON/Markdown.
/// </summary>
public static class BenchmarkUtils
{
	/// <summary>
	/// Resultado de uma medição única.
	/// </summary>
	public sealed record MeasurementResult(string Name, long ElapsedMs, DateTimeOffset Timestamp, string Outcome);

	/// <summary>
	/// Resumo agregado de múltiplas medições (média/p95/p99).
	/// </summary>
	public sealed record SummaryResult(string Name, int Iterations, long MeanMs, long P95Ms, long P99Ms, DateTimeOffset Timestamp);

	/// <summary>
	/// Resumo agregado de taxa de sucesso em múltiplas execuções.
	/// </summary>
	public sealed record SuccessSummaryResult(string Name, int Iterations, int SuccessCount, double SuccessRate, DateTimeOffset Timestamp);

	/// <summary>
	/// Resultado genérico para teorias/validações (ex.: limites de Brent).
	/// </summary>
	public sealed record TheoryResult(string Name, DateTimeOffset Timestamp, IReadOnlyDictionary<string, object> Data);

	/// <summary>
	/// Mede a execução de uma função assíncrona e retorna o tempo decorrido.
	/// </summary>
	public static async Task<MeasurementResult> MeasureAsync(string name, Func<Task<string>> action)
	{
		var stopwatch = Stopwatch.StartNew();
		var outcome = await action();
		stopwatch.Stop();
		var result = new MeasurementResult(name, stopwatch.ElapsedMilliseconds, DateTimeOffset.UtcNow, outcome);
		Console.WriteLine(JsonSerializer.Serialize(result));
		return result;
	}

	/// <summary>
	/// Executa a ação múltiplas vezes e calcula média, p95 e p99, exportando os resultados.
	/// </summary>
	public static async Task<SummaryResult> MeasureManyAsync(string name, int iterations, Func<Task> action, string? resultsDirectory = null)
	{
		if (iterations <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(iterations));
		}

		var elapsedSamplesMs = new List<long>(capacity: iterations);
		for (var i = 0; i < iterations; i++)
		{
			var stopwatch = Stopwatch.StartNew();
			await action();
			stopwatch.Stop();
			elapsedSamplesMs.Add(stopwatch.ElapsedMilliseconds);
		}

		elapsedSamplesMs.Sort();
		long mean = (long)elapsedSamplesMs.Average();
		long p95 = Percentile(elapsedSamplesMs, 0.95);
		long p99 = Percentile(elapsedSamplesMs, 0.99);

		var summary = new SummaryResult(name, iterations, mean, p95, p99, DateTimeOffset.UtcNow);
		Console.WriteLine(JsonSerializer.Serialize(summary));

		WriteOutputs(name, summary, resultsDirectory);
		return summary;
	}

	/// <summary>
	/// Executa a ação booleana múltiplas vezes e calcula taxa de sucesso, exportando os resultados.
	/// </summary>
	public static async Task<SuccessSummaryResult> MeasureSuccessAsync(string name, int iterations, Func<Task<bool>> action, string? resultsDirectory = null)
	{
		if (iterations <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(iterations));
		}

		int successCount = 0;
		for (var i = 0; i < iterations; i++)
		{
			if (await action())
			{
				successCount++;
			}
		}

		double rate = iterations == 0 ? 0 : (double)successCount / iterations;
		var summary = new SuccessSummaryResult(name, iterations, successCount, rate, DateTimeOffset.UtcNow);
		Console.WriteLine(JsonSerializer.Serialize(summary));
		WriteOutputs(name, summary, resultsDirectory);
		return summary;
	}

	/// <summary>
	/// Calcula o percentil para uma lista de amostras ordenadas.
	/// </summary>
	private static long Percentile(IReadOnlyList<long> sortedSamples, double percentile)
	{
		if (sortedSamples.Count == 0)
		{
			return 0;
		}
		var rank = percentile * (sortedSamples.Count - 1);
		var lowerIndex = (int)Math.Floor(rank);
		var upperIndex = (int)Math.Ceiling(rank);
		if (lowerIndex == upperIndex)
		{
			return sortedSamples[lowerIndex];
		}
		var weight = rank - lowerIndex;
		return (long)Math.Round(sortedSamples[lowerIndex] * (1 - weight) + sortedSamples[upperIndex] * weight);
	}

	/// <summary>
	/// Escreve os arquivos JSON e Markdown do resumo no diretório de resultados.
	/// </summary>
	private static void WriteOutputs(string name, SummaryResult summary, string? resultsDirectory)
	{
		// Prefer fixed repo path: src/Benchmark/results (3 levels up from bin/Debug/netX.Y)
		var defaultResultsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Benchmark", "results"));
		var baseDir = resultsDirectory ?? defaultResultsDir;
		Directory.CreateDirectory(baseDir);
		var safeName = name.Replace('/', '_').Replace('\\', '_');
		var jsonPath = Path.Combine(baseDir, $"{safeName}-summary.json");
		var mdPath = Path.Combine(baseDir, $"{safeName}-summary.md");

		File.WriteAllText(jsonPath, JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }));
		File.WriteAllText(mdPath, BuildMarkdown(summary));
	}

	/// <summary>
	/// Escreve os arquivos JSON e Markdown do resumo de sucesso no diretório de resultados.
	/// </summary>
	private static void WriteOutputs(string name, SuccessSummaryResult summary, string? resultsDirectory)
	{
		var defaultResultsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Benchmark", "results"));
		var baseDir = resultsDirectory ?? defaultResultsDir;
		Directory.CreateDirectory(baseDir);
		var safeName = name.Replace('/', '_').Replace('\\', '_');
		var jsonPath = Path.Combine(baseDir, $"{safeName}-summary.json");
		var mdPath = Path.Combine(baseDir, $"{safeName}-summary.md");

		File.WriteAllText(jsonPath, JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }));
		File.WriteAllText(mdPath, BuildMarkdown(summary));
	}

	/// <summary>
	/// Escreve um resultado teórico (pares chave/valor) em JSON e Markdown.
	/// </summary>
	public static void WriteTheory(string name, IReadOnlyDictionary<string, object> entries, string? resultsDirectory = null)
	{
		var defaultResultsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Benchmark", "results"));
		var baseDir = resultsDirectory ?? defaultResultsDir;
		Directory.CreateDirectory(baseDir);
		var safeName = name.Replace('/', '_').Replace('\\', '_');
		var jsonPath = Path.Combine(baseDir, $"{safeName}-summary.json");
		var mdPath = Path.Combine(baseDir, $"{safeName}-summary.md");

		var theory = new TheoryResult(name, DateTimeOffset.UtcNow, entries);
		File.WriteAllText(jsonPath, JsonSerializer.Serialize(theory, new JsonSerializerOptions { WriteIndented = true }));
		File.WriteAllText(mdPath, BuildMarkdown(theory));
	}

	/// <summary>
	/// Constrói o conteúdo Markdown para o arquivo de resumo.
	/// </summary>
	private static string BuildMarkdown(SummaryResult s)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"# {s.Name} — Resumo de Métricas");
		sb.AppendLine();
		sb.AppendLine($"- Iterações: {s.Iterations}");
		sb.AppendLine($"- Média (ms): {s.MeanMs}");
		sb.AppendLine($"- p95 (ms): {s.P95Ms}");
		sb.AppendLine($"- p99 (ms): {s.P99Ms}");
		sb.AppendLine($"- Timestamp (UTC): {s.Timestamp:O}");
		return sb.ToString();
	}

	/// <summary>
	/// Constrói o conteúdo Markdown para o arquivo de resumo de sucesso.
	/// </summary>
	private static string BuildMarkdown(SuccessSummaryResult s)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"# {s.Name} — Resumo de Sucesso");
		sb.AppendLine();
		sb.AppendLine($"- Iterações: {s.Iterations}");
		sb.AppendLine($"- Sucessos: {s.SuccessCount}");
		sb.AppendLine($"- Taxa de sucesso: {s.SuccessRate:P2}");
		sb.AppendLine($"- Timestamp (UTC): {s.Timestamp:O}");
		return sb.ToString();
	}

	/// <summary>
	/// Constrói o conteúdo Markdown para dados teóricos (pares chave/valor).
	/// </summary>
	private static string BuildMarkdown(TheoryResult t)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"# {t.Name} — Validação Teórica");
		sb.AppendLine();
		sb.AppendLine($"- Timestamp (UTC): {t.Timestamp:O}");
		foreach (var kvp in t.Data.OrderBy(k => k.Key))
		{
			sb.AppendLine($"- {kvp.Key}: {kvp.Value}");
		}
		return sb.ToString();
	}
}


