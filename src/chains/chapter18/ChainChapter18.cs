namespace GraphIABook.Chains.Chapter18;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 18 — SK chain demonstrating algebraic analysis over a fixed DAG (adjacency/incidence):
/// - Build adjacency matrix A for a 4-node diamond: (1->2, 1->3, 2->4, 3->4)
/// - Compute degrees, reachability (A^2, T[1,4]) and acyclicity checks (no diagonal in A^ℓ)
/// - Merge results into a single string output.
/// This mirrors the formal content in docs/book/24-capitulo-18.md.
/// </summary>
public static class ChainChapter18
{
    /// <summary>
    /// Runs the sequential (chain) analysis pipeline over a fixed small DAG.
    /// </summary>
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var build = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                int[,] A = new int[4, 4];
                // Edges: (1,2), (1,3), (2,4), (3,4)
                A[0, 1] = 1;
                A[0, 2] = 1;
                A[1, 3] = 1;
                A[2, 3] = 1;
                a["A"] = A;
                return Task.FromResult("built-A");
            },
            "BuildAdjacency");

        var degrees = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var A = (int[,])a["A"]!;
                var (outDeg, inDeg) = ComputeDegrees(A);
                a["out"] = outDeg;
                a["in"] = inDeg;
                return Task.FromResult("degrees");
            },
            "Degrees");

        var reach = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var A = (int[,])a["A"]!;
                var A2 = Multiply(A, A);
                bool reachable14 = A2[0, 3] > 0 || A[0, 3] > 0; // length 2 shows 2 paths
                a["A2"] = A2;
                a["r14"] = reachable14;
                return Task.FromResult("reachability");
            },
            "Reachability");

        var acyclic = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var A = (int[,])a["A"]!;
                bool hasCycle = HasDiagonalOnAnyPower(A);
                a["acyclic"] = !hasCycle;
                return Task.FromResult("acyclicity");
            },
            "Acyclicity");

        var merge = KernelFunctionFactory.CreateFromMethod(
            (KernelArguments a) =>
            {
                var outD = (int[])a["out"]!;
                var inD = (int[])a["in"]!;
                bool r14 = a.TryGetValue("r14", out var rv) && rv is bool b && b;
                bool ac = a.TryGetValue("acyclic", out var av) && av is bool bb && bb;
                var summary = $"answer(chain): out=[{string.Join(',', outD)}]; in=[{string.Join(',', inD)}]; r(1->4)={r14}; acyclic={ac}";
                return Task.FromResult(summary);
            },
            "Merge");

        _ = await kernel.InvokeAsync(build, args);
        _ = await kernel.InvokeAsync(degrees, args);
        _ = await kernel.InvokeAsync(reach, args);
        var result = await kernel.InvokeAsync(acyclic, args);
        var merged = await kernel.InvokeAsync(merge, args);
        _ = result; // keep sequential intent explicit
        return merged.GetValue<string>() ?? string.Empty;
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
}


