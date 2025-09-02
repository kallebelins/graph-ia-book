namespace GraphIABook.Benchmark._common;

using SemanticKernel.Graph.Core;

/// <summary>
/// Utilitários de validação estrutural para grafos SKG (aciclicidade e ordem topológica).
/// </summary>
public static class GraphValidationUtils
{
	/// <summary>
	/// Analisa o grafo e retorna se é acíclico e sua ordem topológica (se existir).
	/// </summary>
	public static (bool IsAcyclic, IReadOnlyList<string>? TopologicalOrder) Analyze(GraphExecutor executor)
	{
		ArgumentNullException.ThrowIfNull(executor);
		var plan = GraphPlanCompiler.Compile(executor);
		return (!plan.HasCycles, plan.TopologicalOrder);
	}

	/// <summary>
	/// Lança exceção se o grafo possuir ciclos.
	/// </summary>
	public static void EnsureAcyclic(GraphExecutor executor)
	{
		var (isAcyclic, _) = Analyze(executor);
		if (!isAcyclic)
		{
			throw new InvalidOperationException("Grafo possui ciclos (não é DAG). Revise dependências/arestas.");
		}
	}
}


