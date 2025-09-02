## Capítulo 8 — Integração Multimodal e Híbrida

Este exemplo complementa `docs/book/11-capitulo-8.md` e demonstra:

- Limitação do chain (SK) voz-apenas versus grafo (SKG) multimodal voz+imagem
- Fusão tardia (late fusion) com pesos simples e métricas de latência

### Como executar

- Chain: `GraphIABook.Chains.Chapter8.ChainChapter8.RunAsync(input)`
- Graph: `GraphIABook.Graphs.Chapter8.GraphChapter8.RunAsync(input)`

Pelo runner (`Program`), selecione o capítulo 8 e modo `c` (chain), `g` (graph) ou `b` (benchmark).

### Métricas geradas

- `src/Benchmark/results/cap8_chain_latency-summary.{json,md}`
- `src/Benchmark/results/cap8_graph_latency-summary.{json,md}`
- `src/Benchmark/results/cap8_benchmark_latency-ab-summary.{json,md}`
- `src/Benchmark/results/cap8_theory_fusion-summary.{json,md}`

### Observações

- O chain ignora imagem por desenho, evidenciando duplicação/explosão de pipelines em cenários multimodais.
- O grafo contém subgrafos ASR→NLP (texto) e visão (imagem), convergindo em nó `Fusion` (fusão tardia).


