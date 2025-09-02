# Computabilidade e Decidibilidade na Orquestração

---

## O Problema

Nem toda propriedade de um fluxo é decidível. Precisamos saber **o que é computável** e **quais verificações são decidíveis** para construir orquestrações seguras.

---

## Tese

> **Em DAGs finitos com rótulos finitos, propriedades estruturais como alcançabilidade, aciclicidade e existência de caminho crítico são decidíveis em tempo polinomial.**

Em termos simples:

- Em grafos finitos sem ciclos, conseguimos responder “tem caminho?”, “tem ciclos?” e “qual o tempo mínimo?” de maneira eficiente e garantida.

---

## Propriedades Decidíveis

* Aciclicidade: detecção por ordenação topológica (tempo $O(|V|+|E|)$, linear na soma do número de nós e arestas).
* Alcançabilidade: busca em grafos (BFS, busca em largura / DFS, busca em profundidade) em $O(|V|+|E|)$ (linear em nós mais arestas).
* Caminho crítico: programação dinâmica (DP) em DAG (tempo linear em $|V|+|E|$) dado pesos positivos em nós/arestas.
* Fecho transitivo: via Floyd–Warshall em $O(n^3)$ ou multiplicações booleanas aceleradas[^1][^2].

---

## Limites de Decidibilidade

Quando se permitem **nós com execução arbitrária** (ex.: subprogramas Turing-completos, capazes de simular qualquer computação) e **condições globais** sobre todas as execuções, certas propriedades tornam-se indecidíveis (reduções clássicas ao problema da parada):

- "Para toda entrada, toda execução termina" (terminação universal) — indecidível em geral.
- "Dois workflows são equivalentes para todas as entradas" — indecidível em modelos Turing-completos.
- "Não ocorre violação de política em nenhuma execução possível" — indecidível sem restrições estruturais.

Ao restringir a orquestração a **DAGs finitos** com **nós puramente declarativos** (sem loops internos não-limitados), recuperamos decidibilidade prática para propriedades estruturais[^3].

---

## Estrutura vs. Cálculo

- Propriedades **estruturais** (grafo) → decidíveis e geralmente polinomiais: aciclicidade, alcançabilidade, caminho crítico[^1][^2].
- Propriedades **comportamentais** com código arbitrário → podem ser indecidíveis; mitigar com contratos, timeouts, cota de passos e verificação por casos.

---

## Exemplo Reprodutível

Para um DAG com pesos $t_v$, calcule ordenação topológica e DP: $dist[v]=t_v+\max_{(u,v)\in E}dist[u]$; o máximo em $V$ é o caminho crítico. Isso decide a pergunta "qual o makespan ótimo?" em tempo linear no tamanho do grafo[^1].

Agora modifique um nó para executar um script com laço potencialmente infinito; a pergunta "todas as execuções terminam?" deixa de ser decidível no geral — a menos que imponhamos timeouts/cotas e tratemos estouro como falha, rebaixando o problema a análise estrutural e de políticas locais[^3].

---

## Conclusão

Ao restringir o modelo estrutural (DAG finito) e as propriedades locais, obtemos **decidibilidade prática**; ao liberar computação arbitrária global, emergem **limites clássicos** de indecidibilidade. A engenharia segura equilibra esses aspectos com contratos, timeouts e verificação estática.

```{=latex}
\begin{figure}[ht]
\centering
% Esboço: propriedades decidíveis em DAG vs indecidíveis com computação arbitrária
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (d1) {Aciclicidade};
  \node (d2) [below=of d1] {Alcançabilidade};
  \node (d3) [below=of d2] {Caminho crítico};
  \node (g)  [right=3.0cm of d2] {Gateway};
  \node (u1) [right=3.0cm of g] {Terminação universal?};
  \node (u2) [below=of u1] {Equivalência total?};
  \path[->]
    (d1) edge (g)
    (d2) edge (g)
    (d3) edge (g)
    (g) edge (u1)
    (g) edge (u2);
\end{tikzpicture}
\caption{Propriedades decidíveis em DAG vs indecidíveis com computação arbitrária}
\label{fig:cap19-afnd}
\end{figure}
```

---

[^1]: T. H. Cormen et al., Introduction to Algorithms, 3ª ed., MIT Press, 2009.
[^2]: J. A. Bondy e U. S. R. Murty, Graph Theory, Springer GTM, 2008.
[^3]: S. Russell e P. Norvig, Artificial Intelligence: A Modern Approach, 4ª ed., Pearson, 2021.


