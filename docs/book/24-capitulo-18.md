# Álgebra de Grafos: Matrizes de Adjacência e Incidência\index{álgebra de grafos}\index{matriz de adjacência}\index{matriz de incidência}

---

## O Problema

Para analisar e verificar propriedades de orquestrações em grafos, precisamos de ferramentas **algébricas** que permitam computar caminhos, graus, conectividade e checagens de aciclicidade de forma sistemática.

---

## Definições\index{definição}

\begin{definicao}[Matrizes de adjacência e incidência]\label{def:cap18-adj-inc}
Seja $G=(V,E)$ com $|V|=n$.

\begin{itemize}
\item Matriz de adjacência $A\in\{0,1\}^{n\times n}$: $A_{ij}=1$ se $(v_i,v_j)\in E$, caso contrário $0$.
\item Matriz de incidência $B\in\{-1,0,1\}^{n\times m}$ para $m=|E|$: para aresta $e_k=(v_i,v_j)$, $B_{ik}=-1$, $B_{jk}=1$, demais $0$.
\end{itemize}
\end{definicao}

Em termos simples:

- A matriz de adjacência é uma tabela que marca ligações entre nós (quem aponta para quem).
- A matriz de incidência mostra, para cada aresta, de onde ela sai e para onde chega.

---

## Propriedades

\begin{proposicao}[Características algébricas]\label{prop:cap18-propriedades}
* Caminhos de comprimento $\ell$: entradas não nulas de $A^\ell$ indicam alcançabilidade.
* Grau de saída de $v_i$: $\sum_j A_{ij}$; grau de entrada: $\sum_j A_{ji}$.
* Aciclicidade em DAGs: existe ordenação topológica $\pi$ tal que $A_{\pi(i)\,\pi(j)}=0$ para $i\geq j$ (forma estritamente triangular superior após permutação).
* Fecho transitivo: $T = I \lor A \lor A^2 \lor \cdots \lor A^{n-1}$ (aritmética booleana) registra alcançabilidade; pode ser obtido por Warshall/Floyd-Warshall em $O(n^3)$ (tempo cúbico no número de nós) ou por elevação repetida otimizada (ver \cite{cormen2009introduction,bondy2008graph}).
* Em pesos positivos, o caminho crítico em DAG pode ser computado via DP sobre ordem topológica; para arestas ponderadas, matrizes de adjacência ponderadas ajudam a inspecionar somas máximas por caminhos (ver \cite{bondy2008graph}).
\end{proposicao}

---

## Exemplo Numérico Reprodutível

Considere $V=\{v_1,v_2,v_3,v_4\}$ e $E=\{(v_1,v_2),(v_1,v_3),(v_2,v_4),(v_3,v_4)\}$.

- $A=\begin{pmatrix}0&1&1&0\\0&0&0&1\\0&0&0&1\\0&0&0&0\end{pmatrix}$.
- $A^2=\begin{pmatrix}0&0&0&2\\0&0&0&0\\0&0&0&0\\0&0&0&0\end{pmatrix}$ (duas trilhas de comprimento 2 de $v_1$ até $v_4$).
- Fecho transitivo (booleana): $T_{1,4}=1$ indica alcançabilidade de $v_1$ a $v_4$.
- Graus: $out(v_1)=2$, $in(v_4)=2$ — sugere que $v_4$ é agregador e possível gargalo.

```{=latex}
\begin{figure}[ht]
\centering
% Exemplo de adjacência/incidência: v1->v2, v1->v3, v2->v4, v3->v4
\begin{tikzpicture}[>=Stealth, node distance=1.6cm]
  \node (v1) {v$_1$};
  \node (v2) [right=of v1] {v$_2$};
  \node (v3) [below=of v2] {v$_3$};
  \node (v4) [right=1.6cm of v2] {v$_4$};
  \path[->]
    (v1) edge (v2)
    (v1) edge (v3)
    (v2) edge (v4)
    (v3) edge (v4);
\end{tikzpicture}
\caption{Adjacência e incidência: ramificação e convergência em DAG}
\label{fig:cap18-adjacencia-incidencia}
\end{figure}
```

---

## Incidência e fluxos

A matriz de incidência $B$ codifica conservação de fluxo: para um vetor de fluxos $f\in\mathbb{R}^m$, $B\,f = s$ representa fontes/sumidouros (pontos de entrada/saída do fluxo). Em orquestração, isso modela balanceamento entre ramos paralelos e agregadores. Métodos lineares permitem detectar inconsistências e dimensionar capacidade.

\begin{proposicao}[Critérios de aciclicidade]\label{prop:cap18-aciclicidade}
Para checar aciclicidade, basta encontrar uma ordem topológica $\pi$ que torne $A$ estritamente triangular superior. De forma equivalente em grafos finitos, existe $k$ tal que $A^k = 0$ (nilpotência); além disso, qualquer entrada não nula na diagonal de $A^{\ell}$ para algum $\ell\ge 1$ indica a presença de ciclo. Em aritmética booleana, isso se traduz na ausência de entradas diagonais verdadeiras em todas as potências $A^{\ell}$ com $1\le \ell < n$ (ver \cite{bondy2008graph,kahn1962topological}).
\end{proposicao}

---

## Aplicações à Orquestração

* Verificar alcançabilidade entre módulos.
* Detectar gargalos (nós com alto grau de entrada/saída) e agregadores críticos.
* Garantir aciclicidade antes da execução (triangularização por permutação topológica).
* Estimar número de trilhas entre dois módulos (entradas de $A^\ell$ e somas sobre $\ell$) para avaliar redundância/fallback.

---

## Conclusão

A álgebra de grafos fornece um **kit de ferramentas** para analisar e otimizar orquestrações: de alcançabilidade e graus ao diagnóstico de ciclos e cálculo de caminhos críticos. Em combinação com fecho transitivo e incidência, obtemos verificações estruturais robustas e diretamente operacionais para engenharia de fluxos.

---

## Exercícios – Parte V {-}

1) Prove que para qualquer DAG existe uma permutação dos vértices que torna a matriz de adjacência estritamente triangular superior. Dê um algoritmo que encontra tal permutação e analise seu custo.

2) Considere um grafo em diamante (várias fontes convergindo em um agregador). Mostre como o número de trilhas de comprimento 2 aparece em A^2 e relacione com redundância e gargalos.

3) Para um conjunto de k nós independentes com tempo t e um agregador de custo $\alpha$, compare formalmente $T_{\text{chain}}$ e $T_{\text{DAG}}$ e mostre como árvores de redução mudam o caminho crítico para $O(\log k)$.

4) Dado A booleana de um DAG com n nós, mostre que existe k < n tal que A^k = 0. Mostre também que entradas diagonais não nulas em A^ℓ implicam ciclos.

5) Defina uma função de custo que penalize graus de entrada altos em agregadores (ex.: $\lambda\cdot in(v)$) e proponha um critério para identificar nós críticos combinando graus e betweenness.

[^1]: T. H. Cormen et al., Introduction to Algorithms, 3ª ed., MIT Press, 2009.
[^2]: J. A. Bondy e U. S. R. Murty, Graph Theory, Springer GTM, 2008.
[^4]: A. B. Kahn, “Topological Sorting of Large Networks,” CACM, 1962.


