## Glossário Técnico — Exemplos mínimos

Este material acompanha `docs/book/36-glossario-tecnico.md` e demonstra, de forma mínima, os conceitos:

- DAG (grafo acíclico dirigido)
- Ordem topológica
- Centralidade de intermediação (betweenness)
- Caminho crítico (critical path) em DAG

### Código de suporte

Os exemplos estão em `graph-ia-book/src/Glossary/GlossaryExamples.cs` e constroem um pequeno DAG:

```
start -> a -> c -> end
start -> b -> c
```

### Como executar um demo rápido

Execute o runner e invoque o método de demonstração a partir de um capítulo ou REPL. Exemplo de uso em C# interativo:

```csharp
using GraphIABook.Glossary;

var result = await GlossaryExamples.RunDagDemoAsync();

Console.WriteLine("Topological order: " + string.Join(" -> ", result.TopologicalOrder));
Console.WriteLine("Critical path (duration=" + result.CriticalPathDuration + "): " + string.Join(" -> ", result.CriticalPathNodes));

foreach (var kv in result.BetweennessCentrality.OrderByDescending(k => k.Value))
{
    Console.WriteLine($"{kv.Key}: {kv.Value:F3}");
}
```

### Pedaços relevantes

- Ordem topológica via plano compilado do SKG:

```csharp
var plan = GraphPlanCompiler.Compile(executor);
var topo = plan.TopologicalOrder; // nulo se houver ciclos
```

- Centralidade de intermediação (versão simples não ponderada): `ComputeBetweennessCentrality(executor)`.

- Caminho crítico (DAG, duração por nó): `ComputeCriticalPath(executor, durations)` retornando `(TotalDuration, Path)`.

### Observações

- As estruturas seguem os padrões da biblioteca SKG: `GraphExecutor`, `FunctionGraphNode`, `GraphPlanCompiler`.
- Para grafos com ciclos, `TopologicalOrder` é nulo e o cálculo de caminho crítico não se aplica.


