## Capítulo 7 — Recuperação e Resiliência

Como executar:

- PowerShell: .\\src\\Scripts\\run.ps1 -chapter 7 -mode b
- Bash: bash src/Scripts/run.sh 7 b

Saídas (exemplos):

- src/Benchmark/results/cap7_benchmark_latency-p95-p99-summary.json ([abrir JSON](../../src/Benchmark/results/cap7_benchmark_latency-p95-p99-summary.json))
- src/Benchmark/results/cap7_benchmark_latency-p95-p99-summary.md ([abrir Markdown](../../src/Benchmark/results/cap7_benchmark_latency-p95-p99-summary.md))
- src/Benchmark/results/cap7_benchmark_success-rate-summary.json ([abrir JSON](../../src/Benchmark/results/cap7_benchmark_success-rate-summary.json))
- src/Benchmark/results/cap7_benchmark_success-rate-summary.md ([abrir Markdown](../../src/Benchmark/results/cap7_benchmark_success-rate-summary.md))
- src/Benchmark/results/cap7_benchmark_success-theory-summary.json ([abrir JSON](../../src/Benchmark/results/cap7_benchmark_success-theory-summary.json))
- src/Benchmark/results/cap7_benchmark_success-theory-summary.md ([abrir Markdown](../../src/Benchmark/results/cap7_benchmark_success-theory-summary.md))

Notas:

- Chain: falha no estágio 2 interrompe o fluxo
- Graph: fallback (v2 → v2') com checkpoints e reexecução parcial

### Como interpretar as métricas

- Média (ms): custo médio considerando reexecução parcial/alternativa.
- p95/p99 (ms): impacto da cauda sob falhas; fallback tende a reduzir erros fatais e estabilizar a cauda.
- Sucesso: comparar taxa de sucesso efetiva entre chain (abort) e graph (fallback).
- Teórico vs medido: sucesso composto \(1 - \prod (1 - p_i)\); compare com a taxa medida para validar o modelo de fallback.

