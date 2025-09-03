# Relatório Capítulo 17 — Execução por Ordem Topológica e Caminho Crítico

Este relatório consolida os resultados do Capítulo 17, comparando cadeia (SK) e grafo (SKG) com foco em scheduling por ordem topológica e impacto do caminho crítico no makespan.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap17_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap17_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap17_theory_scheduling-critical-path-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 607 | 626 | 678 |
| Graph (SKG) | 30 | 383 | 390 | 475 |

O grafo reduz o makespan ao explorar paralelismo e minimizar o caminho crítico, enquanto o chain acumula somas sequenciais.

### Teoria: Caminho Crítico

| Parâmetro | Valor |
|---|---|
| relation | T_chain = sum; T_graph = max over paths (critical path) |
| t_chain_sum_ms | 550 |
| t_graph_critical_ms | 360 |
| tA_ms | 100 |
| tB_ms | 130 |
| tC_ms | 90 |
| tD_ms | 110 |
| tE_ms | 70 |
| tMerge_ms | 50 |

Interpretação: O makespan do grafo é determinado pelo maior caminho (crítico) mais o custo do merge; isso explica a melhora observada nos percentis e na média.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 17 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 17 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 17 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 17 c
bash Scripts/run.sh 17 g
bash Scripts/run.sh 17 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap17_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap17_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (caminho crítico): `src/Benchmark/results/cap17_theory_scheduling-critical-path-summary.json`, `.md`

