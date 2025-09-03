namespace GraphIABook.Graphs.Chapter18;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 18 — SKG graph performing algebraic analysis (adjacency/incidence) on a fixed DAG.
/// Parallel branches compute degrees, reachability and acyclicity; results are merged.
/// Mirrors docs/book/24-capitulo-18.md numeric example.
/// </summary>
public static class GraphChapter18
{
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };
        var executor = CreateExecutor();
        GraphValidationUtils.EnsureAcyclic(executor);
        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var build = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            int[,] A = new int[4, 4];
            // Edges: (1,2), (1,3), (2,4), (3,4)
            A[0, 1] = 1; A[0, 2] = 1; A[1, 3] = 1; A[2, 3] = 1;
            a["A"] = A;
            return Task.FromResult("built-A");
        }, "BuildAdjacency"), nodeId: "build");

        var degrees = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var A = (int[,])a["A"]!;
            var (outDeg, inDeg) = ComputeDegrees(A);
            a["out"] = outDeg;
            a["in"] = inDeg;
            return Task.FromResult("degrees");
        }, "Degrees"), nodeId: "degrees");

        var reach = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var A = (int[,])a["A"]!;
            var A2 = Multiply(A, A);
            bool reachable14 = A2[0, 3] > 0 || A[0, 3] > 0;
            a["A2"] = A2;
            a["r14"] = reachable14;
            return Task.FromResult("reachability");
        }, "Reachability"), nodeId: "reach");

        var acyclic = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var A = (int[,])a["A"]!;
            bool hasCycle = HasDiagonalOnAnyPower(A);
            a["acyclic"] = !hasCycle;
            return Task.FromResult("acyclicity");
        }, "Acyclicity"), nodeId: "acyclic");

        var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
        {
            var outD = (int[])a["out"]!;
            var inD = (int[])a["in"]!;
            bool r14 = a.TryGetValue("r14", out var rv) && rv is bool b && b;
            bool ac = a.TryGetValue("acyclic", out var av) && av is bool bb && bb;
            var summary = $"answer(graph): out=[{string.Join(',', outD)}]; in=[{string.Join(',', inD)}]; r(1->4)={r14}; acyclic={ac}";
            return Task.FromResult(summary);
        }, "Merge"), nodeId: "merge");

        var executor = new GraphExecutor("ch18_algebra", "algebraic analysis over DAG with parallel branches");
        executor.AddNode(start)
            .AddNode(build)
            .AddNode(degrees)
            .AddNode(reach)
            .AddNode(acyclic)
            .AddNode(merge);

        executor.SetStartNode("start");
        executor.Connect("start", "build");
        executor.Connect("build", "degrees");
        executor.Connect("build", "reach");
        executor.Connect("build", "acyclic");
        executor.Connect("degrees", "merge");
        executor.Connect("reach", "merge");
        executor.Connect("acyclic", "merge");

        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = 3
        });

        return executor;
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


