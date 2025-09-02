namespace GraphIABook.Benchmark._common;

using System.Collections.ObjectModel;
using System.Text;

/// <summary>
/// Fixtures de dados determinísticos para benchmarks reprodutíveis.
/// </summary>
public static class TestFixtures
{
	private static readonly string[] DefaultPrompts = new[]
	{
		"Explique o conceito de grafo em palavras simples.",
		"Resuma em 2 frases o papel de nós e arestas.",
		"Liste 3 casos de uso de grafos em IA.",
		"Qual a diferença entre chain e graph orquestration?",
		"Defina caminho crítico em um DAG.",
		"Quando paralelizar ramos é melhor que sequência?",
		"Explique ordem topológica e sua utilidade.",
		"O que é um nó condicional em grafos de execução?",
		"Como lidar com falhas e fallback em grafos?",
		"Quando um pipeline linear é preferível a um grafo?"
	};

	/// <summary>
	/// Retorna uma lista determinística de entradas textuais baseada em uma semente.
	/// </summary>
	public static IReadOnlyList<string> GetFixedTextInputs(int count = 10, int seed = 12345)
	{
		if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
		var rnd = new Random(seed);
		var list = new List<string>(capacity: count);
		for (var i = 0; i < count; i++)
		{
			var idx = rnd.Next(0, DefaultPrompts.Length);
			list.Add(DefaultPrompts[idx]);
		}
		return new ReadOnlyCollection<string>(list);
	}

	/// <summary>
	/// Pequeno dataset Q&A sintético e determinístico para testes básicos.
	/// </summary>
	public static IReadOnlyList<(string Question, string ExpectedContains)> GetQaPairs()
	{
		return new List<(string, string)>
		{
			("O que é um DAG?", "aciclico"),
			("Para que serve ordem topológica?", "sequência"),
			("Quando usar chain vs graph?", "depende"),
			("Explique fallback em grafos.", "falha"),
			("Defina caminho crítico.", "crítico")
		};
	}
}


