# Relatório Capítulo 3 — Orquestração (Decisão Condicional e Merge)

Este relatório consolida os resultados do Capítulo 3, comparando execução em cadeia (SK) e em grafo (SKG) em termos de latência sob decisão condicional e registrando a verificação de corretude do merge determinístico.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap3_benchmark_latency-p95-p99_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap3_benchmark_latency-p95-p99_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap3_benchmark_latency-p95-p99-summary.json|.md` (agregado)
  - `src/Benchmark/results/cap3_benchmark_merge-correctness-summary.json|.md`

### Benchmark A/B: Latência por Modo (50 iterações)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 50 | 45 | 52 | 54 |
| Graph (SKG) | 50 | 44 | 48 | 53 |

Observa-se leve vantagem do grafo em p95, com p99 semelhante, indicando estabilidade nas decisões condicionais com merge.

### Agregado de Latência (referência)

| Métrica | Valor (ms) |
|---|---|
| Iterações | 50 |
| Média | 13 |
| p95 | 19 |
| p99 | 20 |

Nota: o agregado resume um cenário sintético de referência e não substitui a comparação direta chain vs graph acima.

### Corretude do Merge Determinístico

| Parâmetro | Valor |
|---|---|
| Iterações | 20 |
| Média (ms) | 50 |
| p95 (ms) | 60 |
| p99 (ms) | 207 |

Interpretação: os tempos de merge permanecem baixos em média e p95; o p99 elevado está associado a picos ocasionais de caminhos mais longos/overheads simulados, sem violar a política de fusão determinística. As verificações de corretude do merge confirmam que a saída respeita a política definida, garantindo consistência.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 3 -mode b
```

- Bash
```bash
bash Scripts/run.sh 3 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap3_benchmark_latency-p95-p99_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap3_benchmark_latency-p95-p99_graph_latency-summary.json`, `.md`
- Agregado: `src/Benchmark/results/cap3_benchmark_latency-p95-p99-summary.json`, `.md`
- Merge (corretude): `src/Benchmark/results/cap3_benchmark_merge-correctness-summary.json`, `.md`
