# Relatório 00e — Resultados e Métricas

Este relatório consolida os resultados do capítulo introdutório (00e), onde medimos latência de um fluxo simples e ilustramos o limite por caminho crítico (Brent) para paralelismo com dois ramos.

## Sumário de Métricas

- Fonte dos dados:
  - `src/Benchmark/results/00e_benchmark_latency-summary.json|.md`
  - `src/Benchmark/results/00e_benchmark_brent-theory-summary.json|.md`

### Latência (50 iterações)

| Métrica | Valor (ms) |
|---|---|
| Média | 9 |
| p95 | 16 |
| p99 | 17 |

### Teoria (Brent / caminho crítico)

| Parâmetro | Valor |
|---|---|
| t_stage1_ms | 10 |
| t_A_ms | 6 |
| t_B_ms | 6 |
| t_seq_ms | 22 |
| t_parallel_ms | 16 |
| processors_p | 2 |
| brent_lower_bound_ms | 16 |
| speedup_ideal | 1.375 |

## Interpretação

- A média de 9 ms, com p95 em 16 ms e p99 em 17 ms, indica baixa variabilidade sob a carga simulada.
- O modelo teórico (Brent) estimou makespan paralelo de 16 ms (igual ao bound), contra 22 ms no sequencial, com speedup ideal de ~1,375×. Isso corrobora a tese de que, mesmo em exemplos mínimos, o paralelismo reduz o tempo total quando há ramos independentes, limitado pelo caminho crítico e overheads de coordenação.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 0 -mode b
```

- Bash
```bash
bash Scripts/run.sh 0 b
```

## Referências de Saída

- Latência: `src/Benchmark/results/00e_benchmark_latency-summary.json`, `src/Benchmark/results/00e_benchmark_latency-summary.md`
- Teoria (Brent): `src/Benchmark/results/00e_benchmark_brent-theory-summary.json`, `src/Benchmark/results/00e_benchmark_brent-theory-summary.md`
