# Processos Estocásticos em Grafos (Markov, Estados Absorventes)

---

## O Problema

Orquestrações reais envolvem **incerteza**: latências variáveis, falhas intermitentes, escolhas probabilísticas. Precisamos de um formalismo para modelar e analisar esses comportamentos.

---

Em termos simples:

- Usamos cadeias de Markov para representar escolhas incertas no fluxo: de cada etapa, há chances de ir para próximas etapas ou terminar (processo estocástico = com incerteza/aleatoriedade controlada).
- A matriz fundamental e as probabilidades de absorção nos dizem quanto tempo esperamos gastar e qual a chance de sucesso.

## Cadeias de Markov

* Estado: nó atual do fluxo (ou par nó,contexto).
* Matriz de transição $P$ (probabilidades de ir de um estado para outro), $P_{ij}=\Pr(v_i\to v_j)$.
* Estados absorventes: estados terminais, sem saída (uma vez neles, o processo termina).

Reordene estados como transitórios $T$ (ainda podem mudar) e absorventes $A$ (terminais); então $P = \begin{pmatrix} Q & R \\ 0 & I \end{pmatrix}$, onde $Q$ é a submatriz de transições entre transitórios, $R$ das transições de transitórios para absorventes.

---

## Métricas Clássicas

* Matriz fundamental: $N=(I-Q)^{-1} = I + Q + Q^2 + \cdots$.
* **Tempo esperado** de permanência por estado: vetor $t = N\,\mathbf{1}$.
* **Probabilidades de absorção**: $B = N\,R$ (cada linha dá as probabilidades de absorver em cada estado final a partir de um transitório).

---

## Exemplo Reprodutível

Considere estados $\{S,A,B,F\}$, com $F$ absorvente. Transições:

- $S\to A$ com 0.6, $S\to B$ com 0.4.
- $A\to F$ com 0.9, $A\to B$ com 0.1.
- $B\to F$ com 0.8, $B\to A$ com 0.2.

Reordene como transitórios $[S,A,B]$ e absorvente $[F]$.

- $Q = \begin{pmatrix}0 & 0.6 & 0.4\\0 & 0 & 0.1\\0 & 0.2 & 0\end{pmatrix}$,
  $R = \begin{pmatrix}0\\0.9\\0.8\end{pmatrix}$.
- $N=(I-Q)^{-1}$; compute $t=N\,\mathbf{1}$ para obter tempo esperado até $F$ a partir de cada transitório.
- $B = N\,R$ dá a probabilidade de sucesso (absorção em $F$) a partir de $S$, $A$, $B$.

Essas grandezas orientam políticas de fallback e priorização de caminhos sob incerteza. Para reproduzir numericamente, veja `examples/markov_demo.py`.

```{=latex}
\begin{figure}[ht]
\centering
% Cadeia de Markov com estado absorvente F
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (s) {S};
  \node (a) [right=of s] {A};
  \node (b) [below=of a] {B};
  \node (f) [right=2.2cm of a] {F};
  \path[->]
    (s) edge node[above]{0.6} (a)
    (s) edge node[right]{0.4} (b)
    (a) edge node[above]{0.9} (f)
    (a) edge node[right]{0.1} (b)
    (b) edge node[above]{0.8} (f)
    (b) edge node[left]{0.2} (a);
\end{tikzpicture}
\caption{Cadeia de Markov com estado absorvente e transições entre transitórios}
\label{fig:cap21-markov-absorvente}
\end{figure}
```

---

## Engenharia de Políticas

- Maximize probabilidade de sucesso escolhendo transições que elevem a linha correspondente em $B$; 
- Minimize tempo esperado preferindo menores entradas em $t$; 
- Equilibre custo vs. risco ponderando $\lambda\,t - (1-\lambda)\,\Pr(\text{sucesso})$ conforme metas do sistema.

A variabilidade e a **cauda de latência** devem ser tratadas por retries/replicação seletiva e tempo de guarda; modelos Markovianos auxiliam a calibrar tais decisões[^1][^2].

---

## Discussão

Markovianos capturam fallback probabilístico, alternância entre nós e estimativas de custo/latência esperada. Em grafos maiores, estimativas podem ser aprendidas (ex.: por \glspl{gnn}) e injetadas como probabilidades/custos, fechando o ciclo de orquestração adaptativa.

---

## Conclusão

Modelos estocásticos adicionam **previsibilidade sob incerteza** à orquestração, permitindo escolhas de caminho informadas por risco e tempo esperado.

---

[^1]: M. E. J. Newman, Networks: An Introduction, OUP, 2010. (processos em redes e random walks)
[^2]: S. Russell e P. Norvig, Artificial Intelligence: A Modern Approach, 4ª ed., Pearson, 2021. (modelos probabilísticos e decisão sob incerteza)


