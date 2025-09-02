namespace GraphIABook.Benchmark._common;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Extensions;
using SemanticKernel.Graph.Streaming;

/// <summary>
/// Utilitários para habilitar métricas e realizar tracing/streaming por nó e caminho.
/// </summary>
public static class GraphLoggingUtils
{
	/// <summary>
	/// Habilita métricas no executor de grafo (modo desenvolvimento ou produção).
	/// </summary>
	public static void EnableMetrics(GraphExecutor executor, bool production = false)
	{
		ArgumentNullException.ThrowIfNull(executor);
		if (production)
		{
			executor.EnableProductionMetrics();
		}
		else
		{
			executor.EnableDevelopmentMetrics();
		}
	}

	/// <summary>
	/// Executa o grafo em modo streaming, emitindo logs simples por evento (nó iniciado/concluído, início/fim da execução).
	/// </summary>
	public static async Task TraceExecutionAsync(
		StreamingGraphExecutor executor,
		IKernel kernel,
		KernelArguments arguments,
		StreamingExecutionOptions? options = null,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(executor);
		ArgumentNullException.ThrowIfNull(kernel);
		ArgumentNullException.ThrowIfNull(arguments);

		var execOptions = options ?? new StreamingExecutionOptions
		{
			EventTypesToEmit = new[]
			{
				GraphExecutionEventType.ExecutionStarted,
				GraphExecutionEventType.NodeStarted,
				GraphExecutionEventType.NodeCompleted,
				GraphExecutionEventType.ExecutionCompleted
			}
		};

		var stream = executor.ExecuteStreamAsync(kernel, arguments, execOptions, cancellationToken);
		await foreach (var @event in stream.WithCancellation(cancellationToken))
		{
			var ts = @event.Timestamp.ToString("O");
			switch (@event)
			{
				case GraphExecutionStartedEvent started:
					Console.WriteLine($"[TRACE] {ts} EXECUTION STARTED id={started.ExecutionId}");
					break;
				case NodeExecutionStartedEvent nodeStart:
					Console.WriteLine($"[TRACE] {ts} NODE START name={nodeStart.Node.Name} id={nodeStart.Node.NodeId}");
					break;
				case NodeExecutionCompletedEvent nodeCompleted:
					Console.WriteLine($"[TRACE] {ts} NODE DONE name={nodeCompleted.Node.Name} id={nodeCompleted.Node.NodeId} durMs={nodeCompleted.ExecutionDuration.TotalMilliseconds:F0}");
					break;
				case GraphExecutionCompletedEvent completed:
					Console.WriteLine($"[TRACE] {ts} EXECUTION COMPLETED durMs={completed.TotalDuration.TotalMilliseconds:F0}");
					break;
			}
		}
	}
}


