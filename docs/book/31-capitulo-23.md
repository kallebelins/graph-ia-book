# Métricas Estruturais para IA (diâmetro, centralidades, ciclomática)\index{diâmetro}\index{centralidade}\index{complexidade ciclomática}

---

## O Problema

Como medir e otimizar a **qualidade estrutural** de orquestrações em grafos? Precisamos de métricas que correlacionem **latência**, **robustez** e **explicabilidade** com a topologia.

---

## Definições

* Diâmetro (em DAG sob grafo subjacente não-direcionado): maior distância geodésica.
* Centralidades: grau, betweenness, closeness (proximidade: inverso da distância média a outros nós) adaptadas a DAGs.
* Complexidade ciclomática (em subgrafos com ciclos): $M = E - V + P$ (número de caminhos independentes), com $P$ componentes; para DAG puro, $M=0$[^1].

Em termos simples:

- Diâmetro resume “quantos passos no pior caso” entre dois pontos.
- Betweenness indica quão “atravessado” um nó é pelos caminhos, sinalizando gargalos.
- $M$ próximo de zero significa menos complexidade de ciclos e menor risco de loops.

---

## Métricas e Objetivos

* Menor diâmetro esperado correlaciona com menores latências end-to-end.
* Alta betweenness sugere nós críticos (gargalos) para escalar.
* $M$ próximo de 0 reduz risco de loops e facilita verificação.
* Assortatividade (correlação grau–grau, isto é, semelhança de graus entre vizinhos): alta assortatividade pode concentrar carga em hubs (nós muito conectados); desassortatividade moderada favorece distribuição.
* Redundância de caminhos (número de trilhas disjuntas entre módulos críticos) aumenta resiliência.

---

## Medição e prática

- Meça betweenness apenas em subgrafos ativos (com tráfego) para priorizar capacidade.
- Use fecho transitivo para contar alcançabilidade e mapear superfícies de ataque/falha.
- Combine closeness com latências observadas para identificar nós com "centralidade efetiva" sob rede real.

---

## Exemplo Reprodutível

Grafo com camadas: 5 nós independentes convergindo em agregador. Diâmetro pequeno, betweenness alta no agregador, $M=0$. Se adicionar um segundo agregador redundante, a betweenness de cada um cai e a redundância de caminhos aumenta, reduzindo risco de saturação.

```{=latex}
\begin{figure}[ht]
\centering
% Métricas: um agregador com alta betweenness e diâmetro curto
\begin{tikzpicture}[>=Stealth, node distance=1.6cm]
  \node (s1) {S1};
  \node (s2) [below=of s1] {S2};
  \node (s3) [below=of s2] {S3};
  \node (s4) [below=of s3] {S4};
  \node (s5) [below=of s4] {S5};
  \node (m)  [right=2.0cm of s3] {Agregador};
  \node (t)  [right=of m] {T};
  \path[->]
    (s1) edge (m)
    (s2) edge (m)
    (s3) edge (m)
    (s4) edge (m)
    (s5) edge (m)
    (m) edge (t);
\end{tikzpicture}
\caption{Agregador com alta betweenness e diâmetro curto}
\label{fig:cap23-betweenness-agregador}
\end{figure}
```

---

## Conclusão

Métricas estruturais informam engenharia: dimensionar, escalar e auditar fluxos com base em propriedades topológicas mensuráveis. Ao monitorar essas métricas em produção, a orquestração pode adaptar topologias para metas de SLA.

---

## Exercícios – Parte VII {-}

1) Dado um grafo em camadas com 10 fontes convergindo em um agregador, calcule diâmetro, betweenness do agregador e discuta como um segundo agregador redundante altera essas métricas.

2) Para um grafo dirigido com pesos de latência observada por aresta, proponha uma métrica de “centralidade efetiva” que combine closeness com latências; aplique em um exemplo pequeno e interprete.

3) Em um grafo com um nó superconectado, proponha uma política de particionamento por chave que reduza betweenness observada. Simule o efeito com distribuição de carga.

4) Mostre como o fecho transitivo pode ser usado para estimar “superfície de ataque/falha” (pares alcançáveis) e discuta implicações para governança.

5) Defina um indicador composto que combine redundância de caminhos, diâmetro e betweenness para priorizar refatorações de topologia.

---

## Estudo de Caso – Diagnóstico de Topologia {-}

Objetivo: Dado um fluxo de atendimento (várias entradas → agregador → decisão), medir diâmetro, betweenness e redundância; propor mitigação.

Passos:

- Construir o grafo lógico (nós, arestas) e coletar latências históricas.
- Calcular métricas estruturais (diâmetro, betweenness) no subgrafo ativo.
- Identificar gargalos (nós com alta betweenness) e validar redundância de caminhos para trechos críticos.
- Propor: segundo agregador redundante com política determinística; medir antes/depois.

[^1]: M. E. J. Newman, Networks: An Introduction, OUP, 2010.


