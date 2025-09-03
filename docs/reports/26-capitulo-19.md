# Relatório Capítulo 19 — Grafos, Autômatos e Linguagens Formais

Este relatório consolida os resultados do Capítulo 19, comparando cadeia (SK) e grafo (SKG) em um cenário de aceitação de strings por AFND/AFD induzido por DAG rotulado. Inclui A/B de latência e validação teórica de regularidade.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap19_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap19_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap19_theory_regularity-nfa-dfa-summary.json|.md`

### Benchmark A/B: Latência (20 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 20 | 5 | 6 | 88 |
| Graph (SKG) | 20 | 3 | 5 | 56 |

O grafo reduz a cauda e a média, refletindo paralelização/compartilhamento de caminhos no DAG rotulado.

### Teoria: Regularidade em DAGs

| Parâmetro | Valor |
|---|---|
| alphabet | abc |
| accepts_a | False |
| accepts_ac | True |
| accepts_bc | True |
| dfa_reachable_states | 4 |
| thesis | Finite labeled DAG induces a regular language via NFA; determinization yields DFA |

Interpretação: Um DAG rotulado finito induz uma linguagem regular via AFND; a determinização produz um AFD finito, consistente com os exemplos de aceitação.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 19 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 19 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 19 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 19 c
bash Scripts/run.sh 19 g
bash Scripts/run.sh 19 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap19_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap19_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (regularidade): `src/Benchmark/results/cap19_theory_regularity-nfa-dfa-summary.json`, `.md`

