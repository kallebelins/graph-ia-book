# Guia de Topologias e Anti-\-padrões\index{topologia}\index{anti-padrão}\index{diamante}

---

## O Problema

Topologias afetam diretamente latência, resiliência e manutenção. Precisamos de um guia prático.

---

## Topologias Recomendadas

* Em estrela com agregadores: facilita paralelismo e auditoria.
* Em camadas: separa preocupações e controlos.
* Subgrafos especializados reutilizáveis: reduz duplicação.
* Árvores de redução (associativas): reduzem profundidade de fusão e caminho crítico[^1].
* Diamante com agregação determinística: paralelismo com política clara de merge[^2].

Em termos simples:

- Topologias em estrela e em camadas ajudam a organizar e paralelizar o trabalho.
- Diamante com regra de fusão clara evita decisões ambíguas e resultados inconsistentes.

---

## Anti-padrões

* Correntes longas (chains profundos): alta latência e fragilidade.
* Diamante sem agregação determinística: inconsistência em fusão.
* Nós-gargalo não escalados: saturação e falhas em cascata.
* Acoplamento circular inadvertido: ciclos que impedem ordenação topológica e execução segura.

```{=latex}
\begin{figure}[ht]
\centering
% Diamante com merge determinístico (ex.: max/votação)
\begin{tikzpicture}[>=Stealth, node distance=1.8cm]
  \node (s) {Start};
  \node (a) [right=of s] {A};
  \node (b) [below=of a] {B};
  \node (m) [right=2.0cm of a] {Merge(det)};
  \node (t) [right=of m] {End};
  \path[->]
    (s) edge (a)
    (s) edge (b)
    (a) edge (m)
    (b) edge (m)
    (m) edge (t);
\end{tikzpicture}
\caption{Diamante com agregação determinística (ex.: max, votação, média ponderada)}
\label{fig:cap24-diamante-merge-det}
\end{figure}
```

---

## Mitigações

- Introduza agregadores com regras determinísticas (max, votação, média ponderada).
- Escale horizontalmente nós com alta betweenness; particione por chave[^2].
- Valide aciclicidade a cada alteração (ordem topológica) e teste alcançabilidade.
- Padronize interfaces entre subgrafos para reuso e substituição sem quebrar o contrato.

---

## Exemplo Reprodutível

Comparar tempo de resposta entre (i) chain de 5 nós e (ii) 5 nós paralelos + agregador (parâmetros dos capítulos 6 e 10). Em geral, (ii) aproxima-se do máximo dos tempos paralelos mais o custo de merge, reduzindo a latência média e a cauda p99.

---

## Conclusão

Topologias adequadas maximizam paralelismo, minimizam risco e simplificam governança. Clareza de políticas de fusão e validações estruturais contínuas são essenciais.

---

## Exercícios – Parte VII {-}

1) Classifique as topologias (estrela, camadas, diamante, árvore de redução) quanto a latência, resiliência e manutenção. Justifique.

2) Dado um diamante com regra de merge determinística, avalie o impacto de introduzir um agregador intermediário (dois níveis) no caminho crítico.

3) Identifique dois anti‑padrões recorrentes na sua área e mostre como reescrever em topologia recomendada.

4) Explique como validar aciclicidade e alcançabilidade em pipelines reais após cada alteração de topologia.

5) Projete uma política de cancelamento para paralelismo OR que minimize custo adicional e cauda p99.

[^1]: R. P. Brent, “The Parallel Evaluation of General Arithmetic Expressions,” JACM, 1974.
[^2]: M. E. J. Newman, Networks: An Introduction, OUP, 2010.


