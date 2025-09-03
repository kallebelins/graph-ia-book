## Capítulo 23 — Métricas Estruturais para IA (diâmetro, centralidades, ciclomática)

- Chain: `GraphIABook.Chains.Chapter23.ChainChapter23.RunAsync`
- Graph: `GraphIABook.Graphs.Chapter23.GraphChapter23.RunAsync`

Resultados principais (arquivos gerados em `src/Benchmark/results/`):

- cap23/chain/metrics — execução sequencial (diâmetro, betweenness, M)
- cap23/graph/metrics — ramos paralelos (diâmetro, betweenness, M) com merge determinístico
- cap23/benchmark/latency-ab — comparação de latência média e p95/p99 (chain vs graph)
- cap23/theory/structural — comparação ilustrativa: 1 agregador vs 2 agregadores

Observação: este exemplo segue o capítulo do livro em `docs/book/31-capitulo-23.md`.


