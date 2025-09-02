## Capítulo 9 — Governança e Segurança

Como executar:

- PowerShell: .\src\Scripts\run.ps1 -chapter 9 -mode b
- Bash: bash src/Scripts/run.sh 9 b

Saídas (exemplos):

- src/Benchmark/results/cap9_chain_latency-summary.json ([abrir JSON](../../src/Benchmark/results/cap9_chain_latency-summary.json))
- src/Benchmark/results/cap9_chain_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/cap9_chain_latency-summary.md))
- src/Benchmark/results/cap9_graph_latency-summary.json ([abrir JSON](../../src/Benchmark/results/cap9_graph_latency-summary.json))
- src/Benchmark/results/cap9_graph_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/cap9_graph_latency-summary.md))
- src/Benchmark/results/cap9_benchmark_latency-ab-summary.json ([abrir JSON](../../src/Benchmark/results/cap9_benchmark_latency-ab-summary.json))
- src/Benchmark/results/cap9_benchmark_latency-ab-summary.md ([abrir Markdown](../../src/Benchmark/results/cap9_benchmark_latency-ab-summary.md))
- src/Benchmark/results/cap9_theory_governance-summary.json ([abrir JSON](../../src/Benchmark/results/cap9_theory_governance-summary.json))
- src/Benchmark/results/cap9_theory_governance-summary.md ([abrir Markdown](../../src/Benchmark/results/cap9_theory_governance-summary.md))

Notas:

- Chain: avaliação de política bloqueia a saída quando há violação (não há desvio seguro).
- Graph: PolicyGuard roteia para Anonymizer quando necessário e segue para processamento seguro.

### Como interpretar as métricas

- Média/p95/p99 (ms): impacto do desvio seguro vs bloqueio.
- Teoria: chain tem 2 pontos de monitoramento; graph possui políticas por nó e suporte a roteamento seguro.


