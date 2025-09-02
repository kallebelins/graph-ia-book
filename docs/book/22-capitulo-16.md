# Teoremas de Expressividade: Chains $\subset$ DAGs e Limites\index{expressividade}\index{chain}\index{DAG}

---

## O Problema

Para fundamentar a superioridade estrutural dos grafos, precisamos de **provas formais** estabelecendo a inclusão de chains em DAGs e esclarecendo **limites de expressividade**. Também é necessário explicitar os casos-limite em que a equivalência pode falhar devido a restrições de graus, ciclos e multimodalidade.

---

## A Tese

> **Chains são um subconjunto estrito dos DAGs.**

> **Em termos de expressividade, DAGs permitem ramificações, fusões e paralelismo que não podem ser representados por chains.**

Em termos simples:

- Chains são “linhas retas” de etapas; DAGs são “mapas” com bifurcações e reencontros sem voltas.
- Em grafos, executar módulos em paralelo reduz o tempo total ao limite do caminho crítico, enquanto chains somam todas as etapas.

---

## Definições\index{definição}

* Chain: grafo direcionado acíclico com grau de entrada e saída máximo igual a 1.\index{chain}
* \gls{dag}: grafo direcionado acíclico sem ciclos (graus arbitrários).\index{DAG}

---

## Teorema 1 (Inclusão Estrita)\index{inclusão estrita}

\begin{teorema}[Inclusão estrita: Chains $\subset$ DAGs]\label{thm:cap16-inclusao-estrita}
Todo chain é um DAG; existe DAG que não é chain.
\end{teorema}

\begin{proof}
1. Inclusão: um chain tem uma única sequência de arestas e, por definição, não possui ciclos. Logo, é um DAG.
2. Estrita: considere $G=(V,E)$ com $V=\{v_1,v_2,v_3\}$ e $E=\{(v_1,v_2),(v_1,v_3)\}$. $G$ é DAG, mas não pode ser representado por um chain (grau de saída de $v_1$ maior que 1).
\end{proof}

---

## Teorema 2 (Limite por Graus)\index{grau}

\begin{proposicao}[Limite por graus]\label{prop:cap16-limite-graus}
Para um grafo direcionado simples com $n$ nós, o número máximo de arestas é $n(n-1)$; para um chain com $n$ nós, o máximo é $n-1$.
\end{proposicao}

\begin{proof}
Em grafo direcionado simples, toda ordenação de pares distintos é possível: $n(n-1)$. Em chain, há exatamente uma aresta entre vizinhos consecutivos.
\end{proof}

---

## Teorema 3 (Caminhos e Paralelismo)\index{paralelismo}\index{caminho crítico}

\begin{teorema}[Caminho crítico limita o makespan]\label{thm:cap16-caminho-critico}
Existem DAGs em que o tempo de execução ótimo (makespan) é limitado pelo caminho crítico e independe do número total de nós.
\end{teorema}

\begin{proof}[Esboço da prova]
Para um conjunto de nós independentes, o tempo total é $T=\max_i t_i$ (mais custo de agregação). Em chain, $T=\sum_i t_i$.
\end{proof}

---

## Lema 1 (Serialização Forçada em Chains)
\small{(serialização = executar entradas em sequência, sem paralelizar)}

\begin{lema}[Serialização em chains]\label{lem:cap16-serializacao}
Se um fluxo requer a avaliação de duas hipóteses independentes $h_1$ e $h_2$ e uma decisão de fusão, então qualquer chain que preserve a correção deve serializar as avaliações; em um DAG, as avaliações podem ocorrer em paralelo e convergir para um nó decisor. Logo, existe instância em que $T_{DAG}<T_{chain}$ sob os mesmos custos por nó (ver \cite{kahn1962topological,cormen2009introduction,brent1974parallel}).
\end{lema}

---

## Lema 2 (Fusão e Expressividade)

\begin{lema}[Fusão paralela]\label{lem:cap16-fusao}
Seja um nó de fusão que implementa uma função associativa/comutativa $f$ (ex.: max, média ponderada). Em um chain, a inserção de múltiplas entradas exige serialização estrita; em um DAG, ramos podem agrupar-se em árvores de redução com profundidade $O(\log k)$ para $k$ entradas, reduzindo o caminho crítico (ver \cite{brent1974parallel}).
\end{lema}

