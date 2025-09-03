# Relatório Capítulo 25 — (Tópico a definir)

Este relatório consolida o A/B do Capítulo 25. Observa-se que, neste cenário específico, a versão em grafo apresenta maior latência média, sugerindo overhead de coordenação maior que os ganhos para a topologia escolhida.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap25_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap25_benchmark_latency-ab_graph_latency-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 131 | 128 | 247 |
| Graph (SKG) | 30 | 160 | 162 | 216 |

Interpretação: A média do grafo é maior, e embora o p99 seja menor que o do chain, o custo médio indica que a coordenação/overheads do grafo superam os ganhos de paralelismo para este caso. Diretriz: preferir chain quando a topologia é simples e o ganho esperado (G) é menor que o custo (C).

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 25 -mode c
./Scripts/run.ps1 -chapter 25 -mode g
./Scripts/run.ps1 -chapter 25 -mode b
```

- Bash
```bash
bash Scripts/run.sh 25 c
bash Scripts/run.sh 25 g
bash Scripts/run.sh 25 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap25_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap25_benchmark_latency-ab_graph_latency-summary.json`, `.md`

