## Capítulo 11 — Aplicações Demonstrativas (Turismo, Finanças, Saúde)

Este exemplo compara pipelines lineares (CHAIN) com grafos (GRAPH) em três domínios práticos:

- Turismo (concierge)
- Finanças (detecção de fraude)
- Saúde (triagem)

Implementação de código:

- Chain: `graph-ia-book/src/chains/chapter11/ChainChapter11.cs`
- Graph: `graph-ia-book/src/graphs/chapter11/GraphChapter11.cs`
- Runner: `graph-ia-book/src/Chapters/Chapter11.cs`

Como executar (runner):

- PowerShell: `graph-ia-book/src/Scripts/run.ps1 --chapter 11 --mode c|g|b`
- Bash: `graph-ia-book/src/Scripts/run.sh --chapter 11 --mode c|g|b`

Resultados gerados (exemplos):

- Benchmarks CHAIN vs GRAPH:
  - `src/Benchmark/results/cap11/benchmark/tourism*`
  - `src/Benchmark/results/cap11/benchmark/finance*`
  - `src/Benchmark/results/cap11/benchmark/health*`
- Teoria/validação:
  - `src/Benchmark/results/cap11/theory/tourism.*`
  - `src/Benchmark/results/cap11/theory/finance.*`
  - `src/Benchmark/results/cap11/theory/health.*`

Resumo esperado:

- CHAIN: soma sequencial das etapas → maior latência e fragilidade.
- GRAPH: ramos paralelos + fusão determinística → menor latência e maior resiliência/multimodalidade.