---

## Corolário (Multimodalidade)

\begin{corolario}[Fusão multimodal]\label{cor:cap16-multimodal}
Seja um fluxo com entradas texto, visão e áudio. Em chain, é necessário serializar modalidades; em DAG, modalidades são nós paralelos que convergem — estrutura não representável por um único chain sem perda de paralelismo.
\end{corolario}

---

## Exemplo construtivo

Considere $k$ módulos independentes $M_1,\ldots,M_k$ com tempos idênticos $t$ e um agregador $A$ com custo $\alpha$.

- Chain: $T_{chain}=kt+\alpha$.
- DAG (paralelo): $T_{DAG}=t+\alpha$.

Para $k\ge 2$, $T_{DAG}<T_{chain}$. O ganho cresce linearmente em $k$ sob custos fixos de agregação.

Se a fusão exigir redução associativa (ex.: soma), usar árvore de redução binária dá caminho crítico $t\cdot \lceil \log_2 k \rceil + \alpha$, ainda assim assintoticamente menor que $kt$[^3].

---

## Casos-limite e limites

- **Grau limitado:** se graus de saída/entrada forem limitados a 1, o DAG colapsa em chains. Limites de grau restringem o ganho de expressividade.
- **Ciclos:** permitir ciclos altera a classe (não-DAG); resultados de ordenação topológica deixam de valer[^4].
- **Oráculos custosos** (componentes caros/externos, como modelos grandes ou serviços lentos): quando agregadores ou sincronizações dominam o custo, os ganhos de paralelismo podem desaparecer; a análise deve considerar a cauda de latência e variância[^5][^6].

---

## Discussão

Os teoremas e lemas mostram que a vantagem dos DAGs não é apenas de conveniência, mas **estrutural**. Em particular: (i) bifurcação e convergência ampliam expressividade; (ii) limites de grau permitem múltiplas relações; (iii) o tempo é reduzido pelo caminho crítico; (iv) árvores de redução melhoram a assimetria serialização/fusão.

---

## Conclusão

Consolidamos formalmente: $Chains \subsetneq DAGs$. Modelos realistas precisam considerar custos de agregação e variância. Ainda assim, para classes amplas de fluxos, DAGs possibilitam expressividade e desempenho inalcançáveis por chains.

```{=latex}
\begin{figure}[ht]
\centering
% Chains ⊂ DAGs: comparação visual (parte 1)
\begin{tikzpicture}[>=Stealth, node distance=1.6cm]
  % Chain
  \node (c1) {v$_1$};
  \node (c2) [right=of c1] {v$_2$};
  \node (c3) [right=of c2] {v$_3$};
  \path[->] (c1) edge (c2) (c2) edge (c3);
  \node[below=0.3cm of c2] {Chain};
\end{tikzpicture}
\caption{Chains: caminho único e grau máximo 1}
\label{fig:cap16-chain}
\end{figure}
```

```{=latex}
\begin{figure}[ht]
\centering
% DAG
\begin{tikzpicture}[>=Stealth, node distance=1.6cm]
  \node (d1) {v$_1$};
  \node (d2) [right=of d1] {v$_2$};
  \node (d3) [below=of d2] {v$_3$};
  \node (d4) [right=1.6cm of d2] {v$_4$};
  \path[->] (d1) edge (d2) (d1) edge (d3) (d2) edge (d4) (d3) edge (d4);
  \node[below=0.3cm of d2] {DAG (mais expressivo)};
\end{tikzpicture}
\caption{DAG: ramificação e convergência (mais expressivo que chain)}
\label{fig:cap16-dag}
\end{figure}
```

---

[^1]: A. B. Kahn, “Topological Sorting of Large Networks,” CACM, 1962.
[^2]: T. H. Cormen et al., Introduction to Algorithms, 3ª ed., MIT Press, 2009.
[^3]: R. P. Brent, “The Parallel Evaluation of General Arithmetic Expressions,” JACM, 1974.
[^4]: J. A. Bondy e U. S. R. Murty, Graph Theory, Springer GTM, 2008.
[^5]: J. Dean e L. A. Barroso, “The Tail at Scale,” Communications of the ACM, 2013.
[^6]: L. A. Barroso, J. Clidaras e U. Hölzle, The Datacenter as a Computer, 2ª ed., Morgan & Claypool, 2013.


