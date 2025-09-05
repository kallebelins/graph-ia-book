### Benchmarks Chain vs Graph com Semantic Kernel (SK) e Semantic Kernel Graph (SKG)

Este diretório contém o runner e a documentação prática do livro "Grafos e Inteligência Artificial - Uma Fundamentação Teórica e Aplicada para Orquestração de Sistemas" sobre orquestração baseada em grafos para IA, comparando pipelines lineares (chains com SK) e grafos de execução (graphs com SKG). Aqui você encontra:
- Visão geral dos capítulos e objetivos
- Como executar os exemplos (chain/graph) e os benchmarks
- Índice com links para todos os exemplos e relatórios
- Referências cruzadas para a biblioteca SKG e diretrizes do projeto


### Como este projeto está organizado

- `graph-ia-book/src`:
  - `Program.cs` e `book.csproj`: runner que executa capítulos em três modos: chain (c), graph (g) e benchmark (b)
  - `Chapters/ChapterX.cs`: entrypoints por capítulo com `RunChainAsync`, `RunGraphAsync`, `RunBenchmarkAsync`
  - `Benchmark/_common`: utilitários de medição (média, p95/p99, custo de tokens), logging/tracing e validações de grafos
  - `Benchmark/notebooks`: templates para relatórios
  - `chains/` e `graphs/`: implementações SK e SKG por capítulo
  - `Scripts/`: scripts reprodutíveis PowerShell e bash
  - `tests/`: verificações automatizadas mínimas
- `graph-ia-book/docs`:
  - `examples/`: descrição dos exemplos por capítulo
  - `reports/`: consolidação dos resultados e métricas por capítulo
- `semantic-kernel-graph/` e `semantic-kernel-graph-docs/`: biblioteca SKG e documentação de referência

O plano detalhado de implementação, com checklist por capítulo, está em `graph-ia-book/docs/tasks.md`.


### Executando rapidamente

Pré-requisitos:
- .NET SDK 8+
- PowerShell (Windows) ou bash (Linux/macOS)
- Variáveis de ambiente para o(s) provedor(es) de LLM (por exemplo):
  - `OPENAI_API_KEY` ou `AZURE_OPENAI_API_KEY` (e afins)

Exemplos de execução:
- Benchmark do Capítulo 6 (PowerShell):
  - `graph-ia-book/src/Scripts/run.ps1 -chapter 6 -mode b`
- Benchmark do Capítulo 6 (bash):
  - `bash graph-ia-book/src/Scripts/run.sh --chapter 6 --mode b`
- Introdução 00e (scripts dedicados):
  - `graph-ia-book/src/scripts/run-00e-chain.ps1` | `graph-ia-book/src/scripts/run-00e-graph.ps1`
  - `bash graph-ia-book/src/scripts/run-00e-chain.sh` | `bash graph-ia-book/src/scripts/run-00e-graph.sh`

Resultados padronizados são exportados para `graph-ia-book/src/Benchmark/results/*` em formatos `.json` e `.md`. Cada relatório em `docs/reports` referencia esses arquivos.


### Índice de Exemplos (docs/examples)

- 00e — Introdução: `docs/examples/00-introducao.md`
- 01 — Problema da Complexidade: `docs/examples/02-problema-complexidade.md`
- 03 — Orquestração: `docs/examples/04-orquestracao.md`
- 04 — Capítulo 4: `docs/examples/06-capitulo-4.md`
- 05 — Capítulo 5: `docs/examples/08-capitulo-5.md`
- 06 — Capítulo 6: `docs/examples/09-capitulo-6.md`
- 07 — Resiliência (Cap. 7): `docs/examples/10-resiliencia-7.md`
- 08 — Capítulo 8: `docs/examples/11-capitulo-8.md`
- 09 — Capítulo 9: `docs/examples/12-capitulo-9.md`
- 11 — Capítulo 11: `docs/examples/15-capitulo-11.md`
- 12 — Capítulo 12: `docs/examples/16-capitulo-12.md`
- 13 — Limitações (Cap. 13): `docs/examples/18-limitacoes-grafos-13.md`
- 14 — Capítulo 14: `docs/examples/19-capitulo-14.md`
- 16 — Capítulo 16: `docs/examples/22-capitulo-16.md`
- 17 — Capítulo 17: `docs/examples/23-capitulo-17.md`
- 18 — Capítulo 18: `docs/examples/24-capitulo-18.md`
- 19 — Capítulo 19: `docs/examples/26-capitulo-19.md`
- 20 — Capítulo 20: `docs/examples/27-capitulo-20.md`
- 21 — Capítulo 21: `docs/examples/28-capitulo-21.md`
- 22 — Capítulo 22: `docs/examples/29-capitulo-22.md`
- 23 — Capítulo 23: `docs/examples/31-capitulo-23.md`
- 24 — Capítulo 24: `docs/examples/32-capitulo-24.md`
- 25 — Capítulo 25: `docs/examples/33-capitulo-25.md`
- 26 — Capítulo 26: `docs/examples/34-capitulo-26.md`
- 15 — Síntese (Cap. 15): `docs/examples/35-capitulo-15.md`
- Glossário técnico: `docs/examples/36-glossario.md`


