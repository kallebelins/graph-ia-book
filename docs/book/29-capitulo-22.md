# Resiliência Probabilística e Fallback: Modelos e Bounds (limites)

---

## O Problema

Sistemas em produção precisam de **garantias** de sucesso ou limites de risco sob falhas parciais.

---

## Modelo

Em termos simples:

- Fallback é ter planos alternativos; compomos as chances de sucesso e o tempo esperado conforme a ordem de tentativa ou execução em paralelo.

* Cada nó $v$ tem probabilidade de sucesso $p_v$ e tempo $t_v$.
* Arestas podem representar tentativas alternativas (fallback) com probabilidades condicionais.
* Para ramos paralelos independentes, probabilidades compõem-se multiplicativamente; para fallback sequencial, usamos complementos.

---

## Métricas

* Probabilidade de sucesso do fluxo: por composição ao longo de caminhos e nós paralelos.
* Limites (bounds) via união de eventos e desigualdades (ex.: Boole, Bonferroni) para upper/lower bounds (limites superiores/inferiores).
* Tempo esperado e variância: para fallback sequencial, condicionar no sucesso/fracasso de cada etapa.

---

## Independência vs. Correlação

- Assumir independência entre caminhos superestima a resiliência quando há **falhas correlatas** (falhas relacionadas por causas comuns, ex.: dependência compartilhada, hotspot de rede). 
- Modele correlação introduzindo nós/arestas compartilhados (causas comuns) ou parâmetros de covariância; quando incerto, use bounds conservadores e sensibilidade de cenários[^1][^2].

---

## Ordem de Fallback

Dado um conjunto de alternativas com $(p_i,t_i)$, ordenar por **maior razão** $\rho_i=\frac{p_i}{t_i}$ tende a maximizar sucesso por unidade de tempo esperado. Em ambientes com cauda de latência pronunciada, prefira ramos de menor variância/cauda antes dos de alto pico[^1].

---

## Exemplos Reprodutíveis

1) Dois caminhos alternativos independentes: $P_1$ com sucesso $0.8$, $P_2$ com sucesso $0.7$.

- Fallback sequencial (tentar $P_1$ e, se falhar, $P_2$):
  Sucesso $=1-(1-0.8)(1-0.7)=0.94$.
  Tempo esperado: $\mathbb{E}[T]=t_1 + (1-0.8)\,t_2$.

2) Três caminhos com $(p,t)=(0.6,200ms),(0.5,120ms),(0.4,80ms)$.

- Ordem por $\rho$: $(0.5/120)>(0.6/200)>(0.4/80)$ → tente na ordem 2→1→3.
- Compare $\mathbb{E}[T]$ e probabilidade total para diferentes ordens e escolha a que otimiza a meta (SLA, acordo de nível de serviço, ou sucesso mínimo).

3) Paralelismo com agregação OR (sucesso se qualquer ramo succeeds): para independentes, $p_{OR}=1-\prod_i(1-p_i)$. Custo de agregação e **cauda** dependem do primeiro retorno bem-sucedido e da política de cancelamento dos demais ramos[^1].

```{=latex}
\begin{figure}[ht]
\centering
% Rede de resiliência: dois caminhos alternativos e um nó de agregação OR
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (start) {Start};
  \node (a1) [right=of start] {A1};
  \node (a2) [right=of a1] {A2};
  \node (b1) [below=of a1] {B1};
  \node (b2) [right=of b1] {B2};
  \node (or) [right=2.0cm of a2] {OR};
  \node (end) [right=of or] {End};
  \path[->]
    (start) edge (a1)
    (a1) edge (a2)
    (a2) edge (or)
    (start) edge (b1)
    (b1) edge (b2)
    (b2) edge (or)
    (or) edge (end);
\end{tikzpicture}
\caption{Resiliência: caminhos alternativos convergindo em agregador OR}
\label{fig:cap22-resiliencia-or}
\end{figure}
```

---

## Engenharia de Resiliência

- Limite superior de falha por união de eventos: $\Pr(\text{falha})\le \sum_i \Pr(\text{falha no ramo }i)$ — útil quando dependências são desconhecidas.
- Reduza compartilhamentos (dependências comuns) entre ramos de fallback para aproximar independência.
- Em produção, caudas dominam: combine paralelismo controlado, timeouts e cancelamento precoce para reduzir p99/p999 (percentis de latência em que 99%/99,9% das requisições respondem até esse tempo; valores altos indicam cauda pesada)[^1].
- Instrumente probabilidade e tempo por trilha; ajuste ordens de fallback dinamicamente.

---

## Relação com Topologia

- Diamante (ramos paralelos que convergem) aumenta resiliência quando ramos são suficientemente independentes.
- Agregadores com alta betweenness tornam-se críticos: mitigue com replicação e escalonamento horizontal[^2].

---

## Conclusão

Ao incorporar probabilidades e fallback, grafos tornam-se um instrumento quantitativo para **resiliência mensurável**. Políticas de ordenação e paralelismo, sensíveis à cauda, elevam a confiabilidade percebida sem explodir custos computacionais.

---

## Exercícios – Parte VI {-}

1) Para duas alternativas independentes com (p,t), derive a ordem ótima por razão p/t e compare com ordenações alternativas numericamente.

2) Mostre que assumir independência entre ramos paralelos pode superestimar resiliência quando há causas comuns; construa um contraexemplo simples.

3) Modele fallback sequencial com três alternativas e derive a expressão do tempo esperado em função dos p_i e t_i.

4) Discuta o impacto da variância/cauda nas políticas de replicação especulativa; proponha uma regra de ativação que considere p99.

5) Em um diamante com OR de caminhos, derive p_OR e discuta como cancelar ramos em voo afeta custo e latência.

---

[^1]: J. Dean e L. A. Barroso, “The Tail at Scale,” Communications of the ACM, 2013.
[^2]: M. E. J. Newman, Networks: An Introduction, OUP, 2010.


