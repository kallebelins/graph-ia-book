## Capítulo 21 — Processos Estocásticos em Grafos (Markov)

- Chain: `GraphIABook.Chains.Chapter21.ChainChapter21.RunAsync`
- Graph: `GraphIABook.Graphs.Chapter21.GraphChapter21.RunAsync`

Resultados principais (arquivos gerados em `src/Benchmark/results/`):

- cap21/chain/markov — execução sequencial, cálculo de N, t, B
- cap21/graph/markov — ramos paralelos (t e B), merge determinístico
- cap21/benchmark/latency-ab — comparação de latência média e p95/p99
- cap21/theory/markov — N=(I−Q)^{-1}, t=N·1, B=N·R para {S,A,B}→F

Observação: este exemplo segue o capítulo do livro em `docs/book/28-capitulo-21.md`.


