# Grafos, Autômatos e Linguagens Formais

---

## O Problema

Como relacionar a orquestração em grafos com **autômatos** e **linguagens formais**? Queremos caracterizar fluxos admissíveis como **linguagens de caminhos** e discutir quando os grafos induzem **autômatos finitos**, **autômatos com pilha** ou modelos mais expressivos.

---

## Definições

* Alfabeto $\Sigma$: conjunto de rótulos de arestas.
* Linguagem de caminhos $\mathcal{L}(G)$: conjunto de sequências de rótulos ao longo de caminhos válidos.
* Autômato induzido: $A=(Q,\Sigma,\delta,q_0,F)$ com $Q=V$, transições de $\delta$ dadas por $E$.

Em termos simples:

- A “linguagem de caminhos” é o conjunto de sequências de passos permitidos no grafo.
- Um autômato finito é uma máquina que lê esses passos e decide se a sequência é válida.

---

## Tese

> **DAGs com rótulos finitos induzem linguagens regulares sob apropriação de estados finais.**

---

## Proposição 1 (Regularidade em DAGs)

Se $G$ é um DAG finito com rótulos em $\Sigma$ e $F\subseteq V$ é o conjunto de estados finais, então $\mathcal{L}(G)$ é regular[^1][^2].

**Esboço.** Não há ciclos; o grafo corresponde a um \gls{afnd} (autômato finito não determinístico) finito; toda linguagem reconhecida por \gls{afnd} é regular.

---

## Proposição 2 (Necessidade de Pilha)

Quando o fluxo exige **balanceamento** (ex.: emparelhamento de chamadas/retornos arbitrários), a linguagem deixa de ser regular; é necessária memória adicional (autômato com pilha).

---

## Fechamentos e construções

- As linguagens de caminhos de DAGs são fechadas por **união** (grafo disjunto com novo inicial) e por **concatenação** (ligações de finais de $G_1$ aos inícios de $G_2$). Fechamento por **estrela de Kleene** (repetição arbitrária de um padrão) requer ciclos e pode violar a aciclicidade; portanto, não está contida em DAGs[^2].
- Construção de AFND: cada nó é estado; cada aresta rotulada é transição. Estados iniciais/finais são escolhidos por contrato de fluxo[^1].
- Minimização para \gls{afd} (autômato finito determinístico) pode ser feita após determinização (subset construction, construção por subconjuntos) se desejado para verificação de políticas.

---

## Exemplo com pilha (pushdown, uso de uma pilha como memória)

Considere a linguagem de chamadas/retornos balanceados $\mathcal{L}=\{ a^n b^n : n\ge 1 \}$. Não é regular; requer **pilha** para contar e casar símbolos. Em orquestração, isso corresponde a subfluxos com **escopos** aninhados arbitrariamente (ex.: abrir contexto → processar → fechar contexto). Modelar diretamente em DAG exige **desenrolar** até uma profundidade fixa (aproximação), perdendo generalidade.

---

## Implicações para políticas

- Políticas regulares (listas brancas de sequências válidas) mapeiam-se bem a DAGs rotulados.
- Políticas com aninhamento não limitado requerem \gls{pda}/CFGs; se o sistema exige apenas profundidade limitada, um DAG expandido é suficiente.
- A validação estática pode operar sobre AFND/AFD derivado do grafo para checar conformidade de execuções planejadas.

---

## Conclusão

Grafos induzem autômatos adequados ao nível de expressividade requerido: DAGs → regulares; dependências aninhadas → pilha; casos com contagem não limitada → modelos mais ricos. Na engenharia, escolha o formalismo mínimo que satisfaça a política desejada e o risco operacional.

```{=latex}
\begin{figure}[ht]
\centering
% Autômato induzido por grafo rotulado (AFND simples)
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (q0) {q$_0$};
  \node (q1) [right=of q0] {q$_1$};
  \node (q2) [right=of q1] {q$_2$};
  \path[->]
    (q0) edge node[above]{a} (q1)
    (q0) edge[bend left=20] node[below]{b} (q2)
    (q1) edge node[above]{c} (q2);
\end{tikzpicture}
\caption{AFND induzido por grafo rotulado: exemplo simples}
\label{fig:cap19-afnd}
\end{figure}
```

---

[^1]: T. H. Cormen et al., Introduction to Algorithms, 3ª ed., MIT Press, 2009. (construções e representações)
[^2]: J. A. Bondy e U. S. R. Murty, Graph Theory, Springer GTM, 2008. (fundamentos)


