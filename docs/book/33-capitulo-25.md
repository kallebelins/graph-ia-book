# Orquestração vs. Graph Neural Networks (\glspl{gnn})

---

## O Problema

\glspl{gnn} operam sobre grafos para **aprender representações**; orquestração usa grafos para **executar fluxos**. Como se relacionam e se complementam?[^1]

---

## Diferenças Essenciais

* Orquestração: grafo como plano de execução determinístico/probabilístico.
* \gls{gnn}: grafo como estrutura de dados; mensagens, agregações e aprendizados[^1].

Em termos simples:

- Orquestração decide e executa; GNN observa e aprende padrões para melhorar decisões futuras.

---

## Complementaridade

* \gls{gnn} pode estimar custos/risco por nó/aresta e alimentar orquestração.
* Orquestração fornece subgrafos e métricas como features para \glspl{gnn}.
* Ciclo fechado: predições da \gls{gnn} ajustam pesos de caminhos; logs de execução retroalimentam o treinamento[^2].

---

## Padrões de Integração

- Extraia features estruturais (grau, betweenness), estatísticas de latência e taxa de falha; treine \gls{gnn} (ex.: \gls{gcn}/GraphSAGE — arquiteturas de redes neurais em grafos) para prever custo/risco.
- Use a previsão como pesos de arestas/nós; compute caminho crítico esperado e selecione rota.
- Em produção, atualize o modelo periodicamente e mantenha fallback baseado em heurísticas quando incerteza for alta.

```{=latex}
\begin{figure}[ht]
\centering
% GNN informa pesos de orquestração (mensagens → pesos → seleção de caminho)
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  % Grafo para GNN (esquerda)
  \node (g1) {g$_1$};
  \node (g2) [above right=0.8cm and 1.6cm of g1] {g$_2$};
  \node (g3) [below right=0.8cm and 1.6cm of g1] {g$_3$};
  \path[->]
    (g1) edge (g2)
    (g1) edge (g3)
    (g2) edge[bend left=15] (g3);
  \node (nn) [right=2.2cm of g2] {\gls{gnn}};
  \path[->] (g2) edge (nn) (g3) edge (nn) (g1) edge (nn);
  % Orquestração (direita)
  \node (o1) [right=2.6cm of nn] {O$_1$};
  \node (o2) [below=of o1] {O$_2$};
  \node (o3) [right=of o1] {O$_3$};
  \path[->]
    (o1) edge node[above]{w$_{13}$} (o3)
    (o2) edge node[below]{w$_{23}$} (o3);
\end{tikzpicture}
\caption{GNN informando pesos de orquestração para seleção de caminho}
\label{fig:cap25-gnn-pesos}
\end{figure}
```

---

## Exemplo Reprodutível

Treinar \gls{gnn} para prever latência esperada por nó a partir de grau, betweenness e histórico; orquestração escolhe caminho com menor makespan esperado. Compare com baseline que usa médias móveis sem aprendizado.

---

## Conclusão

Orquestração e GNNs são **complementares**: uma executa decisões, a outra as informa via aprendizado. Esse acoplamento permite **orquestrações auto-otimizáveis** sem abrir mão de controle e governança.

---

## Exercícios – Parte VII {-}

1) Liste features estruturais e operacionais úteis para prever latência por nó; justifique cada uma.

2) Compare GCN e GraphSAGE em termos de custo/escala para grafos de orquestração.

3) Defina uma loss que penalize escolhas de caminho ruins (latência acima do p95 desejado) e explique como treinar a GNN.

4) Projete um experimento A/B em produção para avaliar a troca “GNN vs baseline heurístico” com métricas de SLA.

5) Discuta riscos de drift e estratégias de recalibração/retreinamento.

[^1]: Z. Wu et al., “A Comprehensive Survey on Graph Neural Networks,” IEEE TNNLS, 2021.
[^2]: M. E. J. Newman, Networks: An Introduction, OUP, 2010.


