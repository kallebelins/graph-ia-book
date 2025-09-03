# Relatório Capítulo 6 — Escalabilidade e Concorrência

Este relatório consolida os resultados do Capítulo 6, comparando latência entre cadeia (SK) e grafo (SKG) em um cenário com fan-out de 5 ramos paralelos e agregação determinística. Inclui análise teórica de caminho crítico.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap6_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap6_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap6_theory_critical-path-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 108 | 119 | 225 |
| Graph (SKG) | 30 | 45 | 55 | 109 |

O grafo apresenta melhora substancial em média e percentis, como esperado em fan-out/fan-in com ramos independentes.

### Teoria: Caminho Crítico e Speedup

| Parâmetro | Valor |
|---|---|
| preprocess_ms | 5 |
| branch_durations_ms | `12,10,9,8,7` |
| aggregator_ms | 6 |
| t_chain_sum_ms | 57 |
| t_graph_critical_ms | 23 |
| speedup_chain_over_graph | `2,48` |

Interpretação: No chain, o tempo segue a soma das etapas; no grafo, o makespan aproxima-se do máximo dos ramos paralelos mais o custo de agregação (caminho crítico). O speedup observado (≈2,48×) é consistente com a diferença entre soma sequencial e caminho crítico do DAG.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 6 -mode b
```

- Bash
```bash
bash Scripts/run.sh 6 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap6_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap6_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (caminho crítico): `src/Benchmark/results/cap6_theory_critical-path-summary.json`, `.md`
