# Relatório Capítulo 18 — Álgebra de Grafos: Matrizes de Adjacência e Incidência

Este relatório consolida resultados do Capítulo 18, comparando latência entre cadeia (SK) e grafo (SKG) em análises estruturais (graus, alcançabilidade, aciclicidade) e registrando a validação teórica via matrizes de adjacência e suas potências.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap18_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap18_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap18_theory_adjacency-incidence-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 4 | 1 | 101 |
| Graph (SKG) | 30 | 3 | 1 | 71 |

Os tempos são baixos (análises locais). A abordagem em grafo apresenta cauda p99 menor, refletindo coordenação ligeiramente mais eficiente em casos com checagens paralelizáveis.

### Teoria: Adjacência, A^2 e Aciclicidade

| Parâmetro | Valor |
|---|---|
| acyclic | True |
| reachable_1_to_4 | True |
| notes | A^2[1,4]=2 indica duas trilhas de comprimento 2; ausência de ciclos detectáveis em potências < n |

Interpretação: A matriz de adjacência A e suas potências (A^2, ...) permitem inferir alcançabilidade e contagem de trilhas. A ausência de entradas de auto-laço nas potências iniciais sugere aciclicidade (DAG), coerente com a execução por ordem topológica.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 18 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 18 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 18 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 18 c
bash Scripts/run.sh 18 g
bash Scripts/run.sh 18 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap18_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap18_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (adjacência/incidência): `src/Benchmark/results/cap18_theory_adjacency-incidence-summary.json`, `.md`

