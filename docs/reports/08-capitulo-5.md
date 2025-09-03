# Relatório Capítulo 5 — Explicabilidade e Auditoria

Este relatório consolida os resultados do Capítulo 5, comparando latência entre cadeia (SK) e grafo (SKG) em um cenário com decisão condicional e destacando o aspecto de explicabilidade: trilhas explícitas no grafo versus trilhas implícitas no chain.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap5_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap5_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap5_theory_explainability-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 47 | 66 | 133 |
| Graph (SKG) | 30 | 39 | 54 | 154 |

Observa-se vantagem do grafo em média e p95. O p99 mais alto no grafo reflete picos ocasionais em execuções com caminhos mais longos/overheads de auditoria, o que pode ocorrer quando múltiplos ramos são avaliados antes do merge.

### Teoria: Explicabilidade e Trilhas de Auditoria

| Parâmetro | Valor |
|---|---|
| chain_trace | `implicit (no explicit path)` |
| graph_trace | `explicit path with conditional evaluation events` |
| compliance_note | `graphs enable audit trails aligning with AI governance` |

Interpretação: Chains dependem de controle externo/imperativo para registrar decisões, tornando difícil auditar “por que” certo resultado ocorreu. Em grafos, a execução materializa um caminho explícito com eventos de condição e merge, provendo trilha detalhada — essencial para governança e conformidade.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 5 -mode b
```

- Bash
```bash
bash Scripts/run.sh 5 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap5_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap5_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (explicabilidade): `src/Benchmark/results/cap5_theory_explainability-summary.json`, `.md`
