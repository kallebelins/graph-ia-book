### Capítulo 14 — Tendências Futuras: Grafos Dinâmicos e Adaptativos

Este exemplo acompanha o texto em `docs/book/19-capitulo-14.md`.

- Chain (baseline): janela determinística de 100 execuções com latências 800/1200ms — média 1000ms.
- Graph (baseline): mesmo perfil, sem caminho alternativo.
- Graph (adaptativo): mesma janela porém com aumento da cauda (10 amostras 800→1400ms), adicionando caminho alternativo B' (800ms).

Execução (runner):

```bash
# Chain
dotnet run --project src/book.csproj -- -c 14 -m c
# Graph (baseline + adaptativo)
dotnet run --project src/book.csproj -- -c 14 -m g
# Benchmark (teoria da regra dinâmica)
dotnet run --project src/book.csproj -- -c 14 -m b
```

Resultados esperados (arquivos gerados):
- `src/Benchmark/results/cap14_chain_latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap14_graph_baseline-latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap14_graph_adaptive-latency-summary-summary.{json,md}`
- `src/Benchmark/results/cap14_benchmark_dynamic-rule-summary.{json,md}`

Observação: a regra é determinística e reprodutível, seguindo a descrição do capítulo.


