# Relatório Capítulo 14 — Tendências Futuras da Orquestração em Grafos

Este relatório consolida o experimento de regra dinâmica (dynamic-rule) que ajusta a topologia do grafo com base em telemetria de latência. O objetivo é ilustrar adaptações de grafos ao ambiente em tempo quase real.

## Sumário de Métricas

- Fonte dos dados:
  - `src/Benchmark/results/cap14_benchmark_dynamic-rule-summary.json|.md`

### Regra Dinâmica: Adaptação por Janela de Latência

| Parâmetro | Valor |
|---|---|
| baseline_mean_ms | 1000 |
| threshold_ms_strict_greater | 1000 |
| window_size | 100 |
| new_mean_ms | 1060 |
| should_add_B_prime_baseline | False |
| should_add_B_prime_new | True |

Interpretação: A média de latência na janela excedeu o limiar, disparando a inclusão de um ramo alternativo `B'` (should_add_B_prime_new = True). Esse padrão demonstra como grafos podem evoluir regras/topologias para reagir a degradações, buscando reduzir caudas futuras.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 14 -mode b   # Benchmarks/experimentos consolidados
```

- Bash
```bash
bash Scripts/run.sh 14 b
```

## Referências de Saída

- Regra dinâmica: `src/Benchmark/results/cap14_benchmark_dynamic-rule-summary.json`, `.md`

