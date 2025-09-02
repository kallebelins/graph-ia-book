## Plano de Implementação de Exemplos, Provas e Métricas por Capítulo

Este documento organiza, por capítulo do livro em `graph-ia-book/docs/book`, as tarefas para criar:

- Exemplos de pipeline linear (chain) usando Semantic Kernel (SK)
- Exemplos em grafo (graph) usando Semantic Kernel Graph (SKG)
- Provas/demonstrações formais e validações matemáticas
- Métricas, benchmarks e experimentos reprodutíveis

Regras gerais:
- Siga estritamente a arquitetura, padrões e exemplos dos diretórios de referência:
  - Biblioteca (SKG): `semantic-kernel-graph/src/SemanticKernel.Graph`
  - Exemplos (SKG): `semantic-kernel-graph-docs/examples`
  - Testes (SKG): `semantic-kernel-graph/src/SemanticKernel.Graph.Tests`
  - Documentação (SKG): `semantic-kernel-graph-docs/docs`
- Para os exemplos SK (chains), referenciar guia oficial do Semantic Kernel e manter estilo consistente com os exemplos SKG.
- Todos os artefatos de código deste plano serão criados em `graph-ia-book/src`.
- Cada capítulo deve conter: (1) um exemplo SK, (2) um exemplo SKG, (3) métricas comparativas (latência média, p95/p99, custo de tokens, acertos/precisão quando aplicável), (4) mini-prova ou validação matemática conectada ao texto, (5) checklist de reprodutibilidade.

Organização do Projeto (Runner book.csproj):
- Projeto runner: `graph-ia-book/src/book.csproj`
- Entrypoint: `graph-ia-book/src/Program.cs`
- Capítulos: `graph-ia-book/src/Chapters/ChapterX.cs` (X = 1..N)
- Utilitários de benchmark comuns: `graph-ia-book/src/Benchmark/_common`
- Scripts: `graph-ia-book/src/Scripts`
- Guia de execução: ver `graph-ia-book/src/README.md`
- Execução: `Program` seleciona capítulo (1,2,3,...) e modo (c=chain, g=graph, b=benchmark) e invoca:
  - se c: `ChapterX.RunChainAsync()`
  - se g: `ChapterX.RunGraphAsync()`
  - senão: `ChapterX.RunBenchmarkAsync()`
  
Contrato por Capítulo (ChapterX):
- Classe deve conter descrição do que é testado/medido/comparado no capítulo.
- Métodos obrigatórios (com XML docs detalhando o que medem):
  - `Task RunChainAsync()`
  - `Task RunGraphAsync()`
  - `Task RunBenchmarkAsync()` (compara métricas/relatórios de chain e graph)
- Se existir mais de uma métrica/relatório por modo, criar métodos específicos por palavra-chave, por exemplo:
  - `Task RunChain_LatencyAsync()`, `Task RunChain_CostAsync()`
  - `Task RunGraph_LatencyAsync()`, `Task RunGraph_CostAsync()`
  - `Task RunBenchmark_LatencyVsCostAsync()`
  E garantir que `RunChainAsync`, `RunGraphAsync` e `RunBenchmarkAsync` invoquem todos os métodos específicos do respectivo modo.

Observação: ao concluir cada tarefa, marque [ ] e inclua links para arquivos criados.

---

### Infraestrutura Global (antes de iniciar capítulos)
- [x] Criar projeto runner `graph-ia-book/src/book.csproj`
- [x] Implementar `graph-ia-book/src/Program.cs` com seleção de capítulo e modo (c/g/b)
- [x] Criar pasta `graph-ia-book/src/Chapters` e esqueleto `ChapterTemplate.cs` + `Chapter1.cs` (com XML docs)
- [x] Criar scripts `graph-ia-book/src/Scripts/run.ps1` e `run.sh` aceitando `--chapter` e `--mode`
- [x] Integrar utilitários de medição aos métodos `Run*` via `_common`
- [x] Criar utilitários de medição comuns (latência média, p95/p99, contagem de tokens, custo estimado)
  - Saída padronizada: JSON e Markdown
  - Diretório: `graph-ia-book/src/Benchmark/_common`
