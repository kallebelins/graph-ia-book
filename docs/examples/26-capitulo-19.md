### Capítulo 19 — Grafos, Autômatos e Linguagens Formais

Este exemplo acompanha o capítulo em `docs/book/26-capitulo-19.md`.

- Chain (SK): simula um AFND sobre um DAG rotulado e calcula tamanho do AFD (via determinização) de forma sequencial.
- Graph (SKG): executa aceitação e tamanho do AFD em ramos paralelos e faz merge determinístico.

Artefatos gerados (via runner):

- Resultados de latência: `src/Benchmark/results/cap19_chain_latency-summary.{json,md}` e `src/Benchmark/results/cap19_graph_latency-summary.{json,md}`
- Benchmark A/B: `src/Benchmark/results/cap19_benchmark_latency-ab-summary.{json,md}`
- Teoria (regularidade, exemplos de aceitação e tamanho do AFD): `src/Benchmark/results/cap19_theory_regularity-nfa-dfa-summary.{json,md}`

Execução sugerida:

```bash
dotnet run -c Release --project graph-ia-book/src -- -c 19 -m benchmark
```


