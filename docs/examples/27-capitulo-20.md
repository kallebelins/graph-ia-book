## Capítulo 20 — Hierarquia e Subgrafos

Este capítulo demonstra composição hierárquica:

- Chain (SK): subpipeline sequencial — calcula FeatureA e FeatureB em série.
- Grafo (SKG): subgrafo com ramos paralelos — calcula FeatureA e FeatureB em paralelo e faz merge determinístico.

### Como executar

```bash
dotnet run --project src/book.csproj -- -c 20 -m chain
dotnet run --project src/book.csproj -- -c 20 -m graph
dotnet run --project src/book.csproj -- -c 20 -m benchmark
```

### Resultados e métricas

Arquivos gerados em `src/Benchmark/results` (exemplos):

- `cap20_chain_hierarchy-summary.json` e `.md`
- `cap20_graph_hierarchy-summary.json` e `.md`
- `cap20_chain_latency-summary.json` e `.md`
- `cap20_graph_latency-summary.json` e `.md`
- `cap20_benchmark_latency-ab-summary.json` e `.md`
- Teoria: `cap20_theory_hierarchy-speedup-summary.json` e `.md`

### Observação teórica

Quando custos dos ramos são similares, o subgrafo paralelo aproxima o tempo do maior ramo (max), enquanto o chain soma tempos (sum). Ignorando overheads, espera-se speedup próximo de 2× para dois ramos equivalentes.


