# Piloto: \gls{gnn} para Seleção de Caminho

---

## Objetivo

Demonstrar, em piloto conceitual, como uma \gls{gnn} pode prever custos/risco e **guiar a seleção de caminho** na orquestração[^1].

---

Em termos simples:

- Treinamos uma \gls{gnn} para estimar custos/risco nos nós e usamos essas estimativas para escolher o melhor caminho.

## Setup Conceitual

* Grafo de orquestração com múltiplos caminhos alternativos.
* Features por nó/aresta: grau, betweenness, histórico de latência, taxa de falha[^2].
* Rótulos: latência observada; objetivo: regressão de latência esperada.
* Divisão temporal: treinar em janelas históricas, validar em janela futura.

---

## Procedimento

1. Extrair features estruturais e históricas.
2. Treinar \gls{gnn} (ex.: GraphSAGE/\gls{gcn}, modelos de message passing em grafos) para prever latência por nó.
3. Orquestração escolhe caminho de menor makespan esperado (caminho crítico com pesos previstos).
4. Implantar em canário; coletar métricas e comparar com baseline heurístico.

```{=latex}
\begin{figure}[ht]
\centering
% Piloto: grafo com dois caminhos alternativos e pesos previstos (latência)
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (s) {S};
  \node (a) [right=of s] {A};
  \node (b) [below=of a] {B};
  \node (t) [right=2.2cm of a] {T};
  \path[->]
    (s) edge node[above]{\small 0.9s} (a)
    (s) edge node[right]{\small 0.8s} (b)
    (a) edge node[above]{\small 0.6s} (t)
    (b) edge node[below]{\small 0.7s} (t);
\end{tikzpicture}
\caption{Piloto \gls{gnn}: pesos previstos orientando a escolha de caminho}
\label{fig:cap26-piloto-pesos}
\end{figure}
```

---

## Métricas de Avaliação

- RMSE/MAE (erros quadrático e absoluto médios) da previsão de latência por nó.
- Redução de latência p50/p95/p99 end-to-end (percentis 50/95/99 do tempo total).
- Aumento de taxa de sucesso sob fallback probabilístico.
- Estabilidade: variação na escolha de caminhos; robustez a drift (mudança de distribuição ao longo do tempo).

---

## Exemplo Reprodutível

Usar os tempos dos capítulos 6 e 10 como base; gerar variações sintéticas; avaliar $R^2$ da previsão e comparar caminho escolhido com baseline sem \gls{gnn}. Registre trilhas para auditoria do impacto das predições. Um stub de preparação de dados está em `examples/gnn_synthetic_demo.py`.

---

## Conclusão

O piloto ilustra a **ponte prática** entre aprendizado em grafos e execução orquestrada, apontando para sistemas auto-otimizáveis. Com métricas adequadas e governança, o acoplamento permanece seguro e auditável.

---

## Estudo de Caso – Piloto com Dados Sintéticos {-}

Objetivo: Treinar uma \gls{gnn} simples para prever latência esperada por nó e comparar escolha de caminhos com baseline.

Dataset sintético:

- Nós com features: grau in/out, betweenness aproximada, média/variância de latência.
- Rótulo: latência observada por nó em janelas de tempo.

Protocolo:

1) Gerar grafos pequenos (10–30 nós) com 2–3 caminhos alternativos.
2) Extrair features e dividir temporalmente (treino/validação).
3) Treinar GCN/GraphSAGE para regressão de latência esperada.
4) Usar predições como pesos para caminho crítico esperado e escolher rota.
5) Comparar com baseline (média móvel) em p50/p95/p99 e taxa de sucesso sob fallback.

[^1]: Z. Wu et al., “A Comprehensive Survey on Graph Neural Networks,” IEEE TNNLS, 2021.
[^2]: M. E. J. Newman, Networks: An Introduction, OUP, 2010.


