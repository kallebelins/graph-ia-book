## Capítulo 3 — Orquestração de Sistemas Inteligentes

Como executar:

- PowerShell: .\\src\\Scripts\\run.ps1 -chapter 3 -mode b
- Bash: bash src/Scripts/run.sh 3 b

Saídas (exemplos):

- src/Benchmark/results/cap3_benchmark_latency-p95-p99-summary.json ([abrir JSON](../../src/Benchmark/results/cap3_benchmark_latency-p95-p99-summary.json))
- src/Benchmark/results/cap3_benchmark_latency-p95-p99-summary.md ([abrir Markdown](../../src/Benchmark/results/cap3_benchmark_latency-p95-p99-summary.md))

Notas:

- Chain: decisão condicional via if/switch externo
- Graph: roteamento condicional nativo e merge determinístico

### Como interpretar as métricas

- Média (ms): custo médio do fluxo.
- p95/p99 (ms): variabilidade introduzida pela decisão condicional.
- Comparar chain vs graph: o grafo tende a apresentar menor variância e melhor controle de merge.

