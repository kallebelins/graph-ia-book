namespace GraphIABook.Chains.Chapter21;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 21 — SK chain computing Markov metrics (fundamental matrix, expected time, absorption probs)
/// for a small absorbing Markov chain with states {S, A, B, F}, where F is absorbing.
/// Sequentially builds Q and R, then computes N=(I−Q)^{-1}, t=N·1 and B=N·R.
/// Mirrors theoretical content in docs/book/28-capitulo-21.md.
/// </summary>
public static class ChainChapter21
{
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var build = KernelFunctionFactory.CreateFromMethod(
 			(KernelArguments a) =>
 			{
 				// Transient states: [S,A,B] (indices 0..2); Absorbing: [F]
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
 				a["Q"] = Q;
 				a["R"] = R;
 				return Task.FromResult("built-markov");
 			},
 			"BuildMarkov");

 		var computeN = KernelFunctionFactory.CreateFromMethod(
 			(KernelArguments a) =>
 			{
 				var Q = (double[,])a["Q"]!;
 				var I = Identity(3);
 				var IminusQ = Subtract(I, Q);
 				var N = Inverse3x3(IminusQ);
 				a["N"] = N;
 				return Task.FromResult("N");
 			},
 			"ComputeN");

 		var computeT = KernelFunctionFactory.CreateFromMethod(
 			(KernelArguments a) =>
 			{
 				var N = (double[,])a["N"]!;
 				double[] ones = new[] { 1.0, 1.0, 1.0 };
 				var t = Multiply(N, ones);
 				a["t"] = t;
 				return Task.FromResult("t");
 			},
 			"ComputeT");

 		var computeB = KernelFunctionFactory.CreateFromMethod(
 			(KernelArguments a) =>
 			{
 				var N = (double[,])a["N"]!;
 				var R = (double[,])a["R"]!; // 3x1
 				var B = Multiply(N, R); // 3x1
 				a["B"] = B;
 				return Task.FromResult("B");
 			},
 			"ComputeB");

 		var merge = KernelFunctionFactory.CreateFromMethod(
 			(KernelArguments a) =>
 			{
 				var t = (double[])a["t"]!;
 				var B = (double[,])a["B"]!; // 3x1
 				string tStr = string.Join(',', t.Select(v => v.ToString("0.###")));
 				string bStr = string.Join(',', new[] { B[0,0], B[1,0], B[2,0] }.Select(v => v.ToString("0.###")));
 				var summary = $"answer(chain21): t=[{tStr}]; B=[{bStr}]";
 				return Task.FromResult(summary);
 			},
 			"Merge");

 		_ = await kernel.InvokeAsync(build, args);
 		_ = await kernel.InvokeAsync(computeN, args);
 		_ = await kernel.InvokeAsync(computeT, args);
 		_ = await kernel.InvokeAsync(computeB, args);
 		var merged = await kernel.InvokeAsync(merge, args);
 		return merged.GetValue<string>() ?? string.Empty;
 	}

 	private static double[,] Identity(int n)
 	{
 		var I = new double[n, n];
 		for (int i = 0; i < n; i++) I[i, i] = 1.0;
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



