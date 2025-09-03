namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter21;
using GraphIABook.Graphs.Chapter21;

/// <summary>
/// Capítulo 21 — A definir. Consulte docs/book/28-capitulo-21.md.
/// </summary>
public sealed class Chapter21 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_MarkovAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_MarkovAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteMarkovTheory();
	}

	/// <summary>
	/// Chain: computa N, t e B para cadeia de Markov absorvente pequena.
	/// </summary>
	public async Task RunChain_MarkovAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap21/chain/markov", async () =>
		{
			var output = await ChainChapter21.RunAsync("Cap21 Markov (chain)");
			return output;
		});
	}

	/// <summary>
	/// Graph: executa ramos paralelos para t e B, com N compartilhado.
	/// </summary>
	public async Task RunGraph_MarkovAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap21/graph/markov", async () =>
		{
			var output = await GraphChapter21.RunAsync("Cap21 Markov (graph)");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain sequencial.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap21/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter21.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo com ramos paralelos.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 40);
		await BenchmarkUtils.MeasureManyAsync("cap21/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter21.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Executa benchmark A/B (latência) entre chain e graph.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = TestFixtures.GetFixedTextInputs(count: 30);
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap21/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter21.RunAsync(s); },
			async s => { _ = await GraphChapter21.RunAsync(s); });
	}

	/// <summary>
	/// Escreve resumo teórico: N, t, B para o exemplo de Markov absorvente.
	/// </summary>
	public static void WriteMarkovTheory()
	{
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
		var I = Identity(3);
		var N = Inverse3x3(Subtract(I, Q));
		var t = Multiply(N, new[] { 1.0, 1.0, 1.0 });
		var B = Multiply(N, R);

		BenchmarkUtils.WriteTheory("cap21/theory/markov", new Dictionary<string, object>
		{
			["Q"] = ToList(Q),
			["R"] = ToList(B), // store B as list under R key? Keep both:
			["R_matrix"] = ToList(R),
			["N"] = ToList(N),
			["t"] = t,
			["B"] = ToList(B),
			["notes"] = "N=(I−Q)^{-1}; t=N*1; B=N*R para estados transitórios S,A,B com F absorvente."
		});
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

	private static double[][] ToList(double[,] M)
	{
		int n = M.GetLength(0), m = M.GetLength(1);
		var list = new double[n][];
		for (int i = 0; i < n; i++)
		{
			list[i] = new double[m];
			for (int j = 0; j < m; j++) list[i][j] = M[i, j];
		}
		return list;
	}
}


