# Relatório Capítulo 20 — Placeholder (baseline)

Este relatório registra um baseline mínimo para o Capítulo 20, enquanto o escopo definitivo é definido. Inclui medições padrão de chain/graph/benchmark com esperas fixas e referências de execução.

## Sumário de Métricas (baseline)

- Fontes dos dados (nomes de saída atuais):
  - `cap20/chain/default`
  - `cap20/graph/default`
  - `cap20/benchmark/default`

Resultados esperados (aprox.):

| Modo | Observação |
|---|---|
| Chain (SK) | atraso ~5 ms |
| Graph (SKG) | atraso ~5 ms |
| Benchmark (default) | atraso ~1 ms |

Nota: Estes valores são placeholders; os experimentos finais deverão substituir por artefatos `src/Benchmark/results/cap20_*` com A/B e teoria, conforme o tema escolhido.

## Reprodutibilidade

- PowerShell
```powershell
./Scripts/run.ps1 -chapter 20 -mode c
./Scripts/run.ps1 -chapter 20 -mode g
./Scripts/run.ps1 -chapter 20 -mode b
```

- Bash
```bash
bash Scripts/run.sh 20 c
bash Scripts/run.sh 20 g
bash Scripts/run.sh 20 b
```

## Próximos passos

- Definir o tópico final do capítulo (experimental/teórico).
- Substituir medições de placeholder por medições reais, gerando `src/Benchmark/results/cap20_*` e atualizando este relatório com tabelas e referências.

