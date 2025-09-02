# Aplicações Demonstrativas

---

## O Problema

Até aqui, estabelecemos teoricamente que grafos oferecem **maior expressividade, modularidade, resiliência, escalabilidade, explicabilidade e governança** do que chains.
Mas uma tese só se sustenta plenamente quando pode ser **demonstrada em casos concretos**.

Neste capítulo, aplicaremos a comparação em **três domínios distintos**:

1. **Turismo (concierge de viagens)**
2. **Finanças (detecção de fraude e risco)**
3. **Saúde (triagem inteligente de pacientes)**

Cada domínio será analisado com:

* O problema prático.
* A abordagem com chain.
* A abordagem com grafo.
* Os resultados comparativos.

---

## Caso 1 – Turismo: Concierge de Viagens

### Problema

Um sistema de concierge deve responder a perguntas do usuário sobre:

* destinos turísticos,
* hotéis,
* restaurantes,
* clima,
* eventos locais.

### Chain Linear

* Fluxo típico:

  1. Interpretar pergunta →
  2. Consultar API de hotéis →
  3. Consultar API de clima →
  4. Gerar resposta final.
* Limitações:

  * Execução sequencial → cada chamada depende da anterior.
  * Falha em uma API → quebra o fluxo.
  * Não há integração multimodal (ex.: imagem de hotel).

### Grafo

* Fluxo em grafo:

  * Pergunta →

    * Subgrafo A: hotéis (API A, API B em paralelo)
    * Subgrafo B: clima (consultas paralelas)
    * Subgrafo C: eventos (consulta multimodal em agenda + imagens)
    * Nó de fusão → resposta final.
* Vantagens:

  * Consultas paralelas reduzem latência.
  * Se API de hotéis falha, outra assume (fallback).
  * Pode incluir imagens dos destinos.

### Resultado

* **Chain:** lento, frágil, unidimensional.
* **Grafo:** rápido, resiliente, multimodal.

```{=latex}
\begin{figure}[ht]
\centering
% Caso Turismo: subgrafos para hotéis, clima e eventos com fusão
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (q) {Pergunta};
  \node (h) [right=of q] {Hotéis};
  \node (c) [below=of h] {Clima};
  \node (e) [below=of c] {Eventos};
  \node (m) [right=2.0cm of c] {Fusão};
  \node (r) [right=of m] {Resposta};
  \path[->]
    (q) edge (h)
    (q) edge (c)
    (q) edge (e)
    (h) edge (m)
    (c) edge (m)
    (e) edge (m)
    (m) edge (r);
\end{tikzpicture}
\caption{Concierge de viagens: subgrafos de hotéis, clima e eventos com fusão}
\label{fig:cap11-concierge}
\end{figure}
```

---

## Caso 2 – Finanças: Detecção de Fraude

### Problema

Banco precisa verificar transações suspeitas em tempo real.

* Deve cruzar dados de: geolocalização, histórico do cliente, padrão de compras e redes externas de risco.

### Chain Linear

* Fluxo típico:

  1. Entrada da transação →
  2. Verificação de geolocalização →
  3. Verificação de histórico →
  4. Verificação em rede externa →
  5. Decisão.
* Limitações:

  * Atraso crítico (não pode ser sequencial em tempo real).
  * Uma falha em rede externa paralisa todo pipeline.
  * Difícil auditar qual verificação disparou o alerta.

### Grafo

* Fluxo em grafo:

  * Entrada da transação →

    * Nó 1: geolocalização
    * Nó 2: histórico do cliente
    * Nó 3: redes externas
    * Nó 4: machine learning para padrão estatístico
    * Nó de fusão: decisão final com pesos e explicabilidade.
* Vantagens:

  * Paralelismo → todas verificações ocorrem simultaneamente.
  * Se rede externa falha, os outros nós ainda produzem decisão.
  * Auditoria: grafo registra qual nó disparou alerta de risco.

### Resultado

* **Chain:** lento, frágil, difícil de auditar.
* **Grafo:** rápido, resiliente, auditável, confiável.

---

## Caso 3 – Saúde: Triagem Inteligente de Pacientes

### Problema

Hospital deseja usar IA para triagem inicial de pacientes em pronto-socorro.

* Deve integrar sintomas relatados, histórico eletrônico e exames de imagem.

### Chain Linear

* Fluxo típico:

  1. Sintomas →
  2. Análise textual →
  3. Histórico médico →
  4. Exames →
  5. Classificação final.
* Limitações:

  * Sequência única e rígida.
  * Se exame não estiver disponível, o pipeline falha.
  * Não considera interações multimodais (ex.: histórico + imagem ao mesmo tempo).

### Grafo

* Fluxo em grafo:

  * Entrada →

    * Nó A: sintomas textuais (NLP)
    * Nó B: histórico eletrônico (base de dados)
    * Nó C: exames de imagem (modelo de visão)
    * Nó de fusão: classificador multimodal
    * Nó de decisão: encaminhamento.
* Vantagens:

  * Flexibilidade: mesmo sem exames, sintomas + histórico podem gerar triagem inicial.
  * Multimodalidade: combina visão e linguagem.
  * Explicabilidade: auditor pode ver quais entradas foram decisivas.

### Resultado

* **Chain:** frágil, incompleto, pouco adaptável.
* **Grafo:** flexível, multimodal, seguro para uso clínico.

---

## Discussão

Os três casos mostram um padrão claro:

* **Chains**: simples, mas ineficientes, frágeis e limitados.
* **Grafos**: mais complexos de modelar, mas robustos, auditáveis e adaptáveis[^1].

Ou seja:

* Para protótipos e tarefas simples → chains podem ser suficientes.
* Para aplicações **críticas e em escala** → grafos são indispensáveis.

---

## Conclusão

As aplicações práticas confirmam a tese desenvolvida até aqui:

* **Grafos são a única estrutura capaz de lidar com a complexidade real da IA aplicada.**

Enquanto pipelines lineares se quebram diante da diversidade, volume e falhas, os grafos oferecem:

* paralelismo,
* modularidade,
* resiliência,
* multimodalidade,
* auditabilidade.

No próximo capítulo (Capítulo 12), discutiremos **Agentes Autônomos com Grafos**, mostrando como esse paradigma permite sistemas que não apenas executam fluxos, mas **planejam, adaptam e reorquestram seus próprios caminhos**.

---

[^1]: S. Russell e P. Norvig, Artificial Intelligence: A Modern Approach, 4ª ed., Pearson, 2021.