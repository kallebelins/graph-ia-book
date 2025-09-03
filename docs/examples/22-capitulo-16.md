### Capítulo 16 — Teoremas de Expressividade: Chains ⊂ DAGs e Limites

Este exemplo acompanha o texto em `docs/book/22-capitulo-16.md`.

- Chain: k módulos independentes executados em série (serialização forçada) + merge determinístico.
- Graph: k módulos independentes executados em paralelo + merge determinístico (opcional: árvore de redução).

Execução (runner):

```bash
# Chain
dotnet run --project src/book.csproj -- -c 16 -m c
# Graph
dotnet run --project src/book.csproj -- -c 16 -m g
# Benchmark (comparação T_chain vs T_DAG e teoria)
dotnet run --project src/book.csproj -- -c 16 -m b
```

Resultados esperados (arquivos gerados):
- `src/Benchmark/results/cap16_chain_latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap16_graph_latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap16_benchmark_latency-ab-summary.{json,md}`
- `src/Benchmark/results/cap16_theory_expressivity-proofs-summary.{json,md}`

Relações teóricas refletidas:
- Chain: T_chain = k·t + α (serialização dos k módulos)
- DAG: T_DAG ≈ t + α (paralelo)
- Redução: T_reduce ≈ ⌈log2 k⌉·t + α (árvore de redução)


