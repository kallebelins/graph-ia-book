namespace GraphIABook.Chapters;

/// <summary>
/// Template para capítulos: descreve o que será testado, medido ou comparado.
/// Substitua os comentários com a descrição específica do capítulo.
/// </summary>
public sealed class ChapterTemplate : IChapter
{
	/// <summary>
	/// Executa todos os experimentos do modo CHAIN deste capítulo.
	/// Deve invocar métodos específicos quando houver múltiplas métricas/relatórios (ex.: Latency, Cost).
	/// </summary>
	public async Task RunChainAsync()
	{
		await Task.CompletedTask;
	}

	/// <summary>
	/// Executa todos os experimentos do modo GRAPH deste capítulo.
	/// Deve invocar métodos específicos quando houver múltiplas métricas/relatórios (ex.: Latency, Cost).
	/// </summary>
	public async Task RunGraphAsync()
	{
		await Task.CompletedTask;
	}

	/// <summary>
	/// Executa o benchmark que compara os resultados de CHAIN vs GRAPH para este capítulo.
	/// Deve consolidar métricas e gerar relatório comparativo.
	/// </summary>
	public async Task RunBenchmarkAsync()
	{
		await Task.CompletedTask;
	}
}


