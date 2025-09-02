## Runner (book.csproj)

Como usar:

- PowerShell:
  - Executar capítulo 1 em benchmark: `./Scripts/run.ps1 -chapter 1 -mode b`
  - Executar capítulo 3 em graph: `./Scripts/run.ps1 -chapter 3 -mode g`
  - Executar capítulo 0e em chain: `./Scripts/run.ps1 -chapter 0 -mode c`

- Bash:
  - Executar capítulo 1 em benchmark: `bash Scripts/run.sh 1 b`
  - Executar capítulo 3 em graph: `bash Scripts/run.sh 3 g`
  - Executar capítulo 0e em chain: `bash Scripts/run.sh 0 c`

Resultados:

- Resumos (JSON/Markdown) são escritos em `src/Benchmark/results/*-summary.{json,md}`.

Estrutura:

- `Program.cs`: seleciona capítulo e modo (c/g/b) e despacha para `ChapterX`.
- `Chapters/ChapterX.cs`: implementa `RunChainAsync`, `RunGraphAsync`, `RunBenchmarkAsync` e métodos específicos de métricas.
- `Benchmark/_common/BenchmarkUtils.cs`: utilitários para medir média, p95 e p99 e exportar resultados.
- `Scripts/run.ps1|run.sh`: wrappers para execução reprodutível.


