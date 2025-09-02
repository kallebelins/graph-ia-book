# Execução como Trilhas e Caminho Crítico (Paths, Walks, DAG Scheduling)\index{trilha}\index{caminho crítico}\index{DAG scheduling}

---

## O Problema

Executar fluxos em grafos exige **ordenação válida** (topológica), **controle de dependências** e **otimização do tempo total**. Em chains, a ordem é trivial; em grafos, é necessário raciocinar em termos de **trilhas e caminhos** e minimizar o **makespan**.

---

## Definições\index{definição}

* Caminho: sequência de nós $(v_1,\ldots,v_k)$ tal que $(v_i,v_{i+1})\in E$.\index{caminho}
* Trilha: caminho em que arestas não se repetem.\index{trilha}
* Caminho crítico: caminho de maior duração somada.\index{caminho crítico}
* Escalonamento em DAG: execução em ordem topológica respeitando dependências.\index{ordem topológica}

Em termos simples:

- Caminho crítico é o “trajeto mais demorado” que limita o tempo total.
- A ordem topológica é a sequência segura que respeita todas as dependências, evitando ciclos.

---

## Tese

> **O tempo de execução total em um DAG é limitado inferiormente pela duração do caminho crítico.**

---

## Proposição 1 (Limite Inferior)

\begin{proposicao}[Limite inferior do makespan]\label{prop:cap17-limite-inferior}
Seja $G=(V,E)$ com tempos $t_v>0$. Para qualquer escalonamento válido, $T(G)\geq \max_{p\in P}\sum_{v\in p} t_v$.
\end{proposicao}

---

## Proposição 2 (Escalonamento Ótimo em Caso Independente)

\begin{proposicao}[Camadas independentes]\label{prop:cap17-camadas-independentes}
Para $V$ particionado em camadas independentes, o escalonamento por camadas atinge o limite inferior até $t_{agg}$.
\end{proposicao}

---

## Resultados adicionais

- Uma **ordem topológica** pode ser obtida em tempo $O(|V|+|E|)$ (isto é, linear na soma do número de nós $|V|$ e de arestas $|E|$); a partir dela, o caminho crítico em DAGs com pesos em nós é computável por **programação dinâmica** no mesmo tempo[^1][^2].
- Em ambientes heterogêneos (vários processadores), heurísticas como \gls{heft} (Heuristic Earliest Finish Time) aproximam escalonamentos eficientes com baixa complexidade[^3].
- Retiming e reordenação de dependências podem reduzir latência aparente sem alterar correção estrutural[^4].

---

## Exemplo Numérico Reprodutível

Seja $V=\{A,B,C,D,E\}$, com tempos $(1.0,1.3,0.9,1.1,0.7)$ e um nó agregador $F$ com $t_F=0.5$.

- Independência: $A,B,C,D,E$ sem dependências; todos apontam para $F$.
- Ordem topológica: $[A,B,C,D,E,F]$.
- DP (programação dinâmica): $dist[x]=t_x$ para fontes; $dist[F]=t_F+\max(dist[A],\ldots,dist[E])=0.5+1.3=1.8$.
- $T_{chain}=5.0$; $T_{DAG}=1.8$.

Varie pesos para observar robustez do ganho; em particular, se um nó domina a cauda, convém replicação especulativa apenas nesse ramo[^5].

---

## Discussão

O modelo de trilhas permite: (i) rastreabilidade; (ii) detecção de gargalos via caminho crítico; (iii) balanceamento de carga por camadas.

Em plataformas heterogêneas, combine ordem topológica com heurísticas de mapeamento (ex.: HEFT) e políticas de fila para reduzir makespan esperado[^3]. Métricas como betweenness ajudam a identificar agregadores críticos a serem escalados[^6].

---

## Conclusão

Executar como trilhas e otimizar pelo caminho crítico transforma a orquestração em um problema clássico de **scheduling em DAGs** (escalonamento de tarefas respeitando dependências), com benefícios claros de desempenho e governança. A computação linear do caminho crítico e heurísticas práticas permitem ganhos consistentes em ambientes reais.

```{=latex}
\begin{figure}[ht]
\centering
% DAG em camadas e ordem topológica
\begin{tikzpicture}[>=Stealth, node distance=1.6cm]
  % Camada 1
  \node (a) {A}; \node (b) [below=of a] {B};
  % Camada 2
  \node (c) [right=2.0cm of a] {C};
  \node (d) [right=2.0cm of b] {D};
  % Camada 3
  \node (e) [right=2.0cm of c] {E};
  % Arestas
  \path[->]
    (a) edge (c)
    (b) edge (c)
    (b) edge (d)
    (c) edge (e)
    (d) edge (e);
\end{tikzpicture}
\caption{DAG em camadas e execução por ordem topológica}
\label{fig:cap17-dag-camadas}
\end{figure}
```

---

## Exercícios – Parte V {-}

1) Dado um DAG com pesos em nós, descreva um algoritmo em O(|V|+|E|) para computar o caminho crítico usando ordem topológica e programação dinâmica.

2) Em ambiente heterogêneo (vários processadores), descreva o funcionamento do HEFT e proponha uma modificação para reduzir cauda p99.

3) Construa um exemplo numérico com cinco nós e valide, por código, as métricas de makespan em chain e grafo.

4) Mostre que, para camadas independentes, o escalonamento por camadas atinge o limite inferior salvo custo de agregação.

5) Discuta como retiming pode reduzir latência aparente sem alterar correção estrutural, ilustrando com um pequeno grafo.

[^1]: A. B. Kahn, “Topological Sorting of Large Networks,” CACM, 1962.
[^2]: T. H. Cormen et al., Introduction to Algorithms, 3ª ed., MIT Press, 2009.
[^3]: H. Topcuoglu, S. Hariri e M.-Y. Wu, “HEFT,” IEEE TPDS, 2002.
[^4]: C. E. Leiserson e J. B. Saxe, “Retiming Synchronous Circuitry,” Algorithmica, 1991.
[^5]: J. Dean e L. A. Barroso, “The Tail at Scale,” CACM, 2013.
[^6]: M. E. J. Newman, Networks: An Introduction, OUP, 2010.


