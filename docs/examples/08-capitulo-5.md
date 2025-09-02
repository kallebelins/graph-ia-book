## Capítulo 5 — Explicabilidade e Auditoria

Este exemplo complementa `docs/book/08-capitulo-5.md` e demonstra:

- Diferença entre chain (SK) sem trilha explícita e grafo (SKG) com rastreio estruturado.
- Como habilitar métricas e opcionalmente streaming de eventos para auditoria por nó/caminho.

### Como executar

- Chain: `GraphIABook.Chains.Chapter5.ChainChapter5.RunAsync(input)`
- Graph: `GraphIABook.Graphs.Chapter5.GraphChapter5.RunAsync(input)`
- Trace opcional (streaming): `GraphIABook.Graphs.Chapter5.GraphChapter5.RunWithTraceAsync(input)`

Pelo runner (`Program`), selecione o capítulo 5 e modo `c` (chain), `g` (graph) ou `b` (benchmark).

### Métricas geradas

- `src/Benchmark/results/cap5_chain_latency-summary.{json,md}`
- `src/Benchmark/results/cap5_graph_latency-summary.{json,md}`
- `src/Benchmark/results/cap5_benchmark_latency-ab_*`
- `src/Benchmark/results/cap5_theory_explainability-summary.{json,md}`

### Observações

- O grafo utiliza `ConditionalGraphNode` com expressão `{{ gte score 0.5 }}` para roteamento explícito.
- Em execução com streaming, eventos incluem: início, nó iniciado/concluído, `ConditionEvaluated` e fim.


