# Relatório Capítulo 2 — Base Matemática (Expressividade e Latência)

Este relatório consolida os resultados do Capítulo 2, comparando execução em cadeia (SK) e em grafo (SKG) em termos de latência e registrando a verificação teórica de expressividade (chains ⊂ DAGs ⊂ grafos) e propriedades estruturais (aciclicidade e ordem topológica).

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap2_benchmark_latency-ab_chain_latency-summary.json|.md`
  - `src/Benchmark/results/cap2_benchmark_latency-ab_graph_latency-summary.json|.md`
  - `src/Benchmark/results/cap2_theory_expressivity-summary.json|.md`

### Benchmark A/B: Latência (30 iterações por modo)

| Modo | Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---|---:|---:|---:|---:|
| Chain (SK) | 30 | 46 | 49 | 90 |
| Graph (SKG) | 30 | 45 | 53 | 83 |

Observa-se médias semelhantes entre os modos no cenário simulado, com p99 menor no grafo, sugerindo maior resiliência a caudas em configurações com ramificações e merge determinístico.

### Teoria: Expressividade e Estrutura

| Parâmetro | Valor |
|---|---|
| isAcyclic | true |
| ordem_topológica | `a -> b -> merge -> normalize -> start` |
| caminhos_em_chain | 1 |
| ramos_paralelos_em_grafo | 2 |
| relação_expressividade | `chain ⊂ DAG ⊂ graph` |

Interpretação: o grafo validado é acíclico e possui dois ramos paralelos convergindo, reforçando a proposição de que grafos são estritamente mais expressivos que cadeias lineares (que possuem exatamente um único caminho). A ordenação topológica garante execução correta e auditável.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 2 -mode b
```

- Bash
```bash
bash Scripts/run.sh 2 b
```

## Referências de Saída

- Chain (latência): `src/Benchmark/results/cap2_benchmark_latency-ab_chain_latency-summary.json`, `.md`
- Graph (latência): `src/Benchmark/results/cap2_benchmark_latency-ab_graph_latency-summary.json`, `.md`
- Teoria (expressividade): `src/Benchmark/results/cap2_theory_expressivity-summary.json`, `.md`
