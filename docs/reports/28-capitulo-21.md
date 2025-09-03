# Relatório Capítulo 21 — Processos Estocásticos em Grafos (Markov, Estados Absorventes)

Este relatório consolida resultados do Capítulo 21, comparando cadeia (SK) e grafo (SKG) em um cenário modelado por cadeias de Markov com estados absorventes. Inclui A/B de latência e validação teórica via matriz fundamental N.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap21_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap21_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap21_theory_markov-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 5 | 1 | 112 |
| Graph (SKG) | 30 | 4 | 1 | 84 |

Observa-se cauda p99 menor no grafo, consistente com coordenação paralela e agregação determinística.

### Teoria: Cadeia de Markov com Estados Absorventes

| Parâmetro | Valor |
|---|---|
| N=(I−Q)^{-1} | calculada |
| t=N·1 | tempos esperados em estados transitórios |
| B=N·R | probabilidades de absorção |
| Nota | estados transitórios S,A,B e F absorvente |

Interpretação: A matriz fundamental N e as derivadas t e B permitem estimar tempo esperado até absorção e probabilidades de absorção, alinhando a análise ao comportamento agregado de execuções em grafo vs chain.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 21 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 21 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 21 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 21 c
bash Scripts/run.sh 21 g
bash Scripts/run.sh 21 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap21_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap21_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (Markov): `src/Benchmark/results/cap21_theory_markov-summary.json`, `.md`

