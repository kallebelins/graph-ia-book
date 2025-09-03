namespace GraphIABook.Graphs.Chapter21;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 21 — SKG graph computing Markov metrics with parallel branches:
/// - Build Q and R
/// - In parallel: compute N=(I−Q)^{-1}, t=N·1, B=N·R
/// - Merge deterministic summary string
/// Aligns with docs/book/28-capitulo-21.md.
/// </summary>
public static class GraphChapter21
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };
		var executor = CreateExecutor();
		GraphValidationUtils.EnsureAcyclic(executor);
		var result = await executor.ExecuteAsync(kernel, args).ConfigureAwait(false);
		return result.GetValue<string>() ?? string.Empty;
	}

	public static GraphExecutor CreateExecutor()
	{
		var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

		var build = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			// Transient [S,A,B]; Absorbing [F]
			double[,] Q = new double[3, 3]
			{
				{0.0, 0.6, 0.4},
				{0.0, 0.0, 0.1},
				{0.0, 0.2, 0.0}
			};
			double[,] R = new double[3, 1]
			{
				{0.0},
				{0.9},
				{0.8}
			};
			a["Q"] = Q; a["R"] = R;
			return Task.FromResult("built-markov");
		}, "BuildMarkov"), nodeId: "build");

		var computeN = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var Q = (double[,])a["Q"]!;
			var I = Identity(3);
			var N = Inverse3x3(Subtract(I, Q));
			a["N"] = N;
			return Task.FromResult("N");
		}, "ComputeN"), nodeId: "N");

		var computeT = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var N = (double[,])a["N"]!;
			double[] ones = new[] { 1.0, 1.0, 1.0 };
			a["t"] = Multiply(N, ones);
			return Task.FromResult("t");
		}, "ComputeT"), nodeId: "t");

		var computeB = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var N = (double[,])a["N"]!;
			var R = (double[,])a["R"]!;
			a["B"] = Multiply(N, R);
			return Task.FromResult("B");
		}, "ComputeB"), nodeId: "B");

		var merge = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod((KernelArguments a) =>
		{
			var t = (double[])a["t"]!;
			var B = (double[,])a["B"]!;
			string tStr = string.Join(',', t.Select(v => v.ToString("0.###")));
			string bStr = string.Join(',', new[] { B[0,0], B[1,0], B[2,0] }.Select(v => v.ToString("0.###")));
			return Task.FromResult($"answer(graph21): t=[{tStr}]; B=[{bStr}]");
		}, "Merge"), nodeId: "merge");

		var exec = new GraphExecutor("ch21_markov", "Markov metrics with parallel branches");
		exec.AddNode(start).AddNode(build).AddNode(computeN).AddNode(computeT).AddNode(computeB).AddNode(merge);
		exec.SetStartNode("start");
		exec.Connect("start", "build");
		exec.Connect("build", "N");
		exec.Connect("N", "t");
		exec.Connect("N", "B");
		exec.Connect("t", "merge");
		exec.Connect("B", "merge");
		exec.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 2 });
		return exec;
	}

	private static double[,] Identity(int n)
	{
		var I = new double[n, n];
		for (int k = 0; k < n; k++) I[k, k] = 1.0;
		return I;
	}

	private static double[,] Subtract(double[,] A, double[,] B)
	{
		int n = A.GetLength(0), m = A.GetLength(1);
		var R = new double[n, m];
		for (int i = 0; i < n; i++) for (int j = 0; j < m; j++) R[i, j] = A[i, j] - B[i, j];
		return R;
	}

	private static double[,] Inverse3x3(double[,] M)
	{
		double a = M[0,0], b = M[0,1], c = M[0,2];
		double d = M[1,0], e = M[1,1], f = M[1,2];
		double g = M[2,0], h = M[2,1], i = M[2,2];

		double A =  (e*i - f*h);
		double B = -(d*i - f*g);
		double C =  (d*h - e*g);
		double D = -(b*i - c*h);
		double E =  (a*i - c*g);
		double F = -(a*h - b*g);
		double G =  (b*f - c*e);
		double H = -(a*f - c*d);
		double I =  (a*e - b*d);

		double det = a*A + b*B + c*C;
		if (Math.Abs(det) < 1e-12) throw new InvalidOperationException("Matrix not invertible");
		double invDet = 1.0 / det;
		var inv = new double[3,3]
		{
			{A*invDet, D*invDet, G*invDet},
			{B*invDet, E*invDet, H*invDet},
			{C*invDet, F*invDet, I*invDet}
		};
		return inv;
	}

	private static double[] Multiply(double[,] M, double[] v)
	{
		int n = M.GetLength(0), m = M.GetLength(1);
		if (m != v.Length) throw new ArgumentException("Dimension mismatch");
		var r = new double[n];
		for (int i = 0; i < n; i++)
		{
			double s = 0.0;
			for (int j = 0; j < m; j++) s += M[i, j] * v[j];
			r[i] = s;
		}
		return r;
	}

	private static double[,] Multiply(double[,] A, double[,] B)
	{
		int n = A.GetLength(0), m = A.GetLength(1);
		int p = B.GetLength(1);
		if (m != B.GetLength(0)) throw new ArgumentException("Dimension mismatch");
		var R = new double[n, p];
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < p; j++)
			{
				double s = 0.0;
				for (int k = 0; k < m; k++) s += A[i, k] * B[k, j];
				R[i, j] = s;
			}
		}
		return R;
	}
}



