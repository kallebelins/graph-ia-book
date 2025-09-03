### Capítulo 17 — Execução por Ordem Topológica e Caminho Crítico

Este exemplo acompanha o texto em `docs/book/23-capitulo-17.md`.

- Chain: execução serial do DAG em ordem topológica (sem paralelismo) até o merge.
- Graph: DAG com dependências explícitas, paralelismo onde possível, merge determinístico.

Execução (runner):

```bash
# Chain
dotnet run --project src/book.csproj -- -c 17 -m c
# Graph
dotnet run --project src/book.csproj -- -c 17 -m g
# Benchmark (comparação e teoria do caminho crítico)
dotnet run --project src/book.csproj -- -c 17 -m b
```

Resultados esperados (arquivos gerados):
- `src/Benchmark/results/cap17_chain_latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap17_graph_latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap17_benchmark_latency-ab-summary.{json,md}`
- `src/Benchmark/results/cap17_theory_scheduling-critical-path-summary.{json,md}`

Relações teóricas refletidas:
- Chain: T_chain = soma dos tempos + α (sem paralelismo)
- Graph: T_graph = max sobre caminhos (caminho crítico) + α


