# Glossário {-}

---

* **Aciclicidade**: Em termos simples, significa “não voltar para trás”. Tecnicamente, é a propriedade de não conter ciclos; é garantida em DAGs e verificável por ordem topológica.
* **AFND/AFD**: Autômato Finito Não Determinístico/Determinístico; modelos para linguagens regulares derivadas de DAGs rotulados.
* **Agente Autônomo**: Sistema de IA capaz de planejar, adaptar e reorquestrar seus próprios fluxos de execução em tempo real.
* **Alcançabilidade**: Existência de caminho entre dois nós; decidível via BFS/DFS e fecho transitivo.
* **Alcançabilidade Disjunta (Caminhos Disjuntos)**: Número de caminhos sem arestas compartilhadas entre pares; quantifica tolerância a falhas.
* **Aresta**: Em termos simples, é a ligação entre etapas. Tecnicamente, codifica dependência/transferência de estado e pode carregar rótulos, pesos (custos/probabilidades) e políticas.
* **ASR**: Automatic Speech Recognition; nó que transcreve áudio em texto.
* **Assortatividade Grau–Grau**: Correlação entre graus de nós conectados; alta assortatividade concentra carga em hubs.
* **Árvore de Redução**: Para funções associativas/comutativas, estrutura de fusão O(log k) que reduz caminho crítico.
* **Backpressure**: Controle de pressão em filas para evitar saturação; essencial em nós de alta betweenness.
* **Caminho**: Sequência de nós conectados. Em termos formais, sequência (v1,...,vk) com arestas (vi,vi+1)∈E.\index{caminho}
* **Caminho Crítico**: Em termos simples, é “o gargalo” de tempo. Tecnicamente, é o caminho com maior soma de durações; limita inferiormente o makespan.\index{caminho crítico}
* **Cauda de Latência (Tail at Scale)**: Em termos simples, são “os piores tempos que prejudicam a experiência”. Tecnicamente, é a distribuição de latências p99/p999 dominando SLAs em sistemas distribuídos.
* **Centralidade de Intermediação (Betweenness)**: Fração de caminhos mínimos que passam por um nó; alto valor indica gargalo potencial.\index{centralidade}
* **Centralidade de Proximidade (Closeness)**: Inverso da distância média a outros nós; aproxima “acesso” rápido ao grafo ativo.
* **CFG (Gramática Livre de Contexto)**: Formalismo para linguagens com dependências aninhadas; mais expressivo que regular.
* **Chain (Pipeline Linear)**: Em termos simples, é uma sequência rígida de etapas. Tecnicamente, é a estrutura de execução sequencial em que cada etapa depende rigidamente da saída da anterior.
* **Complexidade Ciclomática**: Em termos simples, mede “quantos caminhos independentes” existem. Tecnicamente, é a medida E−V+P (em grafos com ciclos), onde E é o número de arestas, V é o número de vértices e P é o número de componentes conexas; em DAG puro vale 0; indica risco de loops complexos.\index{complexidade ciclomática}
* **Confiança/Calibração**: Grau de certeza de um modelo. Tecnicamente, pode ser estimada por entropia de softmax, temperaturas e calibração pós-treinamento.
* **DAG (Directed Acyclic Graph)**: Em termos simples, um grafo com setas que nunca formam voltas. Tecnicamente, é um grafo direcionado e sem ciclos, ideal para representar fluxos de execução finitos. \index{DAG}
* **DAG Scheduling**: Estratégias para mapear nós em processadores respeitando dependências; inclui HEFT e camadas independentes.
* **Decidibilidade**: Capacidade de decidir uma propriedade por algoritmo finito; alcançabilidade/aciclicidade são decidíveis em DAGs finitos.
* **Diamante (Topologia)**: Em termos simples, a execução se divide em dois caminhos e depois junta. Tecnicamente, são dois ramos paralelos que convergem; bom para paralelismo com agregação determinística.\index{diamante}
* **Diamante Não Determinístico (Anti-padrão)**: Em termos simples, juntar resultados sem regra clara. Tecnicamente, é a convergência sem política definida; gera inconsistência e efeitos de ordem.
* **Diâmetro**: Em termos simples, é o número máximo de passos entre dois nós. Tecnicamente, é a maior distância geodésica (menor número de arestas) do grafo subjacente; correlaciona com latências end-to-end.\index{diâmetro}
* **Entropia de Softmax**: Em termos simples, mede o “quão indecisa” está a previsão. Tecnicamente, é a incerteza nas probabilidades de classe; maior entropia → menor confiança.
* **Explainability (Explicabilidade)**: Em termos simples, “explicar por que o sistema decidiu assim”. Tecnicamente, é a capacidade de justificar decisões por nó e por caminho, registrando racional, versão de modelo e métricas associadas.\index{explainability}
* **Estados Absorventes**: Estados terminais sem saída em cadeias de Markov; modelam conclusões (sucesso/falha finais).\index{estados absorventes}
* **Fallback**: Caminho alternativo acionado em caso de falha em um nó principal.
* **Fallback Sequencial**: Tentar alternativas em ordem; sucesso composto 1−∏(1−p_i) e tempo esperado condicionado.
* **Fecho Transitivo**: Conjunto de pares (u,v) tais que v é alcançável a partir de u; obtido por Floyd–Warshall ou multiplicações booleanas.
* **Fusão Híbrida**: Combina early para sinais fortemente correlatos e late para robustez, modelado como subgrafos hierárquicos.
* **Fusão Precoce (Early Fusion)**: Em termos simples, juntar sinais logo no começo. Tecnicamente, concatenação/projeção de embeddings de modalidades diferentes nas camadas iniciais.
* **Fusão Tardia (Late Fusion)**: Em termos simples, decidir após processar cada modalidade separadamente. Tecnicamente, agregação de escores/decisões (voto, max, média) com robustez a falhas.
* **GCN**: Convolutional network em grafos; propaga/filtra sinais por vizinhança com normalização de grau.
* **GNN (Graph Neural Network)**: Em termos simples, “rede neural que entende grafos”. Tecnicamente, modelos de message passing (ex.: GCN/GraphSAGE) que aprendem embeddings estruturais.
* **Grafo**: Estrutura matemática composta por vértices (nós) e arestas (ligações), capaz de modelar múltiplas relações.\index{grafo}
* **Grau (in/out)**: Número de arestas que entram/saem de um nó; graus altos indicam hubs/agregadores.\index{grau}
* **GraphSAGE**: Amostragem de vizinhos e agregação; escalável para grafos grandes em produção.
* **Governança**: Conjunto de políticas, restrições e controles aplicados para garantir confiabilidade, ética e compliance em sistemas de IA.\index{governança}
* **Guardrails (Políticas de Conteúdo)**: Em termos simples, “trilhos de proteção”. Tecnicamente, são políticas estruturais por nó/caminho que bloqueiam, filtram ou redirecionam saídas que violem regras (ex.: PII, toxicidade, categorias sensíveis).\index{guardrails}
* **HEFT**: Heuristic Earliest Finish Time; heurística de escalonamento em ambientes heterogêneos para reduzir makespan.
* **Independência vs. Correlação**: Independência superestima resiliência quando há causas comuns; modelar compartilhamentos ou usar bounds conservadores.
* **Limites de Boole/Bonferroni**: Bounds para compor probabilidades sem independência total; úteis quando há correlação desconhecida.
* **Linguagem Regular**: Conjunto reconhecível por AFND/AFD; em DAGs finitos rotulados, a linguagem de caminhos é regular (dados finais/inícios adequados).
* **LLM (Large Language Model)**: Modelo de linguagem de larga escala; em orquestração, atua como nó especializado com custos/risco variáveis.
* **Makespan**: Tempo total de conclusão. Em DAGs, aproxima-se do tempo no caminho crítico mais custos de agregação.
* **Matriz de Adjacência (A)**: Em termos simples, uma tabela que mostra quem se conecta com quem. Tecnicamente, representa arestas; A^ℓ indica alcançabilidade em ℓ passos (aritmética booleana, usando operações lógicas, ou ponderada, somando pesos).
* **Matriz de Incidência (B)**: Relaciona nós e arestas; útil para conservação/fluxo e diagnósticos estruturais.
* **Matriz Fundamental (N)**: Em termos simples, quantifica quantas vezes “visitamos” estados antes do fim. Tecnicamente, N=(I−Q)^{-1}, onde I é a identidade e Q é a submatriz de transição entre estados transitórios; fornece tempos esperados e probabilidades de absorção.
* **Message Passing (GNN)**: Fase de envio/agregação de mensagens entre vizinhos com funções diferenciáveis; base das GNNs modernas.
* **Multimodalidade**: Capacidade de integrar múltiplas formas de dados (texto, voz, imagem, tabelas).\index{multimodalidade}
* **Logging (Trilhas de Auditoria)**: Registro estruturado por execução e por nó, incluindo identificadores (runId, pathId, nodeId), timestamps, duração, status, política aplicada, modelo e métricas (tokens, custo). Suporta auditoria reprodutível e cálculo de cauda (p95/p99).
* **NLP/PLN**: Processamento de Linguagem Natural; subgrafo para tarefas textuais.
* **Nó (Vértice)**: Em termos simples, é uma etapa da tarefa. Tecnicamente, é uma unidade de computação com assinatura de entradas/saídas e custos/risco associados.\index{nó}
* **Nó Agregador (Fusão)**: Em termos simples, “o ponto que junta resultados”. Tecnicamente, nó que aplica função determinística (ex.: max, média ponderada, votação) sobre ramos.\index{agregador}
* **Nós-gargalo Superconectados**: Nós com alta betweenness/fluxo; exigem replicação e particionamento para evitar saturação.
* **Ordem Topológica**: Em termos simples, a “ordem segura de execução”. Tecnicamente, é uma permutação dos nós que respeita todas as dependências; em DAGs é computável em O(|V|+|E|), onde |V| é o número de nós e |E| o número de arestas.\index{ordem topológica}
* **Ordenação por Camadas**: Execução em “níveis” independentes; atinge limite inferior quando não há dependências cruzadas fora da camada.
* **Orquestração em Grafo**: Em termos simples, é o “mapa” que coordena módulos de IA para trabalhar juntos. Tecnicamente, é o plano de execução definido em um DAG com contratos de I/O, políticas de decisão e logs por trilhas.
* **Paralelismo**: Execução simultânea de nós independentes, reduzindo tempo total de execução.\index{paralelismo}
* **Paralelismo OR**: Disparo simultâneo de ramos, sucesso no primeiro retorno; requer política de cancelamento e controle de cauda.\index{paralelismo OR}
* **Particionamento por Chave**: Divisão de carga por chave (hash/sharding) para escalar nós-gargalo.\index{particionamento}
* **PDA (Autômato com Pilha)**: Modelo com memória de pilha para linguagens com aninhamento (ex.: a^n b^n); necessário quando há escopos arbitrários.
* **Política de Fusão Determinística**: Regras claras e idempotentes (ex.: max, média ponderada, voto) para combinar ramos sem ambiguidade.
* **Políticas por Nó/Globais**: Conjunto de regras aplicadas localmente (por nó) ou ao caminho como um todo (globais), como controle de acesso, limites, anonimização e roteamento de fallback.
* **Anonimização**: Remoção ou mascaramento de dados pessoais/sensíveis em entradas/saídas de nós, preservando utilidade com redução de risco e alinhamento regulatório.
* **Probabilidade de Absorção**: Em termos simples, a chance de terminar em cada final possível. Tecnicamente, B=N·R, onde N é a matriz fundamental e R é a matriz de transição de estados transitórios para estados absorventes; cada linha de B dá as probabilidades de chegar a cada estado final a partir de um estado inicial transitório.
* **Problema da Parada**: Indecidibilidade sobre terminação universal de programas; impõe limites quando nós executam código Turing-completo.
* **Razão p/t**: Em termos simples, prioriza o que tem mais chance de dar certo mais rápido. Tecnicamente, é uma heurística que ordena alternativas por p_i/t_i (probabilidade de sucesso dividida pelo tempo esperado).
* **Redundância de Caminhos**: Número de trilhas disjuntas entre origem e destino; aumenta resiliência a falhas.\index{redundância de caminhos}
* **Replicação Especulativa**: Disparar cópias de uma tarefa para mitigar cauda; cancelar excedentes ao primeiro sucesso.\index{replicação especulativa}
* **Retiming**: Reorganização de dependências temporais mantendo correção, para reduzir latência aparente.
* **SLA**: Service Level Agreement; metas de latência/confiabilidade (ex.: p99) que orientam topologias e políticas.
* **Subgrafo**: Em termos simples, um “módulo” dentro do mapa. Tecnicamente, um conjunto de nós/arestas encapsulados e reutilizáveis com interface estável.
* **Tempo Esperado até Absorção**: Soma das visitas esperadas por estado (N·1); orienta políticas sob incerteza.
* **Teorema de Brent (limite por caminho crítico)**: Em paralelização de expressões, o tempo mínimo é limitado pela profundidade (caminho crítico), independente do número total de operações.
* **Topologia em Estrela**: Múltiplos nós independentes convergindo em agregador central; favorece paralelismo e auditoria.
* **Tracing**: Rastreabilidade do caminho percorrido em uma execução no grafo.
* **Trilha**: Caminho sem repetição de arestas. Útil para auditoria de execuções e contagem de alternativas.
* **Turing-completo**: Capaz de simular máquina de Turing; traz propriedades indecidíveis se não houver restrições (timeouts/cotas).