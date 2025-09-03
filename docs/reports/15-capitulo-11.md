# Relatório Capítulo 11 — Aplicações Demonstrativas (Turismo, Finanças, Saúde)

Este relatório consolida benchmarks e validações teóricas de três aplicações demonstrativas em cadeia (SK) e grafo (SKG): Turismo, Finanças e Saúde. Comparamos latência média e cauda (p95/p99) e sumarizamos notas teóricas por domínio.

## Sumário de Métricas

- Fontes dos dados:
  - Turismo: `src/Benchmark/results/cap11_benchmark_tourism_chain_latency-summary.json|.md`, `cap11_benchmark_tourism_graph_latency-summary.json|.md`, teoria `cap11_theory_tourism-summary.json|.md`
  - Finanças: `src/Benchmark/results/cap11_benchmark_finance_chain_latency-summary.json|.md`, `cap11_benchmark_finance_graph_latency-summary.json|.md`, teoria `cap11_theory_finance-summary.json|.md`
  - Saúde: `src/Benchmark/results/cap11_benchmark_health_chain_latency-summary.json|.md`, `cap11_benchmark_health_graph_latency-summary.json|.md`, teoria `cap11_theory_health-summary.json|.md`

### Turismo — Benchmark A/B (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 81 | 80 | 160 |
| Graph (SKG) | 30 | 33 | 33 | 81 |

Observação: O grafo reduz média e cauda, com p99 substancialmente menor. O chain apresenta picos ocasionais (p99).

### Finanças — Benchmark A/B (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 79 | 80 | 82 |
| Graph (SKG) | 30 | 31 | 32 | 32 |

Observação: Ganho expressivo do grafo em média e caudas, consistente com paralelização de subetapas e fusão determinística.

### Saúde — Benchmark A/B (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 79 | 81 | 94 |
| Graph (SKG) | 30 | 47 | 53 | 56 |

Observação: O grafo melhora média e p95/p99, mantendo caudas mais controladas.

### Teoria por domínio

| Domínio | chain_ms_sum | graph_ms_sum | graph_parallel_branch_ms | isAcyclic |
|---|---:|---:|---:|---|
| Turismo | 32 | 14 | 10 | True |
| Finanças | 27 | 12 | 8 | True |
| Saúde | 30 | 19 | 9 | True |

Interpretação: Os modelos teóricos indicam que em grafo o makespan aproxima-se do máximo das ramificações paralelas mais custos de fusão, enquanto no chain segue a soma sequencial. Isso explica os ganhos observados em todos os domínios.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 11 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 11 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 11 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 11 c
bash Scripts/run.sh 11 g
bash Scripts/run.sh 11 b
```

## Referências de Saída

- Turismo: `cap11_benchmark_tourism_chain_latency-summary.json|.md`, `cap11_benchmark_tourism_graph_latency-summary.json|.md`, teoria `cap11_theory_tourism-summary.json|.md`
- Finanças: `cap11_benchmark_finance_chain_latency-summary.json|.md`, `cap11_benchmark_finance_graph_latency-summary.json|.md`, teoria `cap11_theory_finance-summary.json|.md`
- Saúde: `cap11_benchmark_health_chain_latency-summary.json|.md`, `cap11_benchmark_health_graph_latency-summary.json|.md`, teoria `cap11_theory_health-summary.json|.md`

