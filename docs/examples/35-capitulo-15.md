## Capítulo 15 — Síntese de três ramos (fan-out/fan-in)

Este experimento compara:

- Chain (SK): execução sequencial de A → B → C → Merge
- Grafo (SKG): execução paralela de {A, B, C} → Merge

O objetivo é observar o makespan: chain soma as durações, grafo aproxima o máximo dos ramos mais o custo de merge.

### Como executar

PowerShell:

```bash
dotnet run --project ./graph-ia-book/src/book.csproj -- --chapter 15 --mode chain
dotnet run --project ./graph-ia-book/src/book.csproj -- --chapter 15 --mode graph
dotnet run --project ./graph-ia-book/src/book.csproj -- --chapter 15 --mode benchmark
```

### Resultados esperados

- `cap15/chain/latency-summary-summary.{json,md}`
- `cap15/graph/latency-summary-summary.{json,md}`
- `cap15/theory/makespan-3branch-summary.{json,md}`

O grafo deve apresentar menor média e cauda (p95/p99) quando há assimetria entre os ramos, em razão do paralelismo e do merge determinístico.