- [x] Criar harness de benchmark A/B (SK vs SKG) com semente e entradas fixas
  - Arquivos: `graph-ia-book/src/Benchmark/_common/AbBenchmarkHarness.cs`
- [x] Criar fixtures de dados de teste (texto, perguntas, datasets pequenos)
  - Arquivos: `graph-ia-book/src/Benchmark/_common/TestFixtures.cs`
- [x] Criar template de notebook para relatórios de resultados por capítulo
  - Arquivo: `graph-ia-book/src/Benchmark/notebooks/ChapterReportTemplate.ipynb`
- [x] Implementar checagem de aciclicidade e ordem topológica para grafos de exemplo (usa SKG)
  - Arquivo: `graph-ia-book/src/Benchmark/_common/GraphValidationUtils.cs`
- [x] Definir políticas de logging e tracing por nó/caminho (usa SKG Streaming/State)
  - Arquivo: `graph-ia-book/src/Benchmark/_common/GraphLoggingUtils.cs`

---

### 00e-introducao.md — Introdução
- [x] SK (chain): pipeline mínimo (entrada → LLM → resposta)
  - Código: `graph-ia-book/src/chains/00e-introducao`
  - Script: `graph-ia-book/src/scripts/run-00e-chain.ps1` e `graph-ia-book/src/scripts/run-00e-chain.sh`
- [x] SKG (graph): mesmo fluxo com 2 ramos paralelos (ex.: sumarização e extração), convergindo em agregador
  - Código: `graph-ia-book/src/graphs/00e-introducao`
  - Script: `graph-ia-book/src/scripts/run-00e-graph.ps1` e `graph-ia-book/src/scripts/run-00e-graph.sh`
- [x] Métricas: latência média e p95; comparação do makespan com e sem paralelismo
- [x] Validação matemática: explicação do limite por caminho crítico (Brent) aplicada ao exemplo
- [x] Documentar resultados em `docs/examples/00-introducao.md` (ver saídas em `src/Benchmark/results/00e_*`)

### 02-capitulo-1.md — Problema da Complexidade em IA
- [x] SK: chain com 4 estágios sequenciais simulando acoplamento (ex.: preprocess → retrieve → reason → answer)
- [x] SKG: grafo com ramos independentes (retrieve paralelo + verificador), merge determinístico
- [x] Métricas: soma sequencial vs máximo paralelo + custo de agregação; p95/p99
- [x] Prova/checagem: ordem topológica e estimativa de makespan comparativo
- [x] Doc: `docs/examples/02-problema-complexidade.md` (ver `src/Benchmark/results/chapter1_*`)

### 04-capitulo-3.md — Orquestração de Sistemas Inteligentes
- [x] SK: chain com decisão condicional implementada via if/switch externo (limitação)
  - Código: `graph-ia-book/src/chains/chapter3/ChainChapter3.cs`
- [x] SKG: nó de roteamento condicional nativo → dois caminhos → merge
  - Código: `graph-ia-book/src/graphs/chapter3/GraphChapter3.cs`
- [x] Métricas: impacto da decisão no tempo médio e na variância
- [x] Validação: corretude da fusão determinística (política definida)
- [x] Doc: `docs/examples/04-orquestracao.md` (ver `src/Benchmark/results/cap3_*`)

### 10-capitulo-7.md — Recuperação e Resiliência
- [x] SK: chain com falha injetada no estágio 2 → aborta
  - Código: `graph-ia-book/src/chains/chapter7/ChainChapter7.cs`
- [x] SKG: fallback de `v2` para `v2'`, checkpoints e reexecução parcial
  - Código: `graph-ia-book/src/graphs/chapter7/GraphChapter7.cs`
- [x] Métricas: taxa de sucesso sob falha, tempo adicional sob fallback, custo
- [x] Validação: modelagem probabilística simples de sucesso composto 1−∏(1−p_i)
- [x] Doc: `docs/examples/10-resiliencia.md` (ver `src/Benchmark/results/cap7_*`)
  - Arquivos gerados: `src/Benchmark/results/cap7_benchmark_success-theory-summary.json`, `src/Benchmark/results/cap7_benchmark_success-theory-summary.md`

