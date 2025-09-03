namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter18;
using GraphIABook.Graphs.Chapter18;

/// <summary>
/// Capítulo 18 — Álgebra de Grafos: Matrizes de Adjacência e Incidência.
/// Consulte docs/book/24-capitulo-18.md para a fundamentação teórica.
/// Compara chain (análise sequencial) vs grafo (análises paralelas) e escreve
/// um resumo teórico com alcançabilidade e aciclicidade via potências de A.
/// </summary>
public sealed class Chapter18 : IChapter
{
    public async Task RunChainAsync()
    {
        await RunChain_AlgebraAsync();
        await RunChain_LatencySummaryAsync();
    }

    public async Task RunGraphAsync()
    {
        await RunGraph_AlgebraAsync();
        await RunGraph_LatencySummaryAsync();
    }

    public async Task RunBenchmarkAsync()
    {
        await RunBenchmark_LatencyABAsync();
        WriteAlgebraTheory();
    }

    /// <summary>
    /// Chain: executa análise algébrica sequencial (A, graus, alcançabilidade, aciclicidade).
    /// </summary>
    public async Task RunChain_AlgebraAsync()
    {
        await BenchmarkUtils.MeasureAsync("cap18/chain/algebra", async () =>
        {
            var output = await ChainChapter18.RunAsync("Cap18 algebra (chain)");
            return output;
        });
    }

    /// <summary>
    /// Graph: executa análises em ramos paralelos e faz merge determinístico.
    /// </summary>
    public async Task RunGraph_AlgebraAsync()
    {
        await BenchmarkUtils.MeasureAsync("cap18/graph/algebra", async () =>
        {
            var output = await GraphChapter18.RunAsync("Cap18 algebra (graph)");
            return output;
        });
    }

    /// <summary>
    /// Sumariza latência (média/p95/p99) do chain sequencial.
    /// </summary>
    public async Task RunChain_LatencySummaryAsync()
    {
        var inputs = TestFixtures.GetFixedTextInputs(count: 40);
        await BenchmarkUtils.MeasureManyAsync("cap18/chain/latency", iterations: inputs.Count, action: async () =>
        {
            var idx = Math.Abs(Environment.TickCount) % inputs.Count;
            _ = await ChainChapter18.RunAsync(inputs[idx]);
        });
    }

    /// <summary>
    /// Sumariza latência (média/p95/p99) do grafo com ramos paralelos.
    /// </summary>
    public async Task RunGraph_LatencySummaryAsync()
    {
        var inputs = TestFixtures.GetFixedTextInputs(count: 40);
        await BenchmarkUtils.MeasureManyAsync("cap18/graph/latency", iterations: inputs.Count, action: async () =>
        {
            var idx = Math.Abs(Environment.TickCount) % inputs.Count;
            _ = await GraphChapter18.RunAsync(inputs[idx]);
        });
    }

    /// <summary>
    /// Executa benchmark A/B (latência) entre chain e graph utilizando os mesmos inputs.
    /// </summary>
    public async Task RunBenchmark_LatencyABAsync()
    {
        var inputs = TestFixtures.GetFixedTextInputs(count: 30);
        await AbBenchmarkHarness.RunLatencyABAsync(
            "cap18/benchmark/latency-ab",
            inputs,
            async s => { _ = await ChainChapter18.RunAsync(s); },
            async s => { _ = await GraphChapter18.RunAsync(s); });
    }

    /// <summary>
    /// Escreve resumo teórico: graus, alcançabilidade e aciclicidade via potências de A.
    /// </summary>
    public static void WriteAlgebraTheory()
    {
        int[,] A = new int[4, 4];
        A[0, 1] = 1; A[0, 2] = 1; A[1, 3] = 1; A[2, 3] = 1;
        var (outDeg, inDeg) = ComputeDegrees(A);
        var A2 = Multiply(A, A);
        bool reachable14 = A2[0, 3] > 0 || A[0, 3] > 0;
        bool hasCycle = HasDiagonalOnAnyPower(A);

        BenchmarkUtils.WriteTheory("cap18/theory/adjacency-incidence", new Dictionary<string, object>
        {
            ["A"] = AToList(A),
            ["A2"] = AToList(A2),
            ["out_degrees"] = outDeg,
            ["in_degrees"] = inDeg,
            ["reachable_1_to_4"] = reachable14,
            ["acyclic"] = !hasCycle,
            ["notes"] = "A^2[1,4]=2 (duas trilhas). Acyclic true pois nenhuma diagonal aparece em potências < n."
        });
    }

    private static (int[] outDeg, int[] inDeg) ComputeDegrees(int[,] A)
    {
        int n = A.GetLength(0);
        var outD = new int[n];
        var inD = new int[n];
        for (int i = 0; i < n; i++)
        {
            int o = 0, ii = 0;
            for (int j = 0; j < n; j++)
            {
                o += A[i, j];
                ii += A[j, i];
            }
            outD[i] = o;
            inD[i] = ii;
        }
        return (outD, inD);
    }

    private static int[,] Multiply(int[,] X, int[,] Y)
    {
        int n = X.GetLength(0);
        var R = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int s = 0;
                for (int k = 0; k < n; k++) s += X[i, k] * Y[k, j];
                R[i, j] = s;
            }
        }
        return R;
    }

    private static bool HasDiagonalOnAnyPower(int[,] A)
    {
        int n = A.GetLength(0);
        var P = (int[,])A.Clone();
        for (int p = 1; p < n; p++)
        {
            for (int i = 0; i < n; i++) if (P[i, i] != 0) return true;
            P = Multiply(P, A);
        }
        return false;
    }

    private static int[][] AToList(int[,] A)
    {
        int n = A.GetLength(0);
        var list = new int[n][];
        for (int i = 0; i < n; i++)
        {
            list[i] = new int[n];
            for (int j = 0; j < n; j++) list[i][j] = A[i, j];
        }
        return list;
    }
}


