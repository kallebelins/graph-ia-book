# Relatório Capítulo 13 — Limitações da Abordagem em Grafos

Este relatório consolida resultados do Capítulo 13, destacando casos em que a abordagem em grafo não traz benefícios significativos frente a chains simples, e quantificando overheads.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap13_benchmark_chain-latency-summary.json|.md`
  - `src/Benchmark/results/cap13_benchmark_graph-latency-summary.json|.md`
  - `src/Benchmark/results/cap13_benchmark_gain-vs-cost-summary.json|.md`
  - `src/Benchmark/results/cap13_benchmark_latency-p95-p99-summary.json|.md`

### Chain vs Graph — Latência (50 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 50 | 46 | 48 | 48 |
| Graph (SKG) | 50 | 45 | 48 | 48 |

Observação: As latências são praticamente equivalentes neste cenário, evidenciando que o grafo não necessariamente traz ganhos quando a coordenação/ramificação é mínima e o problema é linear.

### Ganho vs Custo — Indicadores

| Parâmetro | Valor |
|---|---|
| chain_cost_ms | 3 |
| graph_base_ms | 3 |
| mem_before_bytes | 806984 |
| mem_after_bytes | 1224248 |
| mem_delta_bytes | 417264 |

Interpretação: O aumento de memória e a equivalência de custo base indicam que, para este caso, a sobrecarga de coordenação do grafo não se traduz em benefícios materiais. Critérios de decisão devem considerar um limiar G < C (ganho menor que custo) para optar por chain.

### Latência Agregada (contexto)

| Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---:|---:|---:|---:|
| 50 | 14 | 16 | 17 |

Nota: métrica agregada de latência do cenário simplificado; auxilia na comparação qualitativa entre arranjos com pouca coordenação.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 13 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 13 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 13 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 13 c
bash Scripts/run.sh 13 g
bash Scripts/run.sh 13 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap13_benchmark_chain-latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap13_benchmark_graph-latency-summary.json`, `.md`
- Ganho vs Custo: `src/Benchmark/results/cap13_benchmark_gain-vs-cost-summary.json`, `.md`
- Latência agregada: `src/Benchmark/results/cap13_benchmark_latency-p95-p99-summary.json`, `.md`

