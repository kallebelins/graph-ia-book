## 00e — Introdução: Chain mínimo vs Graph paralelo

Resultados gerados pelo runner (src/book.csproj):

- Executar: PowerShell: .\\src\\Scripts\\run.ps1 -chapter 0 -mode b
- Executar: Bash: bash src/Scripts/run.sh 0 b
- Saídas de resumo: src/Benchmark/results/00e_* (*-summary.json e *-summary.md)

Arquivos de resultado (exemplos):

- src/Benchmark/results/00e_benchmark_latency-summary.json ([abrir JSON](../../src/Benchmark/results/00e_benchmark_latency-summary.json))
- src/Benchmark/results/00e_benchmark_latency-summary.md ([abrir Markdown](../../src/Benchmark/results/00e_benchmark_latency-summary.md))

Observações:

- Chain: pipeline mínimo (entrada → LLM → resposta)
- Graph: dois ramos paralelos (sumarização e extração) com merge

### Como interpretar as métricas

- Média (ms): tempo médio por execução.
- p95/p99 (ms): cauda de latência; 95%/99% das execuções ficam abaixo desses valores.
- Makespan: duração do caminho crítico; deve ser menor no grafo quando há paralelismo útil.

