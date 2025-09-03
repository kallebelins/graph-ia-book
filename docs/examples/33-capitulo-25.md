## Capítulo 25 — Orquestração vs. Graph Neural Networks (GNN)

Este exemplo demonstra a complementaridade entre orquestração (grafo de execução) e GNNs (grafos para aprendizado):

- Chain (baseline): seleção de rota por média histórica (heurística).
- Graph (SKG): preditor tipo GNN (features estruturais: grau, betweenness) informa a escolha de rota.
- Métricas: latência média, p95, p99 e A/B (baseline vs GNN-informed).

Artefatos:

- Código chain: `graph-ia-book/src/chains/chapter25/ChainChapter25.cs`
- Código graph: `graph-ia-book/src/graphs/chapter25/GraphChapter25.cs`
- Runner: `graph-ia-book/src/Chapters/Chapter25.cs`
- Resultados: `graph-ia-book/src/Benchmark/results/cap25_*`

Como executar:

```bash
dotnet run -c Debug --project graph-ia-book/src/book.csproj -- -c 25 -m chain
dotnet run -c Debug --project graph-ia-book/src/book.csproj -- -c 25 -m graph
dotnet run -c Debug --project graph-ia-book/src/book.csproj -- -c 25 -m benchmark
```

Notas:

- O baseline pode escolher uma rota que está pior no presente (drift), enquanto o preditor informado por features estruturais tende a capturar padrões mais estáveis.
- Este capítulo não treina uma GNN real; usa um preditor determinístico que imita a ideia de “modelo informa pesos/latências esperadas”.


