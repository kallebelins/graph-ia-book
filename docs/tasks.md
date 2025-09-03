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

### 11-capitulo-8.md — Integração Multimodal e Híbrida
- [x] SK: chain voz→texto→resposta (ignora imagem por desenho)
  - Código: `graph-ia-book/src/chains/chapter8/ChainChapter8.cs`
- [x] SKG: grafo multimodal (voz+imagem) com fusão tardia em nó `Fusion`
  - Código: `graph-ia-book/src/graphs/chapter8/GraphChapter8.cs`
- [x] Métricas: medir média, p95/p99; benchmark A/B chain vs graph
  - Implementadas em `graph-ia-book/src/Chapters/Chapter8.cs` (métodos `Run*`)
- [x] Validação: teoria de fusão (chain = soma; graph ≈ max + fusão)
  - Arquivo gerado: `src/Benchmark/results/cap8_theory_fusion-summary.{json,md}`
- [x] Doc: `docs/examples/11-capitulo-8.md`

### 12-capitulo-9.md — Governança e Segurança em Grafos
- [x] SK: chain com avaliação de política e bloqueio
  - Código: `graph-ia-book/src/chains/chapter9/ChainChapter9.cs`
- [x] SKG: grafo com PolicyGuard → Anonymizer → Processor
  - Código: `graph-ia-book/src/graphs/chapter9/GraphChapter9.cs`
- [x] Métricas: medir média, p95/p99; benchmark A/B chain vs graph
  - Capítulo: `graph-ia-book/src/Chapters/Chapter9.cs` (métodos `Run*`)
- [x] Validação: teoria/checlist de governança por nó e desvio seguro
- [x] Doc: `docs/examples/12-capitulo-9.md`

### 14-capitulo-10.md — Diamante (Chain vs Grafo)
- [x] SK: chain padrão diamante sequencial
  - Código: `graph-ia-book/src/chains/chapter10/ChainChapter10.cs`
- [x] SKG: grafo padrão diamante com ramos paralelos
  - Código: `graph-ia-book/src/graphs/chapter10/GraphChapter10.cs`
- [x] Métricas: média, p95/p99; benchmark A/B chain vs graph
  - Capítulo: `graph-ia-book/src/Chapters/Chapter10.cs` (métodos `Run*`)
- [x] Validação: makespan do diamante (chain = soma; graph = max + merge)
  - Arquivo gerado: `src/Benchmark/results/cap10_theory_diamond-makespan-summary.{json,md}`
- [x] Doc: `docs/examples/14-capitulo-10.md`

### 15-capitulo-11.md — Aplicações Demonstrativas
- [x] SK: chain (Turismo, Finanças, Saúde)
  - Código: `graph-ia-book/src/chains/chapter11/ChainChapter11.cs`
- [x] SKG: grafo (paralelo/multimodal) para os três casos
  - Código: `graph-ia-book/src/graphs/chapter11/GraphChapter11.cs`
- [x] Métricas: medir média, p95/p99, custo aproximado
  - Capítulo: `graph-ia-book/src/Chapters/Chapter11.cs` (métodos `Run*`)
- [x] Validação: teoria chain=sum vs graph=max(paralelo)+overheads
  - Arquivos gerados: `src/Benchmark/results/cap11/theory/*`
- [x] Doc: `docs/examples/15-capitulo-11.md`

### 16-capitulo-12.md — Agentes Autônomos com Grafos
- [x] SK: chain rígido com decisão imperativa (agent-demo)
  - Código: `graph-ia-book/src/chains/chapter12/ChainChapter12.cs`
- [x] SKG: grafo com roteamento dinâmico (FAQ/Code/Escalate)
  - Código: `graph-ia-book/src/graphs/chapter12/GraphChapter12.cs`
- [x] Métricas: média, p95/p99; benchmark A/B chain vs graph
  - Implementadas em `graph-ia-book/src/Chapters/Chapter12.cs`
