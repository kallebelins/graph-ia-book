# Relatório Capítulo 10 — Diamante (Chain vs Grafo)

Este relatório consolida os resultados do Capítulo 10, comparando o padrão Diamante implementado em cadeia (SK) e em grafo (SKG). Inclui A/B de latência e validação teórica de makespan (soma vs máximo + merge).

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap10_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap10_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap10_theory_diamond-makespan-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 209 | 211 | 277 |
| Graph (SKG) | 30 | 144 | 145 | 188 |

O grafo apresenta ganho significativo, consistente com a execução paralela dos ramos do diamante e merge determinístico.

### Teoria: Makespan do Diamante

| Parâmetro | Valor |
|---|---|
| branch_a_ms | 90 |
| branch_b_ms | 60 |
| merge_ms | 40 |
| t_chain_sum_ms | 190 |
| t_graph_max_ms | 130 |
| speedup_chain_over_graph | 1,46 |

Interpretação: No chain, o tempo total é aproximadamente a soma das etapas (ramos em série). No grafo, o makespan aproxima-se do máximo entre os ramos paralelos mais o custo de merge, explicando a diferença de latência observada.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 10 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 10 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 10 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 10 c
bash Scripts/run.sh 10 g
bash Scripts/run.sh 10 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap10_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap10_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (diamante): `src/Benchmark/results/cap10_theory_diamond-makespan-summary.json`, `.md`

