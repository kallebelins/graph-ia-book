## Capítulo 13 — Limitações da Abordagem em Grafos

Como executar:

- PowerShell: .\\src\\Scripts\\run.ps1 -chapter 13 -mode b
- Bash: bash src/Scripts/run.sh 13 b

Saídas (exemplos):

- src/Benchmark/results/cap13_benchmark_latency-p95-p99-summary.json ([abrir JSON](../../src/Benchmark/results/cap13_benchmark_latency-p95-p99-summary.json))
- src/Benchmark/results/cap13_benchmark_latency-p95-p99-summary.md ([abrir Markdown](../../src/Benchmark/results/cap13_benchmark_latency-p95-p99-summary.md))

Notas:

- Chain: CSV→JSON trivial
- Graph: ramos superficiais com sobrecarga de coordenação

### Como interpretar as métricas

- Média (ms): custo agregado do grafo pode exceder chain quando o paralelismo não traz ganho.
- p95/p99 (ms): observar se a coordenação aumenta a cauda.
- Decisão: aplicar a condição G < C do capítulo — só usar grafo quando o ganho superar o custo.