- [x] Validação: teoria de autonomia estrutural e aciclicidade do grafo
  - Arquivo gerado: `src/Benchmark/results/cap12/theory/autonomy-summary.{json,md}`
- [x] Doc: `docs/examples/16-capitulo-12.md`

### 19-capitulo-14.md — Tendências Futuras da Orquestração em Grafos
- [x] SK: definir chain conforme tópico do capítulo
  - Código: `graph-ia-book/src/chains/chapter14/ChainChapter14.cs`
- [x] SKG: definir grafo conforme tópico do capítulo
  - Código: `graph-ia-book/src/graphs/chapter14/GraphChapter14.cs`
- [x] Métricas: medir média, p95/p99, custo de tokens
  - Capítulo: `graph-ia-book/src/Chapters/Chapter14.cs` (métodos `Run*`)
- [x] Validação: adicionar verificação/prova alinhada ao texto
  - Arquivo gerado: `src/Benchmark/results/cap14_benchmark_dynamic-rule-summary.{json,md}`
- [x] Doc: `docs/examples/19-capitulo-14.md`

### 22-capitulo-16.md — Teoremas de Expressividade: Chains ⊂ DAGs e Limites
- [x] SK: chain com k módulos independentes serializados + merge determinístico
  - Código: `graph-ia-book/src/chains/chapter16/ChainChapter16.cs`
- [x] SKG: grafo com k módulos independentes paralelos + merge determinístico
  - Código: `graph-ia-book/src/graphs/chapter16/GraphChapter16.cs`
- [x] Métricas: medir média, p95/p99; benchmark A/B chain vs graph
  - Capítulo: `graph-ia-book/src/Chapters/Chapter16.cs` (métodos `Run*`)
- [x] Validação: escrever teoria T_chain vs T_DAG e redução O(log k)
  - Arquivo gerado: `src/Benchmark/results/cap16_theory_expressivity-proofs-summary.{json,md}`
- [x] Doc: `docs/examples/22-capitulo-16.md`

### 23-capitulo-17.md — Execução por Ordem Topológica e Caminho Crítico
- [x] SK: definir chain conforme tópico do capítulo
  - Código: `graph-ia-book/src/chains/chapter17/ChainChapter17.cs`
- [x] SKG: definir grafo conforme tópico do capítulo
  - Código: `graph-ia-book/src/graphs/chapter17/GraphChapter17.cs`
- [x] Métricas: medir média, p95/p99, custo de tokens
  - Capítulo: `graph-ia-book/src/Chapters/Chapter17.cs` (métodos `Run*`)
- [x] Validação: adicionar verificação/prova alinhada ao texto
  - Arquivo gerado: `src/Benchmark/results/cap17_theory_scheduling-critical-path-summary.{json,md}`
- [x] Doc: `docs/examples/23-capitulo-17.md`

### 24-capitulo-18.md — Álgebra de Grafos: Matrizes de Adjacência e Incidência
- [x] SK: chain com análise sequencial (A, graus, alcançabilidade, aciclicidade)
  - Código: `graph-ia-book/src/chains/chapter18/ChainChapter18.cs`
- [x] SKG: grafo com ramos paralelos (graus, alcançabilidade, aciclicidade) + merge
  - Código: `graph-ia-book/src/graphs/chapter18/GraphChapter18.cs`
- [x] Métricas: medir média, p95/p99, custo de tokens (parcial: latência padronizada)
  - Capítulo: `graph-ia-book/src/Chapters/Chapter18.cs` (métodos `Run*`)
- [x] Validação: escrita de teoria (A, A^2, graus, aciclicidade) alinhada ao texto
  - Arquivo gerado: `src/Benchmark/results/cap18/theory/adjacency-incidence-summary.{json,md}`
- [x] Doc: `docs/examples/24-capitulo-18.md`

### 26-capitulo-19.md — Grafos, Autômatos e Linguagens Formais
- [x] SK: chain (AFND sobre DAG rotulado; tamanho do AFD)
  - Código: `graph-ia-book/src/chains/chapter19/ChainChapter19.cs`
