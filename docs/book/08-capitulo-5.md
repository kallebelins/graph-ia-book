# Explicabilidade e Auditoria

---

## O Problema

Um dos maiores obstáculos na adoção de sistemas de Inteligência Artificial em ambientes críticos (saúde, finanças, jurídico, segurança) é a **falta de explicabilidade**.

* **Modelos lineares (chains)**, quando executados, produzem apenas o resultado final. O caminho percorrido internamente fica implícito.
* Quando ocorre um erro ou uma decisão inesperada, é difícil responder perguntas como:

  * *Por que o sistema tomou essa decisão?*
  * *Qual etapa produziu o erro?*
  * *Que fontes ou dados influenciaram a resposta?*

Essa falta de rastreabilidade não é apenas um problema técnico, mas também **ético e regulatório**. Normas emergentes de governança de IA (como a **AI Act da União Europeia**, regulamentação europeia para sistemas de IA) exigem que sistemas sejam auditáveis[^1][^2].

Portanto, explicabilidade e auditoria não são opcionais: são **requisitos fundamentais** para a adoção segura de IA.

---

## A Tese

> **Grafos oferecem explicabilidade e auditoria nativas, pois cada execução corresponde a um caminho explícito no grafo, permitindo rastrear decisões, verificar dependências e auditar o sistema.**

---

## Fundamentação

### Rastreabilidade em Chains

* Num pipeline linear, o caminho de execução é sempre o mesmo.
* A auditoria só mostra o que entrou e o que saiu.
* Falhas internas são difíceis de localizar porque não existe representação formal do "subcaminho" da execução.

### Rastreabilidade em Grafos

* Em um grafo, cada execução gera um **caminho único**:

  $$
  P = (v_1, v_2, ..., v_k), \quad (v_i, v_{i+1}) \in E
  $$
* Esse caminho pode ser registrado como **trilha de auditoria** (*execution trace*).
* Além disso, grafos permitem **anotações por aresta** (ex.: decisão condicional, confiança probabilística).

### Auditoria Formal

* Cada nó pode registrar:

  * entrada recebida,
  * saída produzida,
  * fontes consultadas,
  * tempo de execução,
  * custo de tokens.
* Isso gera um **log semântico estruturado** (registros com significado de alto nível, não apenas mensagens livres), essencial para explicabilidade.

---

## Prova Teórica

**Proposição:** Grafos permitem explicabilidade completa, enquanto pipelines lineares só permitem explicabilidade parcial.

* **Em chains:**

  * Execução é determinística, não há bifurcações.
  * Só é possível auditar sequência fixa: $f_1 \to f_2 \to ... \to f_n$.
  * Se $f_3$ produz resultado errado, não há metadados de decisão → impossível saber por quê.

* **Em grafos:**

  * Caminho real é registrado.
  * Decisões condicionais ficam explícitas como escolhas de arestas.
  * Auditoria pode reconstruir toda a execução.

---

## Discussão

Essa diferença tem **implicações práticas diretas**:

* **Compliance regulatório**
  Grafos atendem exigências de explicabilidade em setores regulados (financeiro, médico, jurídico).

* **Debugging** (depuração) **e manutenção**
  Engenheiros podem identificar em que nó ocorreu a falha e reproduzir o estado local.

* **Transparência para usuários**
  Um assistente pode explicar: *"consultei base A, descartei base B e escolhi resposta C com 80% de confiança"*.

* **Accountability**
  Empresas podem provar que o sistema tomou decisões dentro de políticas predefinidas.

---

## Conclusão

A ausência de explicabilidade é uma das críticas mais fortes à IA atual.
Mostramos neste capítulo que:

* **Pipelines lineares** oferecem apenas rastreabilidade superficial.
* **Grafos**, por sua própria natureza, geram caminhos de execução auditáveis, permitindo explicabilidade real.

Portanto, a aplicação de grafos na orquestração de IA não é apenas uma escolha técnica, mas uma **exigência para governança, confiança e adoção em larga escala**.

No próximo capítulo, analisaremos **Escalabilidade e Concorrência**, demonstrando como grafos permitem paralelismo natural e reduzem custos computacionais, enquanto pipelines lineares se tornam gargalos.

```{=latex}
\begin{figure}[ht]
\centering
% Trilha de auditoria: caminho percorrido com anotações
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (v1) {v$_1$};
  \node (v2) [right=of v1] {v$_2$};
  \node (v3) [right=of v2] {v$_3$};
  \node (v4) [right=of v3] {v$_4$};
  \path[->]
    (v1) edge node[above]{conf=0.92} (v2)
    (v2) edge node[above]{policy=OK} (v3)
    (v3) edge node[above]{lat=120ms} (v4);
\end{tikzpicture}
\caption{Trilha de auditoria: anotações por aresta (confiança, política, latência)}
\label{fig:cap5-trilha-auditoria}
\end{figure}
```

---

[^1]: European Parliament and Council of the European Union, Artificial Intelligence Act, 2024.
[^2]: S. Russell e P. Norvig, Artificial Intelligence: A Modern Approach, 4ª ed., Pearson, 2021.