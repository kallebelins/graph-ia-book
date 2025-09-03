## Capítulo 26 — Piloto: GNN/preditor para Seleção de Caminho

Este exemplo demonstra um piloto onde um preditor (inspirado em GNN via features estruturais) informa a seleção de caminho na orquestração.

- **Chain (baseline)**: seleção de rota por média histórica (heurística).
- **Graph (SKG)**: preditor com features estruturais (grau, betweenness) estima custo e escolhe a rota.
- **Métricas**: latência média, p95, p99 e A/B (baseline vs preditor).

Artefatos:

- Código chain: `graph-ia-book/src/chains/chapter26/ChainChapter26.cs`
- Código graph: `graph-ia-book/src/graphs/chapter26/GraphChapter26.cs`
- Runner: `graph-ia-book/src/Chapters/Chapter26.cs`
- Resultados: `graph-ia-book/src/Benchmark/results/cap26_*`

Como executar:

```bash
dotnet run -c Debug --project graph-ia-book/src/book.csproj -- -c 26 -m chain
dotnet run -c Debug --project graph-ia-book/src/book.csproj -- -c 26 -m graph
dotnet run -c Debug --project graph-ia-book/src/book.csproj -- -c 26 -m benchmark
```

Saídas (exemplos):

- `src/Benchmark/results/cap26_chain_latency-summary.json` e `.md`
- `src/Benchmark/results/cap26_graph_latency-summary.json` e `.md`
- `src/Benchmark/results/cap26_benchmark_latency-ab-summary.json` e `.md`
- `src/Benchmark/results/cap26_theory_pilot-validation-summary.json` e `.md`

Notas:

- O cenário sintético está alinhado com o texto do capítulo `docs/book/34-capitulo-26.md`.
- O preditor não treina uma GNN real; usa pesos fixos para ilustrar a ideia de "modelo informa pesos/latências esperadas".


