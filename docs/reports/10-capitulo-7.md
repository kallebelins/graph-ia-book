# Relatório Capítulo 7 — Recuperação e Resiliência

Este relatório consolida os resultados do Capítulo 7, focado em resiliência com fallback e recuperação parcial usando grafos (SKG) versus a fragilidade de pipelines lineares (SK). Inclui taxa de sucesso, latência e comparação teórica de sucesso composto.

## Sumário de Métricas

- Fontes dos dados:
  - `src/Benchmark/results/cap7_benchmark_success-rate-summary.json|.md`
  - `src/Benchmark/results/cap7_benchmark_latency-p95-p99-summary.json|.md`
  - `src/Benchmark/results/cap7_benchmark_success-theory-summary.json|.md`

### Graph (SKG): Taxa de Sucesso com Fallback

| Iterações | Sucessos | Taxa de sucesso |
|---:|---:|---:|
| 100 | 98 | 98,00% |

Observa-se alta resiliência do fluxo com fallback: mesmo com falhas injetadas, o sistema mantém taxa de sucesso próxima de 100% ao desviar para caminhos alternativos.

### Graph (SKG): Latência (p95/p99)

| Iterações | Média (ms) | p95 (ms) | p99 (ms) |
|---:|---:|---:|---:|
| 50 | 14 | 16 | 17 |

A sobrecarga de fallback e checkpoints é baixa neste cenário, com caudas controladas (p95/p99) indicando estabilidade da estratégia de recuperação parcial.

### Comparação A/B: Chain (SK) vs Graph (SKG)

| Modo | Cenário | Sucesso | Média (ms) | p95 (ms) | p99 (ms) |
|---|---|---:|---:|---:|---:|
| Chain (SK) | Falha injetada no estágio 2 (abort) | N/A (abort) | N/A | N/A | N/A |
| Graph (SKG) | Fallback + checkpoints | 98,00% | 14 | 16 | 17 |

Nota: No chain com falha injetada, o pipeline aborta por desenho; portanto, não há conclusão bem-sucedida para sumarizar latências. O grafo, por sua vez, desvia para o caminho alternativo e conclui com alta taxa de sucesso e cauda controlada.

### Teoria vs Medido: Sucesso Composto

| Métrica | Valor |
|---|---|
| Teórico | 94,00% |
| Medido (amostra teórica) | 95,00% |

Interpretação: A taxa de sucesso composta esperada sob fallback (aprox. \(1 - \prod_i (1 - p_i)\)) está alinhada com os resultados medidos. Diferenças entre o medido agregado (98%) e a amostra do experimento teórico (95%) refletem variações entre cenários de injeção e amostragens distintas, mantendo coerência qualitativa com a tese: fallback aumenta robustez de forma significativa.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 7 -mode c   # Chain (SK): falha injetada / abort
./Scripts/run.ps1 -chapter 7 -mode g   # Graph (SKG): fallback + checkpoints
./Scripts/run.ps1 -chapter 7 -mode b
```

- Bash
```bash
bash Scripts/run.sh 7 c   # Chain (SK)
bash Scripts/run.sh 7 g   # Graph (SKG)
bash Scripts/run.sh 7 b
```

## Referências de Saída

- Sucesso (agregado): `src/Benchmark/results/cap7_benchmark_success-rate-summary.json`, `.md`
- Latência (p95/p99): `src/Benchmark/results/cap7_benchmark_latency-p95-p99-summary.json`, `.md`
- Teoria (sucesso composto): `src/Benchmark/results/cap7_benchmark_success-theory-summary.json`, `.md`

Observação: no cenário Chain (falha injetada), a execução aborta e, portanto, não há sumário de latência concluída para o modo c.

