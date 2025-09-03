# Relatório Capítulo 16 — Teoremas de Expressividade: Chains ⊂ DAGs e Limites

Este relatório consolida os resultados do Capítulo 16, comparando latência entre cadeia (SK) e grafo (SKG) em um cenário com k módulos independentes. Inclui A/B e prova teórica de redução de tempo via paralelismo (DAG) e árvore de redução.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap16_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap16_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap16_theory_expressivity-proofs-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 308 | 335 | 399 |
| Graph (SKG) | 30 | 116 | 125 | 212 |

O grafo apresenta redução expressiva de latência média e cauda, compatível com execução paralela de módulos independentes e custo de merge controlado.

### Teoria: Expressividade e Redução (k módulos)

| Parâmetro | Valor |
|---|---|
| k_modules | 6 |
| t_module_ms | 60 |
| alpha_merge_ms | 40 |
| levels_log2_k | 3 |
| t_chain_sum_ms | 400 |
| t_dag_parallel_ms | 100 |
| t_reduce_tree_ms | 220 |
| relation | Chain: k*t+α; DAG: t+α; Reduce: ceil(log2 k)*t+α |

Interpretação: Em chains, o tempo cresce linearmente com k (k·t+α). Em DAGs, com paralelismo ideal, aproxima-se de t+α; com árvore de redução, ceil(log2 k)·t+α. Os valores teóricos explicam a melhora observada nos benchmarks.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 16 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 16 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 16 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 16 c
bash Scripts/run.sh 16 g
bash Scripts/run.sh 16 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap16_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap16_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (expressividade): `src/Benchmark/results/cap16_theory_expressivity-proofs-summary.json`, `.md`

