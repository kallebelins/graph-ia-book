# Relatório Capítulo 26 — (Tópico a definir) Validação Piloto

Este relatório consolida resultados do Capítulo 26, incluindo A/B de latência e uma validação piloto com preditor estrutural para escolha de rota de menor latência.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap26_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap26_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap26_theory_pilot-validation-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 131 | 128 | 222 |
| Graph (SKG) | 30 | 160 | 161 | 198 |

Interpretação: O grafo tem maior média, ainda que o p99 seja menor. Neste piloto, a topologia/carga favoreceu a simplicidade do chain.

### Validação Piloto: Preditor de Rota

| Parâmetro | Valor |
|---|---|
| top1_accuracy | 1 |
| notes | Preditor baseado em features estruturais escolhe a rota de menor latência real no dataset sintético |

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 26 -mode c
./Scripts/run.ps1 -chapter 26 -mode g
./Scripts/run.ps1 -chapter 26 -mode b
```

- Bash
```bash
bash Scripts/run.sh 26 c
bash Scripts/run.sh 26 g
bash Scripts/run.sh 26 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap26_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap26_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Validação piloto: `src/Benchmark/results/cap26_theory_pilot-validation-summary.json`, `.md`

