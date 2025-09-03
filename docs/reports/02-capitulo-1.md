# Relatório Capítulo 1 — Problema da Complexidade

Este relatório consolida os resultados do Capítulo 1, comparando execução em cadeia (SK) e em grafo (SKG) e relacionando com a teoria de makespan e caminho crítico.

## Sumário de Métricas

- Fonte dos dados:
  - `src/Benchmark/results/chapter1_benchmark_latency-vs-cost-summary.json|.md`
  - `src/Benchmark/results/chapter1_benchmark_latency-vs-cost_chain_latency-summary.json|.md`
  - `src/Benchmark/results/chapter1_benchmark_latency-vs-cost_graph_latency-summary.json|.md`
  - `src/Benchmark/results/chapter1_theory_makespan-summary.json|.md`

### Benchmark A/B: Latência vs Custo (50 iterações)

| Métrica | Valor |
|---|---|
| Iterações | 50 |
| Média (ms) | 74 |
| p95 (ms) | 84 |
| p99 (ms) | 90 |

### Detalhes por modo

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 58 | 66 | 155 |
| Graph (SKG) | 30 | 61 | 83 | 123 |

Observa-se que as médias são comparáveis no cenário simulado, com p99 menor no grafo, sugerindo maior resiliência a picos em fan-out/fan-in.

### Teoria: Makespan e Caminho Crítico

| Parâmetro | Valor |
|---|---|
| isAcyclic | true |
| ordem_topológica | `merge -> preprocess -> reason -> retrieve -> start -> verify` |
| soma_chain_ms | 43 |
| soma_graph_ms | 42 |
| ramo_paralelo_graph_ms | 20 |
| limite_Brent_ms | 42 |

Interpretação: o limite de Brent para este grafo é 42 ms, alinhando-se ao makespan observado no caminho crítico. A soma sequencial (chain) é maior (43 ms). Em geral, para ramos independentes, o makespan do grafo tende a `max` dos ramos mais overhead de merge, enquanto o chain soma tempos.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 1 -mode b
```

- Bash
```bash
bash Scripts/run.sh 1 b
```

## Referências de Saída

- A/B: `src/Benchmark/results/chapter1_benchmark_latency-vs-cost-summary.json`, `.md`
- Chain: `src/Benchmark/results/chapter1_benchmark_latency-vs-cost_chain_latency-summary.json`, `.md`
- Graph: `src/Benchmark/results/chapter1_benchmark_latency-vs-cost_graph_latency-summary.json`, `.md`
- Teoria: `src/Benchmark/results/chapter1_theory_makespan-summary.json`, `.md`


