# Relatório Capítulo 4 — Explosão de Estados e Modularidade

Este relatório consolida os resultados do Capítulo 4, comparando execução em cadeia (SK) e em grafo (SKG) e registrando a análise teórica de explosão de estados (k^n) versus convergência por reutilização de nós em grafos.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap4_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap4_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap4_theory_state-counts-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 75 | 109 | 148 |
| Graph (SKG) | 30 | 48 | 67 | 106 |

Observa-se vantagem consistente do grafo em média e p95, com redução também da cauda (p99). O nó compartilhado de tratamento e a convergência reduzem o tempo total frente ao chain com manuseio externo por estado.

### Teoria: Explosão de Estados vs Convergência

| Parâmetro | Valor |
|---|---|
| k (ramificações por estágio) | 3 |
| n (número de estágios) | 5 |
| limite_superior_chain (k^n) | 243 |
| nota_convergência_grafo | `shared handler reduces effective states by reuse/convergence` |

Interpretação: Em um chain com controle externo por estado, a combinação potencial de estados cresce como \(k^n\). Já em grafos com convergência e reutilização de nós (tratamento compartilhado), o número efetivo de estados visitáveis é significativamente menor, mitigando explosão combinatória e latências decorrentes.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 4 -mode b
```

- Bash
```bash
bash Scripts/run.sh 4 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap4_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap4_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (contagem de estados): `src/Benchmark/results/cap4_theory_state-counts-summary.json`, `.md`