- [x] SKG: grafo (ramos paralelos: aceitação + tamanho do AFD)
  - Código: `graph-ia-book/src/graphs/chapter19/GraphChapter19.cs`
- [x] Métricas: média, p95/p99; benchmark A/B chain vs graph
  - Capítulo: `graph-ia-book/src/Chapters/Chapter19.cs` (métodos `Run*`)
- [x] Validação: regularidade em DAGs; aceitação de exemplos; tamanho do AFD
  - Arquivo gerado: `src/Benchmark/results/cap19/theory/regularity-nfa-dfa-summary.{json,md}`
- [x] Doc: `docs/examples/26-capitulo-19.md`

### 27-capitulo-20.md — A definir
- [x] SK: definir chain conforme tópico do capítulo
- [x] SKG: definir grafo conforme tópico do capítulo
- [x] Métricas: medir média, p95/p99, custo de tokens
- [x] Validação: adicionar verificação/prova alinhada ao texto
- [x] Doc: `docs/examples/27-capitulo-20.md`

### 28-capitulo-21.md — Processos Estocásticos em Grafos (Markov, Estados Absorventes)
- [x] SK: definir chain conforme tópico do capítulo
  - Código: `graph-ia-book/src/chains/chapter21/ChainChapter21.cs`
- [x] SKG: definir grafo conforme tópico do capítulo
  - Código: `graph-ia-book/src/graphs/chapter21/GraphChapter21.cs`
- [x] Métricas: medir média, p95/p99, custo de tokens
  - Capítulo: `graph-ia-book/src/Chapters/Chapter21.cs` (métodos `Run*`)
- [x] Validação: adicionar verificação/prova alinhada ao texto
  - Arquivo gerado: `src/Benchmark/results/cap21/theory/markov-summary.{json,md}`
- [x] Doc: `docs/examples/28-capitulo-21.md`

### 29-capitulo-22.md — Resiliência Probabilística e Fallback
- [x] SK: chain fallback sequencial
  - Código: `graph-ia-book/src/chains/chapter22/ChainChapter22.cs`
- [x] SKG: grafo OR paralelo com merge determinístico
  - Código: `graph-ia-book/src/graphs/chapter22/GraphChapter22.cs`
- [x] Métricas: média, p95/p99; benchmark A/B chain vs graph
  - Capítulo: `graph-ia-book/src/Chapters/Chapter22.cs` (métodos `Run*`)
- [x] Validação: p_total e E[T] (seq) vs OR (aprox.)
  - Arquivo gerado: `src/Benchmark/results/cap22/theory/fallback-summary.{json,md}`
- [x] Doc: `docs/examples/29-capitulo-22.md`

### 31-capitulo-23.md — A definir
- [x] SK: definir chain conforme tópico do capítulo
- [x] SKG: definir grafo conforme tópico do capítulo
- [x] Métricas: medir média, p95/p99, custo de tokens
- [x] Validação: adicionar verificação/prova alinhada ao texto
- [x] Doc: `docs/examples/31-capitulo-23.md`

### 32-capitulo-24.md — Topologias e Anti-padrões
- [x] SK: definir chain conforme tópico do capítulo
- [x] SKG: definir grafo conforme tópico do capítulo
- [x] Métricas: medir média, p95/p99, custo de tokens (latência padronizada)
- [x] Validação: adicionar verificação/prova alinhada ao texto (makespan)
- [x] Doc: `docs/examples/32-capitulo-24.md`

### 33-capitulo-25.md — A definir
- [x] SK: definir chain conforme tópico do capítulo
  - Código: `graph-ia-book/src/chains/chapter25/ChainChapter25.cs`
- [x] SKG: definir grafo conforme tópico do capítulo
  - Código: `graph-ia-book/src/graphs/chapter25/GraphChapter25.cs`
- [x] Métricas: medir média, p95/p99, custo de tokens
  - Capítulo: `graph-ia-book/src/Chapters/Chapter25.cs` (métodos `Run*`)
