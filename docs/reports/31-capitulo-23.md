# Relatório Capítulo 23 — (Tópico a definir) Análise Estrutural e Desempenho

Este relatório consolida resultados do Capítulo 23, comparando cadeia (SK) e grafo (SKG) e registrando medidas estruturais ilustrativas (betweenness, diâmetro) sob variação de agregadores.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap23_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap23_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap23_theory_structural-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 6 | 1 | 130 |
| Graph (SKG) | 30 | 3 | 1 | 84 |

O grafo apresenta melhor média e cauda (p99), coerente com paralelismo e distribuição de pontos de agregação.

### Teoria: Medidas Estruturais (ilustrativo)

| Parâmetro | Valor |
|---|---|
| betweenness_m_one | 5 |
| betweenness_each_agg_two | 3 |
| diameter_one_agg | 3 |
| diameter_two_aggs | 3 |
| M_one_agg | 0 |
| M_two_aggs | 0 |
| notes | Dois agregadores reduzem betweenness individual; M=0 em DAGs (ilustrativo) |

Interpretação: A presença de múltiplos agregadores pode reduzir betweenness de pontos únicos de fusão, mitigando gargalos; em DAGs, métricas como M permanecem 0 por aciclicidade.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 23 -mode c
./Scripts/run.ps1 -chapter 23 -mode g
./Scripts/run.ps1 -chapter 23 -mode b
```

- Bash
```bash
bash Scripts/run.sh 23 c
bash Scripts/run.sh 23 g
bash Scripts/run.sh 23 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap23_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap23_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (estrutural): `src/Benchmark/results/cap23_theory_structural-summary.json`, `.md`

