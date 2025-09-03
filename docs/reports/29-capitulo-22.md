# Relatório Capítulo 22 — Resiliência Probabilística e Fallback

Este relatório consolida resultados do Capítulo 22, comparando cadeia (SK) e grafo (SKG) sob fallback probabilístico. Inclui A/B de latência e validação teórica de p_total e tempo esperado sob OR com cancelamento.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap22_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap22_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap22_theory_fallback-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 5 | 1 | 112 |
| Graph (SKG) | 30 | 3 | 1 | 71 |

Cauda p99 menor no grafo, consistente com execuções OR-paralelas e cancelamento de ramos após primeiro sucesso.

### Teoria: p_total e E[T] (seq) vs OR (aprox.)

| Parâmetro | Valor |
|---|---|
| p_total | 0,88 |
| E[T]_seq_ms | 236 |
| E[T]_or_ms_approx | 94,4 |
| Nota | Seq: E[T]=t1+(1−p1)t2+(1−p1)(1−p2)t3; OR: p=1−∏(1−p_i) |

Interpretação: O arranjo OR-paralelo melhora tempo esperado e probabilidade de sucesso total (p_total), ao custo de sobrecarga de coordenação e potencial custo de cancelamento.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 22 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 22 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 22 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 22 c
bash Scripts/run.sh 22 g
bash Scripts/run.sh 22 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap22_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap22_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (fallback): `src/Benchmark/results/cap22_theory_fallback-summary.json`, `.md`