### Índice de Relatórios (docs/reports)

- 00e — Introdução: `docs/reports/00e-introducao.md`
- Capítulo 1: `docs/reports/02-capitulo-1.md`
- Capítulo 2: `docs/reports/03-capitulo-2.md`
- Capítulo 3: `docs/reports/04-capitulo-3.md`
- Capítulo 4: `docs/reports/06-capitulo-4.md`
- Capítulo 5: `docs/reports/08-capitulo-5.md`
- Capítulo 6: `docs/reports/09-capitulo-6.md`
- Capítulo 7: `docs/reports/10-capitulo-7.md`
- Capítulo 8: `docs/reports/11-capitulo-8.md`
- Capítulo 9: `docs/reports/12-capitulo-9.md`
- Capítulo 10: `docs/reports/14-capitulo-10.md`
- Capítulo 11: `docs/reports/15-capitulo-11.md`
- Capítulo 12: `docs/reports/16-capitulo-12.md`
- Capítulo 13: `docs/reports/18-capitulo-13.md`
- Capítulo 14: `docs/reports/19-capitulo-14.md`
- Capítulo 15: `docs/reports/35-capitulo-15.md`
- Capítulo 16: `docs/reports/22-capitulo-16.md`
- Capítulo 17: `docs/reports/23-capitulo-17.md`
- Capítulo 18: `docs/reports/24-capitulo-18.md`
- Capítulo 19: `docs/reports/26-capitulo-19.md`
- Capítulo 20: `docs/reports/27-capitulo-20.md`
- Capítulo 21: `docs/reports/28-capitulo-21.md`
- Capítulo 22: `docs/reports/29-capitulo-22.md`
- Capítulo 23: `docs/reports/31-capitulo-23.md`
- Capítulo 24: `docs/reports/32-capitulo-24.md`
- Capítulo 25: `docs/reports/33-capitulo-25.md`
- Capítulo 26: `docs/reports/34-capitulo-26.md`

Cada relatório consolida as métricas exportadas de `src/Benchmark/results/*` (sumários `.json` e `.md`) e inclui checklist de reprodutibilidade com os comandos dos scripts em `src/Scripts`.


### Diretrizes e Fontes de Verdade (SKG)

Este projeto segue estritamente as diretrizes, padrões e exemplos dos diretórios:
- Biblioteca (SKG): `SemanticKernel.Graph` (Nuget)
- Exemplos (SKG): `https://github.com/kallebelins/semantic-kernel-graph-docs/tree/main/examples`
- Documentação (SKG): `https://skgraph.dev/`

As implementações SKG neste repositório foram desenvolvidas para manter consistência com a arquitetura definida nesses diretórios. Para exemplos SK (chains), buscamos estilo equivalente e destacamos limitações frente aos grafos equivalentes.


### Roadmap e Progresso

O progresso, incluindo o que falta e o que foi concluído por capítulo (exemplos, benchmarks, provas e métricas), está descrito em `graph-ia-book/docs/tasks.md`. Ao finalizar uma tarefa, o status é atualizado diretamente nesse arquivo.


### Contribuindo

- Abra PRs com descrições curtas e links para os capítulos/relatórios afetados
- Mantenha padrões de código/documentação e nomenclatura conforme SKG
- Para novos capítulos, garanta: `RunChainAsync`, `RunGraphAsync`, `RunBenchmarkAsync`, métricas padronizadas e relatório correspondente em `docs/reports`
