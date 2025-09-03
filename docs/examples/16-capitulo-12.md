## Capítulo 12 — Agentes Autônomos com Grafos

Este exemplo compara um pipeline SK (chain) rígido com um grafo SKG com roteamento dinâmico
no cenário de um agente de suporte que pode buscar respostas na FAQ, consultar código ou
escalar para humano.

### Execução

- Chain: `cap12/chain/agent-demo` e `cap12/chain/latency`
- Graph: `cap12/graph/agent-demo` e `cap12/graph/latency`
- Benchmark A/B: `cap12/benchmark/latency-ab`
- Teoria: `cap12/theory/autonomy`

Arquivos de resultados (JSON/Markdown) são gerados em `src/Benchmark/results/`.

### Observações

- Chain usa decisão imperativa (if/else) mas mantém fluxo fixo.
- Grafo incorpora a decisão na própria topologia, alterando o caminho percorrido.
- Métricas incluem média, p95/p99 e comparação A/B de latência.


