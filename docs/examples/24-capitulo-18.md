## Capítulo 18 — Álgebra de Grafos: Matrizes de Adjacência e Incidência

Este exemplo implementa a análise algébrica do capítulo (docs/book/24-capitulo-18.md) de duas formas:

- Chain (SK): pipeline sequencial que constrói A, calcula graus, alcançabilidade (A^2) e aciclicidade.
- Graph (SKG): grafo com três ramos paralelos (graus, alcançabilidade, aciclicidade) e merge determinístico.

Resultados e artefatos gerados (ver diretório `src/Benchmark/results`):

- cap18/chain/algebra
- cap18/graph/algebra
- cap18/chain/latency
- cap18/graph/latency
- cap18/benchmark/latency-ab
- cap18/theory/adjacency-incidence (JSON e Markdown)

Observações:

- Para o DAG em diamante, A^2[1,4] = 2 indica duas trilhas de comprimento 2.
- Graus: out(v1)=2, in(v4)=2 apontam v4 como agregador/gargalo.
- Acyclicidade confirmada pela ausência de entradas diagonais não nulas em A^ℓ para ℓ < n.


