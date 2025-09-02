# Governança e Segurança em Grafos

---

## O Problema

À medida que a Inteligência Artificial avança para ambientes críticos — saúde, jurídico, finanças, governo — surgem exigências cada vez mais fortes de **governança e segurança**.

Os principais desafios são:

* **Falta de controle granular**: em pipelines lineares (*chains*), não é possível aplicar regras específicas a cada etapa.
* **Baixa auditabilidade**: decisões não são registradas de forma explicável.
* **Risco de alucinação ou desvio ético**: modelos podem produzir saídas imprevistas sem mecanismos de contenção.
* **Conformidade regulatória**: legislações como o **AI Act (UE)** (regulamentação europeia para IA) e princípios de **IA Responsável** exigem transparência, rastreabilidade e accountability (responsabilização).

Sem uma arquitetura robusta, os sistemas de IA se tornam **caixas-pretas**, incompatíveis com padrões de confiabilidade e compliance.

---

## A Tese

> **Grafos oferecem governança e segurança superiores, pois permitem controle de acesso, aplicação de políticas, rastreabilidade e contenção de riscos em nível de nó e de caminho.**

---

## Fundamentação

### Governança em Chains

* Um pipeline linear é opaco:

  * não há pontos de inspeção internos,
  * não há política diferenciada por etapa,
  * monitoramento se limita ao input e output final.
* Isso viola requisitos básicos de auditoria e segurança.

### Governança em Grafos

* Cada **nó** é uma unidade auditável.
* É possível aplicar **políticas locais**:

  * controle de acesso,
  * limitação de tokens,
  * checagem de bias,
  * guardrails de conteúdo.
* É possível aplicar **políticas globais**:

  * restrição de caminhos,
  * logging completo da execução,
  * rotas seguras para dados sensíveis.

### Segurança Estrutural

* **Isolamento de nós críticos:** subgrafos com dados sensíveis podem ser protegidos.
* **Fallback seguro:** se um nó produz saída insegura, o fluxo pode redirecionar para nó de validação.
* **Auditoria formal:** cada execução gera um **log estruturado** com inputs, outputs, tempo, custo e decisão.

---

## Prova Teórica

**Proposição:** Grafos permitem segurança e governança que não podem ser obtidas em pipelines lineares.

* **Em chains:**

  * Apenas dois pontos possíveis de monitoramento: início e fim.
  * Se $f_3$ gera saída indevida, isso só será percebido no final.
  * Não é possível bloquear/filtrar resultados intermediários sem quebrar a cadeia.

* **Em grafos:**

  * Cada nó $v_i$ pode ter políticas associadas $P(v_i)$.
  * Formalmente:

    $$
    Exec(v_i) = 
    \begin{cases} 
    block & \text{se violar } P(v_i) \\ 
    continue & \text{se estiver conforme}
    \end{cases}
    $$
  * Políticas podem ser compostas ao longo do caminho, permitindo **checagem incremental** de conformidade.

**Exemplo prático:**

* Em um fluxo jurídico, o nó `Summarizer` (resumidor automático de texto) pode ter política de *não incluir dados pessoais*.
* Em caso de violação, o fluxo desvia para `AnonymizerNode` (nó que remove/mascara dados sensíveis) antes de continuar.
* Isso é impossível em pipelines lineares sem duplicar lógica e quebrar a sequência.

---

## Discussão

Essa governança distribuída tem consequências cruciais:

1. **Compliance Regulatório**

   * Atende exigências de explicabilidade e accountability.
   * Cada decisão pode ser registrada e justificada.
   * Checklist prático (AI Act / NIST AI RMF):
     - Identificar propósito e contexto de uso por nó/caminho (mapa de uso).
     - Classificar riscos por nó (impacto/probabilidade) e definir controles.
     - Habilitar logging de trilhas e decisões (auditoria reproduzível).
     - Implementar validações (guardrails) e anonimização onde aplicável.
     - Monitorar métricas de desempenho/segurança (p95/p99, taxa de falhas).
     - Planejar resposta a incidentes e rotas de fallback seguras.
     - Revisar periodicamente políticas e riscos (ciclo de melhoria contínua).

2. **Segurança Operacional**

   * Dados sensíveis podem ser isolados em subgrafos.
   * Guardrails evitam que outputs inseguros cheguem ao usuário.

3. **Confiança do Usuário**

   * Transparência na explicação de como a resposta foi formada.
   * Possibilidade de auditar logs em auditorias externas.

4. **Gestão de Risco**

   * Identificação de pontos vulneráveis em fluxos.
   * Mitigação preventiva por políticas aplicadas a nós específicos.

---

## Conclusão

Enquanto pipelines lineares permanecem opacos e inseguros, grafos oferecem **controle granular, rastreabilidade e políticas de segurança integradas à estrutura de execução**.

Assim, a governança em grafos não é um acessório, mas uma **característica estrutural**:

* Cada nó pode ser auditado,
* Cada caminho pode ser rastreado,
* Cada decisão pode ser explicada.

No próximo capítulo, avançaremos para **Comparação Formal: Grafos vs Chains (Capítulo 10)**, sintetizando as provas já apresentadas e estabelecendo um quadro comparativo robusto entre os dois modelos.

```{=latex}
\begin{figure}[ht]
\centering
% Guard policy e anonymizer: validação de conteúdo e desvio seguro
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (in) {Input};
  \node (p)  [right=of in] {PolicyGuard}; % PolicyGuard (aplicador de políticas)
  \node (ok) [above right=1.0cm and 2.0cm of p] {Processo OK};
  \node (an) [below right=1.0cm and 2.0cm of p] {Anonymizer};
  \node (out)[right=2.0cm of ok] {Output};
  \path[->]
    (in) edge (p)
    (p) edge (ok)
    (p) edge (an)
    (an) edge (ok)
    (ok) edge (out);
\end{tikzpicture}
\caption{Governança em grafo: política de guarda e anonimização com desvio seguro}
\label{fig:cap9-guard-anon}
\end{figure}
```

---

## Seção Prática: Checklist de Governança e Compliance {-}

\index{governança}\index{compliance}\index{explainability}\index{logging}\index{controle de acesso}\index{guardrails}\index{anonimização}

Use esta lista de verificação para mapear práticas de governança ao grafo de execução:

- Políticas por nó: defina controles por nó (acesso, limites, validações). Ver também Cap. 24 (topologias e anti‑padrões) e Cap. 10 (diamante/bottleneck e trade‑offs).
- Logging por trilhas: habilite trilhas de auditoria completas por caminho de execução (inputs/outputs/latência/custos). Ver Cap. 5 (trilha de auditoria) e Fig. \ref{fig:cap9-guard-anon}.
- Explainability: registre decisões e justificativas por nó (racional, modelo, versão, métricas). Relacione com métricas de Cap. 23 (betweenness/paths críticos).
- Guardrails e anonimização: aplique políticas de conteúdo e anonimização onde necessário. Ver Cap. 9 (policy guard) e Cap. 12 (guard policy e anonimização).
- Rotas seguras e fallback: defina caminhos alternativos para violações de política com contenção de risco (fallback validado). Ver Cap. 7 (caminho de fallback).
- Monitoramento: acompanhe p95/p99, taxa de falhas, custos por nó/caminho e alerte desvios.
- Revisão periódica: ciclo de melhoria contínua (riscos, políticas, thresholds) com registro versionado.

Ver também: Glossário Técnico (termos: explainability, guardrails, logging, anonimização) e Tabela de Notação.

---