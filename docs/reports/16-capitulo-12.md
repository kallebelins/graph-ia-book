# Relatório Capítulo 12 — Agentes Autônomos com Grafos

Este relatório consolida os resultados do Capítulo 12, comparando cadeia (SK) e grafo (SKG) em um cenário de agentes com roteamento dinâmico (FAQ/Code/Escalate). Inclui A/B de latência e uma validação teórica de autonomia estrutural.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap12_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap12_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap12_theory_autonomy-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 66 | 64 | 143 |
| Graph (SKG) | 30 | 48 | 48 | 81 |

O grafo apresenta menor média e cauda, beneficiando-se de decisões nativas e caminhos alternativos, reduzindo reprocessamentos e loops imperativos.

### Teoria: Autonomia Estrutural e Aciclicidade

| Parâmetro | Valor |
|---|---|
| autonomy_relation | chain = sequência rígida; graph = espaço de caminhos com decisão nativa |
| chain_path_count | 1 |
| graph_alternative_paths | 3 |
| isAcyclic | True |
| topologicalOrder | analyze -> code -> condCode -> condFaq -> escalate -> faq -> merge -> start |

Interpretação: O grafo permite múltiplos caminhos controlados por nós condicionais, mantendo aciclicidade e preservando corretude via ordem topológica. Isso habilita agentes mais autônomos com menor custo de controle.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 12 -mode c   # Chain (SK)
./Scripts/run.ps1 -chapter 12 -mode g   # Graph (SKG)
./Scripts/run.ps1 -chapter 12 -mode b   # Benchmarks consolidados
```

- Bash
```bash
bash Scripts/run.sh 12 c
bash Scripts/run.sh 12 g
bash Scripts/run.sh 12 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap12_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap12_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (autonomia): `src/Benchmark/results/cap12_theory_autonomy-summary.json`, `.md`

