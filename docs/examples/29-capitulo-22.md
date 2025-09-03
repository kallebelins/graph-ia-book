## Capítulo 22 — Resiliência Probabilística e Fallback

- Chain: `GraphIABook.Chains.Chapter22.ChainChapter22.RunAsync`
- Graph: `GraphIABook.Graphs.Chapter22.GraphChapter22.RunAsync`

Resultados principais (arquivos gerados em `src/Benchmark/results/`):

- cap22/chain/fallback — execução sequencial (fallback), cálculo de p_total e E[T]
- cap22/graph/or — OR paralelo com merge determinístico, aproximação de E[T]
- cap22/benchmark/latency-ab — comparação de latência média e p95/p99 (chain vs graph)
- cap22/theory/fallback — p_total, E[T]_seq e E[T]_or (aprox.) conforme o texto

Observação: este exemplo segue o capítulo do livro em `docs/book/29-capitulo-22.md`.
