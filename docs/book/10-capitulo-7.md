# Recuperação e Resiliência\index{resiliência}\index{fallback}

---

## O Problema

Sistemas de Inteligência Artificial, especialmente em produção, são inerentemente sujeitos a falhas:

* falhas técnicas (timeout em API, indisponibilidade de rede),
* falhas lógicas (alucinação de modelos de linguagem),
* falhas de dados (entrada corrompida, ausência de contexto),
* falhas humanas (inputs incorretos ou maliciosos).

Nos **pipelines lineares (chains)**, uma falha em qualquer etapa interrompe o fluxo inteiro:

$$
f = f_n \circ f_{n-1} \circ ... \circ f_1
$$

Se $f_k$ falha, o pipeline todo retorna erro, pois não há mecanismos internos de recuperação.
Isso leva a:

* **Baixa resiliência**: sistemas quebram facilmente.
* **Custos elevados**: reprocessamento completo a cada falha.
* **Mau atendimento**: usuário final recebe erro em vez de resultado parcial ou alternativo.

---

## A Tese

> **Grafos aumentam a resiliência dos sistemas de IA, pois permitem múltiplos caminhos de fallback, tratamento modular de falhas e recuperação parcial da execução.**

---

## Fundamentação

### Falha em Chains

* Em um chain, cada nó depende rigidamente da saída anterior.
* A ausência de bifurcação impede a execução alternativa.
* Formalmente: se $(v_{k-1}, v_k) \in E$ e $v_k$ falha, não existe outro caminho → execução aborta.

### Fallback em Grafos

* Grafos permitem **arestas alternativas**.
* Se um nó falha, a execução pode seguir por outra aresta ou redirecionar para um nó de tratamento.

Exemplo:

$$
v_{input} \to v_A \to (v_B \; \lor \; v_B')
$$

Se $v_B$ falha, $v_B'$ é chamado como fallback.

### Recuperação Parcial

* Grafos permitem salvar **estados intermediários**.
* Em caso de falha, não é necessário reexecutar todo o fluxo → apenas o subgrafo afetado.

### Isolamento de Falhas

* Cada nó é uma unidade independente.
* Erros podem ser contidos em subgrafos específicos, evitando propagação global.

---

## Prova Teórica

**Exemplo Comparativo:**

* **Pipeline Linear:**
  4 nós em sequência: $v_1 \to v_2 \to v_3 \to v_4$.
  Se $v_2$ falha, $v_3$ e $v_4$ nunca são executados.
  Usuário final recebe erro.

* **Grafo com Fallback:**
  4 nós em sequência, mas $v_2$ tem alternativa $v_2'$.

  $$
  v_1 \to (v_2 \lor v_2') \to v_3 \to v_4
  $$

  Se $v_2$ falha, fluxo segue por $v_2'$.
  Usuário ainda recebe resultado, talvez com aviso de fallback.

**Formalização:**
Seja $G = (V,E)$.
Para cada nó $v_i$, definimos conjunto de fallback $F(v_i) \subseteq V$.

* Em caso de falha:

$$
Exec(v_i) = \begin{cases} 
success & \text{se } v_i \text{ executa corretamente} \\  
Exec(f), \quad f \in F(v_i) & \text{se } v_i \text{ falha}  
\end{cases}
$$

Logo, execução não se encerra em falhas, mas redireciona.

---

## Discussão

As implicações dessa propriedade são profundas:

1. **Robustez em Produção**

   * Usuário raramente vê erro "branco".
   * Sistema sempre tenta caminho alternativo.

2. **Eficiência Operacional**

   * Menos reprocessamento completo.
   * Subgrafos reutilizados com checkpoints.

3. **Experiência do Usuário**

   * Mesmo em falhas, sistema retorna respostas úteis (ex.: versão simplificada).
   * Transparência: logs de fallback podem ser expostos ao usuário como explicação.

4. **Alinhamento Regulatório**

   * Sistemas críticos (banco, saúde) precisam garantir continuidade de operação.
   * Grafos atendem a essa exigência com caminhos redundantes.

---

## Conclusão

A fragilidade dos pipelines lineares decorre de sua própria natureza sequencial.
Mostramos que **grafos permitem fallback, recuperação parcial e isolamento de falhas**, garantindo **resiliência estrutural**.

Assim, enquanto chains quebram facilmente, grafos se adaptam, mantêm continuidade e reduzem custos operacionais.

No próximo capítulo, exploraremos como essa flexibilidade também possibilita **Integração Multimodal e Híbrida** (Capítulo 8), algo praticamente inviável em pipelines lineares, mas natural em arquiteturas em grafo.

```{=latex}
\begin{figure}[ht]
\centering
% Caminho de fallback: v2 tem alternativa v2' em caso de falha
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (v1) {v$_1$};
  \node (v2) [right=of v1] {v$_2$};
  \node (v2p) [below=of v2] {v$_2'$};
  \node (v3) [right=1.8cm of v2] {v$_3$};
  \node (v4) [right=of v3] {v$_4$};
  \path[->]
    (v1) edge (v2)
    (v2) edge (v3)
    (v2p) edge (v3)
    (v1) edge (v2p)
    (v3) edge (v4);
\end{tikzpicture}
\caption{Fallback: alternativa $v_2'$ quando $v_2$ falha}
\label{fig:cap7-fallback}
\end{figure}
```

---