- [x] Validação: adicionar verificação/prova alinhada ao texto (baseline vs GNN-like)
- [x] Doc: `docs/examples/33-capitulo-25.md`

### 34-capitulo-26.md — A definir
- [x] SK: definir chain conforme tópico do capítulo
- [x] SKG: definir grafo conforme tópico do capítulo
- [x] Métricas: medir média, p95/p99, custo de tokens
- [x] Validação: adicionar verificação/prova alinhada ao texto
- [x] Doc: `docs/examples/34-capitulo-26.md`

### 35-capitulo-15.md — Síntese de três ramos (fan-out/fan-in)
- [x] SK: definir chain conforme tópico do capítulo
  - Código: `graph-ia-book/src/chains/chapter15/ChainChapter15.cs`
- [x] SKG: definir grafo conforme tópico do capítulo
  - Código: `graph-ia-book/src/graphs/chapter15/GraphChapter15.cs`
- [x] Métricas: medir média, p95/p99, custo de tokens (proxy)
  - Capítulo: `graph-ia-book/src/Chapters/Chapter15.cs` (métodos `Run*`)
- [x] Validação: adicionar verificação/prova alinhada ao texto (makespan soma vs max)
  - Arquivo gerado: `src/Benchmark/results/cap15/theory/makespan-3branch-summary.{json,md}`
- [x] Doc: `docs/examples/35-capitulo-15.md`

### 36-glossario-tecnico.md — Glossário
- [x] Criar exemplos mínimos por termo-chave (DAG, ordem topológica, betweenness, caminho crítico)
  - Código: `graph-ia-book/src/Glossary/GlossaryExamples.cs`
- [x] Adicionar snippets SKG demonstrando checagens/medições simples onde fizer sentido
  - Doc: `graph-ia-book/docs/examples/36-glossario.md`
- [x] Doc: `docs/examples/36-glossario.md`

---

### Itens recorrentes por capítulo (Checklist)
- [x] Implementar `ChapterX` com descrição clara do escopo do capítulo
- [x] Implementar `RunChainAsync`/`RunGraphAsync`/`RunBenchmarkAsync`
- [x] Documentar cada método com o que é testado/medido/comparado (XML docs) em português
- [x] Se houver múltiplas métricas: criar métodos específicos por palavra-chave e orquestrar via `Run*`
- [x] Implementar versão SK (chain) funcional, com entrada/saída estáveis
- [x] Implementar versão SKG (graph) funcional, com logs e tracing
- [x] Adicionar testes automatizados mínimos:
  - Local: `graph-ia-book/src/tests` (para SK e SKG)
  - Verificações: aciclicidade (SKG), corretude de merge, ausência de exceções
- [x] Incluir script de execução reprodutível via PowerShell e bash (dir: `graph-ia-book/src/Scripts`) chamando `Program`
- [x] Medir métricas padronizadas (média, p95/p99, custo de tokens)
- [x] Exportar relatório Markdown + gráfico simples (boxplot/linha) no diretório do capítulo
- [x] Atualizar links cruzados na documentação do capítulo do livro

---

### Mapeamento de Diretórios de Referência (uso nas tarefas)
- Biblioteca (SKG): `semantic-kernel-graph/src/SemanticKernel.Graph`
- Exemplos (SKG): `semantic-kernel-graph-docs/examples`
- Testes (SKG): `semantic-kernel-graph/src/SemanticKernel.Graph.Tests`
- Documentação (SKG): `semantic-kernel-graph-docs/docs`

Para cada exemplo SKG, usar os padrões de projeto, nomenclatura e convenções dos diretórios acima. Para exemplos SK (chains), seguir estilo semelhante e explanar limitações quando comparado ao grafo equivalente.



---

### Relatórios por Capítulo (consolidar resultados dos benchmarks)

Observação: para cada capítulo abaixo, o objetivo é transformar os arquivos resumidos gerados em `graph-ia-book/src/Benchmark/results` em uma seção clara de “Resultados e Métricas” dentro do arquivo correspondente em `graph-ia-book/docs/book`. Inclua referências aos arquivos `.json` e `.md` de sumário, 1 gráfico simples (linha/boxplot) quando aplicável, e uma checklist curta de reprodutibilidade com o comando do script em `graph-ia-book/src/Scripts`.

