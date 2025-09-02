## Capítulo 6 — Escalabilidade e Concorrência

Como executar:

- PowerShell: .\\src\\Scripts\\run.ps1 -chapter 6 -mode b
- Bash: bash src/Scripts/run.sh 6 b

Saídas (exemplos):

- src/Benchmark/results/cap6_chain_latency-summary.json ([abrir JSON](../../src/Benchmark/results/cap6_chain_latency-summary.json))
- src/Benchmark/results/cap6_chain_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/cap6_chain_latency-summary.md))
- src/Benchmark/results/cap6_graph_latency-summary.json ([abrir JSON](../../src/Benchmark/results/cap6_graph_latency-summary.json))
- src/Benchmark/results/cap6_graph_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/cap6_graph_latency-summary.md))
- src/Benchmark/results/cap6_benchmark_latency-ab-summary.json ([abrir JSON](../../src/src/Benchmark/results/cap6_benchmark_latency-ab-summary.json))
- src/Benchmark/results/cap6_benchmark_latency-ab-summary.md ([abrir Markdown](../../src/Benchmark/results/cap6_benchmark_latency-ab-summary.md))
- src/Benchmark/results/cap6_theory_critical-path-summary.json ([abrir JSON](../../src/Benchmark/results/cap6_theory_critical-path-summary.json))
- src/Benchmark/results/cap6_theory_critical-path-summary.md ([abrir Markdown](../../src/Benchmark/results/cap6_theory_critical-path-summary.md))

Notas:

- Chain: fan-out lógico de 5 ramos executado sequencialmente, seguido por agregação.
- Graph: os mesmos 5 ramos executados em paralelo, com merge determinístico.

### Como interpretar as métricas

- **Média (ms)**: custo médio do fluxo.
- **p95/p99 (ms)**: variabilidade; o grafo tende a apresentar menor makespan.
- **Teoria (caminho crítico)**: tempo no grafo é limitado por preprocess + max(ramos) + agregador, enquanto no chain é a soma de todos os ramos.
