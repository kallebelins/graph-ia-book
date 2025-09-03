## Capítulo 24 — Topologias e Anti-padrões

Como executar:

- PowerShell: .\\src\\Scripts\\run.ps1 -chapter 24 -mode b
- Bash: bash src/Scripts/run.sh 24 b

Saídas (exemplos):

- src/Benchmark/results/cap24_chain_latency-summary.json ([abrir JSON](../../src/Benchmark/results/cap24_chain_latency-summary.json))
- src/Benchmark/results/cap24_chain_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/cap24_chain_latency-summary.md))
- src/Benchmark/results/cap24_graph_latency-summary.json ([abrir JSON](../../src/Benchmark/results/cap24_graph_latency-summary.json))
- src/Benchmark/results/cap24_graph_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/cap24_graph_latency-summary.md))
- src/Benchmark/results/cap24_benchmark_latency-ab-summary.json ([abrir JSON](../../src/Benchmark/results/cap24_benchmark_latency-ab-summary.json))
- src/Benchmark/results/cap24_benchmark_latency-ab-summary.md ([abrir Markdown](../../src/Benchmark/results/cap24_benchmark_latency-ab-summary.md))
- src/Benchmark/results/cap24_theory_topologies-makespan-summary.json ([abrir JSON](../../src/Benchmark/results/cap24_theory_topologies-makespan-summary.json))
- src/Benchmark/results/cap24_theory_topologies-makespan-summary.md ([abrir Markdown](../../src/Benchmark/results/cap24_theory_topologies-makespan-summary.md))

Notas:

- Chain: fan-out lógico de 5 ramos executado sequencialmente, seguido por agregação determinística.
- Graph: os mesmos 5 ramos executados em paralelo, com merge determinístico.

### Como interpretar as métricas

- Média/p95/p99 (ms): custo e variabilidade; o grafo tende a menor makespan.
- Teoria (makespan): T_chain = preprocess + Σ ramos + aggregator; T_graph ≈ preprocess + max(ramos) + aggregator.


