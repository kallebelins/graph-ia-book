# Escalabilidade e Concorrência

---

## O Problema

Sistemas de Inteligência Artificial modernos não operam apenas sobre poucas entradas; eles precisam lidar com **grandes volumes de dados** e **múltiplos usuários simultâneos**.

Exemplos típicos incluem:

* Motores de recomendação processando milhões de consultas.
* Assistentes virtuais atendendo milhares de usuários em paralelo.
* Pipelines de análise em tempo real com múltiplas fontes multimodais.

Quando implementados como **chains lineares**, esses sistemas apresentam sérias limitações:

* **Execução sequencial**: cada etapa espera a anterior terminar, mesmo quando não há dependência lógica.
* **Gargalos centralizados**: um nó lento compromete todo o pipeline.
* **Escalabilidade fraca**: adicionar novos módulos aumenta tempo de resposta linearmente.

Assim, a escalabilidade de *chains* é rigidamente limitada pela sua própria arquitetura sequencial.

---

## A Tese

> **Grafos permitem concorrência natural e escalabilidade superior, pois possibilitam a execução paralela de nós independentes e a convergência de fluxos, reduzindo tempo de resposta e custo computacional.**[^1]

---

## Fundamentação

### Chains e Escalabilidade Linear

Seja um pipeline com $n$ módulos, cada um com tempo médio $t$.

* Tempo total de execução:

$$
T_{chain} = \sum_{i=1}^{n} t_i \approx n \cdot t
$$

* Crescimento é linear com o número de etapas.

### Grafos e Execução Paralela

Seja um grafo $G = (V,E)$.

* Se dois nós $v_i, v_j \in V$ não compartilham dependência ($(v_i,v_j) \notin E$ e não há caminho entre eles), eles podem ser executados em paralelo.
* Tempo total:

$$
T_{grafo} = \max_{p \in P} \sum_{v \in p} t_v
$$

onde $P$ é o conjunto de caminhos possíveis.

* O tempo deixa de ser proporcional ao número total de nós, e passa a ser limitado apenas pelo **caminho crítico** (*critical path*).

### Convergência e Balanceamento

* Grafos permitem que múltiplos caminhos convirjam em um nó comum.
* Isso evita duplicação de processamento.
* O balanceamento de carga se torna mais simples, pois nós podem ser escalados horizontalmente de forma independente.

---

## Prova Teórica

**Exemplo comparativo:**

* **Pipeline Linear:**
  5 nós independentes, cada um com tempo $t = 1s$.

  * Tempo total:

  $$
  T_{chain} = 5 \times 1s = 5s
  $$

* **Grafo Paralelo:**
  Os mesmos 5 nós, mas independentes, conectados a um nó agregador.

  * Tempo total:

  $$
  T_{grafo} = \max(1s,1s,1s,1s,1s) + t_{agregador}
  $$

  Supondo $t_{agregador} = 0.5s$:

  $$
  T_{grafo} = 1s + 0.5s = 1.5s
  $$

**Resultado:**
O grafo reduziu o tempo em mais de 70% em relação ao chain.

**Critérios de reprodutibilidade:**

- Assumimos tempos determinísticos: $t_i = 1s$ para $i=1..5$ e $t_{agregador} = 0.5s$.
- Para variações, considere $t = [0.8s, 1.3s, 0.9s, 1.1s, 0.7s]$:
  * $T_{chain} = 0.8 + 1.3 + 0.9 + 1.1 + 0.7 = 4.8s$.
  * $T_{grafo} = \max(0.8, 1.3, 0.9, 1.1, 0.7) + 0.5 = 1.3 + 0.5 = 1.8s$.

**Formalização:**

* Chains: $O(n)$.
* Grafos (com paralelismo perfeito): $O(\log n)$ em alguns cenários, ou limitado pelo caminho crítico[^1].

---

## Discussão

Essa propriedade dos grafos tem impactos profundos:

* **Desempenho prático:** sistemas com grafos respondem em tempo menor mesmo sob alta carga.
* **Eficiência de custo:** menor tempo de execução → menos tokens consumidos em chamadas a modelos → menor custo operacional.
* **Elasticidade:** cada nó pode ser escalado de forma independente, sem replicar o pipeline inteiro.
* **Resiliência:** gargalos são isolados a nós específicos, e não ao fluxo como um todo.

Além disso, a modelagem em grafos permite integrar facilmente **infraestrutura distribuída** (clusters, containers, microserviços), pois os nós podem ser mapeados diretamente a serviços independentes.

---

## Conclusão

A escalabilidade limitada dos pipelines lineares não é apenas um problema de implementação, mas uma **restrição estrutural** da arquitetura sequencial.
Demonstramos que os grafos, ao permitirem execução paralela e convergência modular, oferecem **escalabilidade superior, concorrência natural e melhor aproveitamento de recursos**.

Nos próximos capítulos, veremos como essas propriedades se estendem a outros aspectos, como **recuperação e resiliência** (Capítulo 7), e **integração multimodal** (Capítulo 8), consolidando a tese de que os grafos são a arquitetura mais adequada para sistemas de IA robustos.

```{=latex}
\begin{figure}[ht]
\centering
% Fan-out paralelo com agregador (estrela com merge)
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (s) {Start};
  \node (a) [right=of s] {A};
  \node (b) [above right=1.0cm and 1.6cm of s] {B};
  \node (c) [below right=1.0cm and 1.6cm of s] {C};
  \node (m) [right=2.0cm of a] {Agregador};
  \node (t) [right=of m] {End};
  \path[->]
    (s) edge (a)
    (s) edge (b)
    (s) edge (c)
    (a) edge (m)
    (b) edge (m)
    (c) edge (m)
    (m) edge (t);
\end{tikzpicture}
\caption{Fan-out paralelo com agregador (estrela com merge)}
\label{fig:cap6-fanout-agregador}
\end{figure}
```

---

[^1]: R. P. Brent, “The Parallel Evaluation of General Arithmetic Expressions,” JACM, 1974.