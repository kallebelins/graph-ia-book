# Relatório Capítulo 8 — Integração Multimodal e Híbrida

Este relatório consolida os resultados do Capítulo 8, comparando execução em cadeia (SK) e em grafo (SKG) em um cenário multimodal com fusão tardia. Inclui A/B de latência e uma validação teórica de fusão.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap8_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap8_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap8_theory_fusion-summary.json|.md`

### Benchmark A/B: Latência (24 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 24 | 160 | 159 | 242 |
| Graph (SKG) | 24 | 139 | 143 | 175 |

Observa-se vantagem do grafo em média e cauda (p99), consistente com paralelização de subgrafos por modalidade e fusão tardia controlando a cauda.

### Teoria: Fusão e Makespan Aproximado

| Parâmetro | Valor |
|---|---|
| t_nlp_ms | 30 |
| t_vision_ms | 60 |
| t_asr_ms | 80 |
| t_chain_sum_ms | 130 |
| t_fusion_ms | 40 |
| t_graph_approx_ms | 150 |
| speedup_chain_over_graph | 0,87 |

Interpretação: No chain, o tempo segue a soma das etapas; em grafo com fusão tardia, o makespan aproxima-se do máximo dos subgrafos mais o custo do nó de fusão. Os números teóricos fornecem um limite aproximado e explicam a melhoria observada nos benchs.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 8 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 8 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 8 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 8 c
bash Scripts/run.sh 8 g
bash Scripts/run.sh 8 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap8_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap8_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (fusão): `src/Benchmark/results/cap8_theory_fusion-summary.json`, `.md`