- [x] 00e — Introdução (docs/book/00e-introducao.md)
  - [x] Consolidar `src/Benchmark/results/00e_*`
  - [x] Criar “Resultados e Métricas” em `docs/reports/00e-introducao.md` com links, tabela e descritivo sobre o resultado
  - [x] Incluir checklist de reprodutibilidade (scripts `run-00e-*.ps1|.sh`)

- [x] Capítulo 1 — Problema da Complexidade (docs/book/02-capitulo-1.md)
  - [x] Consolidar `src/Benchmark/results/chapter1_*`
  - [x] Criar `docs/reports/02-capitulo-1.md` com links, tabela e descritivo sobre o resultado
  - [x] Incluir checklist (script `Scripts/run.ps1 -chapter 1 -mode b`)

- [x] Capítulo 2 — Base Matemática (docs/book/03-capitulo-2.md)
  - [x] Consolidar `src/Benchmark/results/cap2_*`
  - [x] Criar `docs/reports/03-capitulo-2.md` com links, tabela e descritivo sobre o resultado
  - [x] Incluir checklist (script `run.ps1 -chapter 2 -mode b`)

- [x] Capítulo 3 — Orquestração (docs/book/04-capitulo-3.md)
  - [x] Consolidar `src/Benchmark/results/cap3_*`
  - [x] Criar `docs/reports/04-capitulo-3.md` com links, tabela e descritivo sobre o resultado
  - [x] Incluir checklist (script `run.ps1 -chapter 3 -mode b`)

- [x] Capítulo 4 — Explosão de Estados (docs/book/06-capitulo-4.md)
  - [x] Consolidar `src/Benchmark/results/cap4_*`
  - [x] Criar `docs/reports/06-capitulo-4.md` com links, tabela e descritivo sobre o resultado
  - [x] Incluir checklist (script `run.ps1 -chapter 4 -mode b`)

- [x] Capítulo 5 — Explicabilidade (docs/book/08-capitulo-5.md)
  - [x] Consolidar `src/Benchmark/results/cap5_*`
  - [x] Criar `docs/reports/08-capitulo-5.md` com links, tabela e descritivo sobre o resultado
  - [x] Incluir checklist (script `run.ps1 -chapter 5 -mode b`)

- [ ] Capítulo 6 — Escalabilidade (docs/book/09-capitulo-6.md)
  - [ ] Consolidar `src/Benchmark/results/cap6_*`
  - [ ] Criar `docs/reports/09-capitulo-6.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 6 -mode b`)

- [ ] Capítulo 7 — Resiliência (docs/book/10-capitulo-7.md)
  - [ ] Consolidar `src/Benchmark/results/cap7_*`
  - [ ] Criar `docs/reports/10-capitulo-7.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 7 -mode b`)

- [ ] Capítulo 8 — Integração Multimodal (docs/book/11-capitulo-8.md)
  - [ ] Consolidar `src/Benchmark/results/cap8_*`
  - [ ] Criar `docs/reports/11-capitulo-8.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 8 -mode b`)

- [ ] Capítulo 9 — Governança (docs/book/12-capitulo-9.md)
  - [ ] Consolidar `src/Benchmark/results/cap9_*`
  - [ ] Criar `docs/reports/12-capitulo-9.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 9 -mode b`)

- [ ] Capítulo 10 — Diamante (docs/book/14-capitulo-10.md)
  - [ ] Consolidar `src/Benchmark/results/cap10_*`
  - [ ] Criar `docs/reports/14-capitulo-10.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 10 -mode b`)

- [ ] Capítulo 11 — Aplicações (docs/book/15-capitulo-11.md)
  - [ ] Consolidar `src/Benchmark/results/cap11_*`
  - [ ] Criar `docs/reports/15-capitulo-11.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 11 -mode b`)

