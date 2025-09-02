namespace GraphIABook.Chapters;

/// <summary>
/// Contrato para capítulos executáveis no runner (chain, graph e benchmark).
/// </summary>
public interface IChapter
{
	/// <summary>
	/// Executa todos os cenários do modo CHAIN para o capítulo.
	/// </summary>
	Task RunChainAsync();

	/// <summary>
	/// Executa todos os cenários do modo GRAPH para o capítulo.
	/// </summary>
	Task RunGraphAsync();

	/// <summary>
	/// Executa os benchmarks comparativos (CHAIN vs GRAPH) para o capítulo.
	/// </summary>
	Task RunBenchmarkAsync();
}


