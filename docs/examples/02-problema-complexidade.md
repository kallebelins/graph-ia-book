## Capítulo 1 — Problema da Complexidade em IA

Como executar:

- PowerShell: .\\src\\Scripts\\run.ps1 -chapter 1 -mode b
- Bash: bash src/Scripts/run.sh 1 b

Saídas (exemplos):

- src/Benchmark/results/chapter1_benchmark_latency-vs-cost-summary.json ([abrir JSON](../../src/Benchmark/results/chapter1_benchmark_latency-vs-cost-summary.json))
- src/Benchmark/results/chapter1_benchmark_latency-vs-cost-summary.md ([abrir Markdown](../../src/Benchmark/results/chapter1_benchmark_latency-vs-cost-summary.md))

Notas:

- Chain: 4 estágios sequenciais (preprocess → retrieve → reason → answer)
- Graph: ramos independentes (retrieve paralelo + verificador) com merge determinístico

### Como interpretar as métricas

- Média (ms): soma sequencial (chain) vs caminho crítico + custo de agregação (graph).
- p95/p99 (ms): cauda reflete gargalos; paralelismo pode reduzir o makespan, mas custo de merge afeta cauda.
- Latency vs Cost: usar para evidenciar quando o grafo compensa o custo extra de coordenação.

