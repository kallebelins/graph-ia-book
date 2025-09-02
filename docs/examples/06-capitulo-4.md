# Capítulo 4 — Explosão de Estados e Modularidade (Exemplos)

- Chain: `graph-ia-book/src/chains/chapter4/ChainChapter4.cs`
- Graph: `graph-ia-book/src/graphs/chapter4/GraphChapter4.cs`
- Chapter runner: `graph-ia-book/src/Chapters/Chapter4.cs`
- Scripts: `graph-ia-book/src/Scripts/run-06-*.ps1|.sh`

## Como executar

```bash
# Chain
./src/Scripts/run-06-chain.sh
# Graph
./src/Scripts/run-06-graph.sh
# Benchmark (A/B)
./src/Scripts/run-06-benchmark.sh
```

No Windows (PowerShell):

```powershell
./src/Scripts/run-06-chain.ps1
./src/Scripts/run-06-graph.ps1
./src/Scripts/run-06-benchmark.ps1
```

## Métricas e Resultados

Arquivos gerados em `graph-ia-book/src/Benchmark/results/` com prefixos:

- `cap4_chain_state-explosion-summary.{json,md}`
- `cap4_chain_latency-summary.{json,md}`
- `cap4_graph_modularity-convergence-summary.{json,md}`
- `cap4_graph_latency-summary.{json,md}`
- `cap4_benchmark_latency-ab-summary.{json,md}`
- Teoria: `cap4_theory_state-counts-summary.{json,md}`

## Observações

- Chain: decisões/handlers fora do pipeline acoplam estágios e ampliam combinações possíveis.
- Graph: falhas convergem em nó compartilhado de tratamento, reduzindo o espaço efetivo de estados por reuso.