- [ ] Capítulo 12 — Agentes Autônomos (docs/book/16-capitulo-12.md)
  - [ ] Consolidar `src/Benchmark/results/cap12_*`
  - [ ] Criar `docs/reports/16-capitulo-12.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 12 -mode b`)

- [ ] Capítulo 13 — Limitações (docs/book/18-capitulo-13.md)
  - [ ] Consolidar `src/Benchmark/results/cap13_*`
  - [ ] Criar `docs/reports/18-capitulo-13.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 13 -mode b`)

- [ ] Capítulo 14 — Tendências (docs/book/19-capitulo-14.md)
  - [ ] Consolidar `src/Benchmark/results/cap14_*`
  - [ ] Criar `docs/reports/19-capitulo-14.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 14 -mode b`)

- [ ] Capítulo 15 — Síntese 3 Ramos (docs/book/35-capitulo-15.md)
  - [ ] Consolidar `src/Benchmark/results/cap15_*`
  - [ ] Criar `docs/reports/35-capitulo-15.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 15 -mode b`)

- [ ] Capítulo 16 — Expressividade (docs/book/22-capitulo-16.md)
  - [ ] Consolidar `src/Benchmark/results/cap16_*`
  - [ ] Criar `docs/reports/22-capitulo-16.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 16 -mode b`)

- [ ] Capítulo 17 — Caminho Crítico (docs/book/23-capitulo-17.md)
  - [ ] Consolidar `src/Benchmark/results/cap17_*`
  - [ ] Criar `docs/reports/23-capitulo-17.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 17 -mode b`)

- [ ] Capítulo 18 — Álgebra de Grafos (docs/book/24-capitulo-18.md)
  - [ ] Consolidar `src/Benchmark/results/cap18_*`
  - [ ] Criar `docs/reports/24-capitulo-18.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 18 -mode b`)

- [ ] Capítulo 19 — Autômatos e Linguagens (docs/book/26-capitulo-19.md)
  - [ ] Consolidar `src/Benchmark/results/cap19_*`
  - [ ] Criar `docs/reports/26-capitulo-19.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 19 -mode b`)

- [ ] Capítulo 20 — A definir (docs/book/27-capitulo-20.md)
  - [ ] Consolidar `src/Benchmark/results/cap20_*`
  - [ ] Criar `docs/reports/27-capitulo-20.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 20 -mode b`)

- [ ] Capítulo 21 — Markov (docs/book/28-capitulo-21.md)
  - [ ] Consolidar `src/Benchmark/results/cap21_*`
  - [ ] Criar `docs/reports/28-capitulo-21.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 21 -mode b`)

- [ ] Capítulo 22 — Resiliência Probabilística (docs/book/29-capitulo-22.md)
  - [ ] Consolidar `src/Benchmark/results/cap22_*`
  - [ ] Criar `docs/reports/29-capitulo-22.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 22 -mode b`)

- [ ] Capítulo 23 — A definir (docs/book/31-capitulo-23.md)
  - [ ] Consolidar `src/Benchmark/results/cap23_*`
  - [ ] Criar `docs/reports/31-capitulo-23.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 23 -mode b`)

- [ ] Capítulo 24 — Topologias e Anti-padrões (docs/book/32-capitulo-24.md)
  - [ ] Consolidar `src/Benchmark/results/cap24_*`
  - [ ] Criar `docs/reports/32-capitulo-24.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 24 -mode b`)

- [ ] Capítulo 25 — A definir (docs/book/33-capitulo-25.md)
  - [ ] Consolidar `src/Benchmark/results/cap25_*`
  - [ ] Criar `docs/reports/33-capitulo-25.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 25 -mode b`)

- [ ] Capítulo 26 — A definir (docs/book/34-capitulo-26.md)
  - [ ] Consolidar `src/Benchmark/results/cap26_*`
  - [ ] Criar `docs/reports/34-capitulo-26.md` com links, tabela e descritivo sobre o resultado
  - [ ] Incluir checklist (script `run.ps1 -chapter 26 -mode b`)