### 18-capitulo-13.md — Limitações da Abordagem em Grafos
- [x] SK: chain trivial (CSV→JSON) para mostrar casos onde chain é melhor
  - Código: `graph-ia-book/src/chains/chapter13/ChainChapter13.cs`
- [x] SKG: grafo com sobrecarga de coordenação (ramos superficiais) para comparar custo
  - Código: `graph-ia-book/src/graphs/chapter13/GraphChapter13.cs`
- [x] Métricas: TCO simplificado, latência, memória, logs
  - Capítulo: `graph-ia-book/src/Chapters/Chapter13.cs` (métodos `Run*`)
  - Arquivos gerados (exemplos):
    - `src/Benchmark/results/cap13_benchmark_chain-latency-summary.json` e `.md`
    - `src/Benchmark/results/cap13_benchmark_graph-latency-summary.json` e `.md`
    - `src/Benchmark/results/cap13_benchmark_gain-vs-cost-summary.json` e `.md`
- [x] Validação: condição G < C do texto (ganho vs custo), experimento paramétrico
- [x] Doc: `docs/examples/18-limitacoes-grafos.md` (ver `src/Benchmark/results/cap13_*`)

### 03-capitulo-2.md — Teoria de Grafos: Base Matemática
- [x] SK: chain linear demonstrando caminho único
  - Código: `graph-ia-book/src/chains/chapter2/ChainChapter2.cs`
- [x] SKG: grafo com dois ramos paralelos e merge determinístico
  - Código: `graph-ia-book/src/graphs/chapter2/GraphChapter2.cs`
- [x] Métricas: latência média, p95/p99 e A/B (chain vs graph)
  - Implementadas em `graph-ia-book/src/Chapters/Chapter2.cs`
- [x] Validação: teoria de expressividade e ordem topológica (chain ⊂ DAG ⊂ graph)
  - Arquivo gerado: `src/Benchmark/results/cap2_theory_expressivity-summary.{json,md}`
- [x] Doc: `docs/examples/03-capitulo-2.md`

### 06-capitulo-4.md — Explosão de Estados e Modularidade
- [x] SK: chain com 5 estágios e handling externo por estado
  - Código: `graph-ia-book/src/chains/chapter4/ChainChapter4.cs`
- [x] SKG: grafo com convergência em nó compartilhado de tratamento
  - Código: `graph-ia-book/src/graphs/chapter4/GraphChapter4.cs`
- [x] Métricas: medir média, p95/p99, A/B chain vs graph
  - Capítulo: `graph-ia-book/src/Chapters/Chapter4.cs` (métodos `Run*`)
- [x] Validação: teoria k^n vs convergência (arquivo `cap4/theory/state-counts`)
- [x] Doc: `docs/examples/06-capitulo-4.md`

### 08-capitulo-5.md — Explicabilidade e Auditoria
- [x] SK: chain com decisão imperativa (sem trilha explícita)
  - Código: `graph-ia-book/src/chains/chapter5/ChainChapter5.cs`
- [x] SKG: grafo com nó condicional e trilha de auditoria (streaming opcional)
  - Código: `graph-ia-book/src/graphs/chapter5/GraphChapter5.cs`
- [x] Métricas: medir média, p95/p99
- [x] Validação: nota teórica sobre trilha explícita (graph) vs implícita (chain)
- [x] Doc: `docs/examples/08-capitulo-5.md`

### 09-capitulo-6.md — Escalabilidade e Concorrência
- [x] SK: chain com fan-out de 5 ramos executados sequencialmente
  - Código: `graph-ia-book/src/chains/chapter6/ChainChapter6.cs`
- [x] SKG: grafo com 5 ramos paralelos e agregador determinístico
  - Código: `graph-ia-book/src/graphs/chapter6/GraphChapter6.cs`
