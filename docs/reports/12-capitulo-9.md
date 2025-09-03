# Relatório Capítulo 9 — Governança e Segurança em Grafos

Este relatório consolida os resultados do Capítulo 9, comparando execução em cadeia (SK) e em grafo (SKG) em um cenário com política (PolicyGuard) e pós-processamento (Anonymizer). Inclui A/B de latência e uma validação teórica de governança.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap9_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap9_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap9_theory_governance-summary.json|.md`

### Benchmark A/B: Latência (24 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 24 | 50 | 48 | 110 |
| Graph (SKG) | 24 | 67 | 95 | 118 |

Interpretação: O grafo adiciona sobrecarga esperada de governança (roteamento seguro, verificações locais por nó), resultando em maior latência média, porém com benefícios de controle fino e desvio seguro não disponíveis nativamente em chains.

### Teoria: Governança por Nó e Desvio Seguro

| Parâmetro | Valor |
|---|---|
| chain_monitor_points | 2 |
| graph_monitor_nodes | 3 |
| supports_local_policies | True |
| supports_safe_routing | True |

Nota: Grafos permitem enforcement de políticas por nó (PolicyGuard) e roteamento seguro (desvio condicional), com trilhas explícitas para auditoria, ao custo de overhead de decisão e registros.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 9 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 9 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 9 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 9 c
bash Scripts/run.sh 9 g
bash Scripts/run.sh 9 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap9_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap9_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (governança): `src/Benchmark/results/cap9_theory_governance-summary.json`, `.md`

