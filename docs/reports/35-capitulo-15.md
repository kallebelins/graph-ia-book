# Relatório Capítulo 15 — Síntese de três ramos (fan-out/fan-in)

Este relatório consolida resultados do Capítulo 15, com foco em um fan-out/fan-in de três ramos e merge determinístico, comparando chain (SK) vs grafo (SKG) via proxy de custo/latência.

## Sumário de Métricas

- Fonte dos dados:
  - `src/Benchmark/results/cap15_theory_makespan-3branch-summary.json|.md`

### Proxy de Custo e Latência (50 amostras)

| Métrica | Chain (SK) | Graph (SKG) |
|---|---:|---:|
| mean_ms | 325 | 137 |
| cost_proxy | 325 | 137 |
| merge_ms | \- | 10 |
| expected_speedup_chain_over_graph | \- | 2,372 |

Interpretação: O grafo se beneficia de paralelismo entre três ramos, reduzindo o makespan para próximo do máximo dos ramos mais o custo de merge. O chain acumula as etapas em série, resultando em maior latência média e custo proxy.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 15 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 15 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 15 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 15 c
bash Scripts.run.sh 15 g
bash Scripts/run.sh 15 b
```

## Referências de Saída

- Teoria (makespan 3 ramos): `src/Benchmark/results/cap15_theory_makespan-3branch-summary.json`, `.md`