- [x] Métricas: medir média, p95/p99; A/B (chain vs graph)
  - Implementadas em `graph-ia-book/src/Chapters/Chapter6.cs`
- [x] Validação: caminho crítico vs soma sequencial; speedup teórico
  - Arquivo gerado: `src/Benchmark/results/cap6_theory_critical-path-summary.{json,md}`
- [x] Doc: `docs/examples/09-capitulo-6.md`

### 11-capitulo-8.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/11-capitulo-8.md`

### 12-capitulo-9.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/12-capitulo-9.md`

### 14-capitulo-10.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/14-capitulo-10.md`

### 15-capitulo-11.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/15-capitulo-11.md`

### 16-capitulo-12.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/16-capitulo-12.md`

### 19-capitulo-14.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/19-capitulo-14.md`

### 22-capitulo-16.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/22-capitulo-16.md`

### 23-capitulo-17.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/23-capitulo-17.md`

### 24-capitulo-18.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/24-capitulo-18.md`

### 26-capitulo-19.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/26-capitulo-19.md`

### 27-capitulo-20.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/27-capitulo-20.md`

### 28-capitulo-21.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/28-capitulo-21.md`

### 29-capitulo-22.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/29-capitulo-22.md`

### 31-capitulo-23.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/31-capitulo-23.md`

### 32-capitulo-24.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/32-capitulo-24.md`

### 33-capitulo-25.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/33-capitulo-25.md`

### 34-capitulo-26.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/34-capitulo-26.md`

### 35-capitulo-15.md — A definir
- [ ] SK: definir chain conforme tópico do capítulo
- [ ] SKG: definir grafo conforme tópico do capítulo
- [ ] Métricas: medir média, p95/p99, custo de tokens
- [ ] Validação: adicionar verificação/prova alinhada ao texto
- [ ] Doc: `docs/examples/35-capitulo-15.md`

### 36-glossario-tecnico.md — Glossário
- [ ] Criar exemplos mínimos por termo-chave (DAG, ordem topológica, betweenness, caminho crítico)
- [ ] Adicionar snippets SKG demonstrando checagens/medições simples onde fizer sentido
- [ ] Doc: `docs/examples/36-glossario.md`

---

### Itens recorrentes por capítulo (Checklist)
- [ ] Implementar `ChapterX` com descrição clara do escopo do capítulo
- [ ] Implementar `RunChainAsync`/`RunGraphAsync`/`RunBenchmarkAsync`
- [ ] Documentar cada método com o que é testado/medido/comparado (XML docs)
- [ ] Se houver múltiplas métricas: criar métodos específicos por palavra-chave e orquestrar via `Run*`
- [ ] Implementar versão SK (chain) funcional, com entrada/saída estáveis
- [ ] Implementar versão SKG (graph) funcional, com logs e tracing
- [ ] Adicionar testes automatizados mínimos:
  - Local: `graph-ia-book/src/tests` (para SK e SKG)
  - Verificações: aciclicidade (SKG), corretude de merge, ausência de exceções
- [ ] Incluir script de execução reprodutível via PowerShell e bash (dir: `graph-ia-book/src/Scripts`) chamando `Program`
- [ ] Medir métricas padronizadas (média, p95/p99, custo de tokens)
- [ ] Exportar relatório Markdown + gráfico simples (boxplot/linha) no diretório do capítulo
- [ ] Atualizar links cruzados na documentação do capítulo do livro

---

### Mapeamento de Diretórios de Referência (uso nas tarefas)
- Biblioteca (SKG): `semantic-kernel-graph/src/SemanticKernel.Graph`
- Exemplos (SKG): `semantic-kernel-graph-docs/examples`
- Testes (SKG): `semantic-kernel-graph/src/SemanticKernel.Graph.Tests`
- Documentação (SKG): `semantic-kernel-graph-docs/docs`

Para cada exemplo SKG, usar os padrões de projeto, nomenclatura e convenções dos diretórios acima. Para exemplos SK (chains), seguir estilo semelhante e explanar limitações quando comparado ao grafo equivalente.


