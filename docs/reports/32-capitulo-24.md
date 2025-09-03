# Relatório Capítulo 24 — Topologias e Anti-padrões

Este relatório consolida resultados do Capítulo 24, comparando cadeia (SK) e grafo (SKG) sob diferentes topologias, destacando anti-padrões que ampliam cauda de latência. Inclui A/B e validação teórica de makespan por caminho crítico.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap24_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap24_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap24_theory_topologies-makespan-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 113 | 120 | 281 |
| Graph (SKG) | 30 | 51 | 50 | 163 |

O grafo reduz média e cauda, evidenciando benefícios de paralelismo e topologias com agregação adequada, evitando anti-padrões como funis únicos.

### Teoria: Caminho Crítico e Speedup

| Parâmetro | Valor |
|---|---|
| preprocess_ms | 5 |
| branch_durations_ms | 12,10,9,8,7 |
| aggregator_ms | 6 |
| t_chain_sum_ms | 57 |
| t_graph_critical_ms | 23 |
| speedup_chain_over_graph | 2,48 |

Interpretação: No chain, o tempo segue a soma das etapas; no grafo, o makespan aproxima-se do máximo dos ramos paralelos mais o custo de agregação. O speedup teórico sustenta a diferença observada.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 24 -mode c
./Scripts/run.ps1 -chapter 24 -mode g
./Scripts/run.ps1 -chapter 24 -mode b
```

- Bash
```bash
bash Scripts/run.sh 24 c
bash Scripts/run.sh 24 g
bash Scripts/run.sh 24 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap24_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap24_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (topologias/makespan): `src/Benchmark/results/cap24_theory_topologies-makespan-summary.json`, `.md`

