namespace GraphIABook.Chains.Chapter23;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 23 — SK chain computing structural metrics over a small DAG:
/// - Diameter (on the underlying undirected graph)
/// - Betweenness centrality for the aggregator node
/// - Cyclomatic complexity M = E - V + P (weakly connected components)
/// Graph topology mirrors docs/book/31-capitulo-23.md: five sources → aggregator → target.
/// </summary>
public static class ChainChapter23
{
	/// <summary>
	/// Runs the sequential (chain) computation of structural metrics.
	/// </summary>
	public static async Task<string> RunAsync(string input)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["input"] = input };

		var build = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				// Nodes: s1..s5 (0..4), m (5), t (6)
				int nodeCount = 7;
				var dagAdj = CreateEmptyAdjacency(nodeCount);
				for (int s = 0; s < 5; s++) dagAdj[s].Add(5);
				dagAdj[5].Add(6);
				a["N"] = nodeCount;
				a["DAG"] = dagAdj;
				a["AggregatorIndex"] = 5;
				return Task.FromResult("built-dag");
			},
			"BuildDag");

		var diameter = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				int n = (int)a["N"]!;
				var dag = (List<int>[])a["DAG"]!;
				var undirected = ToUndirected(n, dag);
				int d = ComputeDiameter(n, undirected);
				a["diameter"] = d;
				return Task.FromResult("diameter");
			},
			"Diameter");

		var bet = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				int n = (int)a["N"]!;
				var dag = (List<int>[])a["DAG"]!;
				var undirected = ToUndirected(n, dag);
				int m = (int)a["AggregatorIndex"]!;
				double c = ComputeBetweennessForNode(n, undirected, m);
				a["betweenness_m"] = c;
				return Task.FromResult("betweenness");
			},
			"Betweenness");

		var cyclo = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				int n = (int)a["N"]!;
				var dag = (List<int>[])a["DAG"]!;
				int e = CountEdges(dag);
				int p = CountWeaklyConnectedComponents(n, dag);
				int M = e - n + p;
				a["cyclomatic"] = M;
				return Task.FromResult("cyclomatic");
			},
			"Cyclomatic");

		var merge = KernelFunctionFactory.CreateFromMethod(
			(KernelArguments a) =>
			{
				int d = a.TryGetValue("diameter", out var dv) && dv is int di ? di : -1;
				double b = a.TryGetValue("betweenness_m", out var bv) && bv is double bd ? bd : double.NaN;
				int M = a.TryGetValue("cyclomatic", out var cv) && cv is int ci ? ci : int.MinValue;
				var summary = $"answer(chain23): diameter={d}; betweenness(m)={b:0.###}; M={M}";
				return Task.FromResult(summary);
			},
			"Merge");

		_ = await kernel.InvokeAsync(build, args);
		_ = await kernel.InvokeAsync(diameter, args);
		_ = await kernel.InvokeAsync(bet, args);
		var merged = await kernel.InvokeAsync(cyclo, args);
		var output = await kernel.InvokeAsync(merge, args);
		_ = merged;
		return output.GetValue<string>() ?? string.Empty;
	}

	private static List<int>[] CreateEmptyAdjacency(int n)
	{
		var g = new List<int>[n];
		for (int i = 0; i < n; i++) g[i] = new List<int>();
		return g;
	}

	private static List<int>[] ToUndirected(int n, List<int>[] dag)
	{
		var u = CreateEmptyAdjacency(n);
		for (int i = 0; i < n; i++)
		{
			foreach (var j in dag[i])
			{
				u[i].Add(j);
				u[j].Add(i);
			}
		}
		return u;
	}

	private static int CountEdges(List<int>[] dag)
	{
		int e = 0;
		for (int i = 0; i < dag.Length; i++) e += dag[i].Count;
		return e;
	}

	private static int CountWeaklyConnectedComponents(int n, List<int>[] dag)
	{
		var u = ToUndirected(n, dag);
		var seen = new bool[n];
		int c = 0;
		for (int s = 0; s < n; s++)
		{
			if (seen[s]) continue;
			c++;
			var q = new Queue<int>();
			q.Enqueue(s); seen[s] = true;
			while (q.Count > 0)
			{
				var v = q.Dequeue();
				foreach (var w in u[v]) if (!seen[w]) { seen[w] = true; q.Enqueue(w); }
			}
		}
		return c;
	}

	private static int ComputeDiameter(int n, List<int>[] undirected)
	{
		int best = 0;
		for (int s = 0; s < n; s++)
		{
			var dist = BfsDistances(n, undirected, s);
			for (int v = 0; v < n; v++) if (dist[v] >= 0 && dist[v] > best) best = dist[v];
		}
		return best;
	}

	private static int[] BfsDistances(int n, List<int>[] g, int s)
	{
		var dist = new int[n];
		for (int i = 0; i < n; i++) dist[i] = -1;
		var q = new Queue<int>();
		dist[s] = 0; q.Enqueue(s);
		while (q.Count > 0)
		{
			var v = q.Dequeue();
			foreach (var w in g[v]) if (dist[w] == -1) { dist[w] = dist[v] + 1; q.Enqueue(w); }
		}
		return dist;
	}

	private static double ComputeBetweennessForNode(int n, List<int>[] undirected, int node)
	{
		// Brandes (single-source accumulation) specialized to return one node's score.
		double score = 0.0;
		for (int s = 0; s < n; s++)
		{
			var stack = new Stack<int>();
			var predecessors = new List<int>[n];
			for (int i = 0; i < n; i++) predecessors[i] = new List<int>();
			var sigma = new double[n];
			var dist = new int[n];
			for (int i = 0; i < n; i++) { sigma[i] = 0.0; dist[i] = -1; }
			sigma[s] = 1.0; dist[s] = 0;
			var q = new Queue<int>(); q.Enqueue(s);
			while (q.Count > 0)
			{
				var v = q.Dequeue();
				stack.Push(v);
				foreach (var w in undirected[v])
				{
					if (dist[w] < 0)
					{
						dist[w] = dist[v] + 1;
						q.Enqueue(w);
					}
					if (dist[w] == dist[v] + 1)
					{
						sigma[w] += sigma[v];
						predecessors[w].Add(v);
					}
				}
			}

			var delta = new double[n];
			while (stack.Count > 0)
			{
				var w = stack.Pop();
				foreach (var v in predecessors[w])
				{
					delta[v] += (sigma[v] / sigma[w]) * (1.0 + delta[w]);
				}
				if (w != s && w == node) score += delta[w];
			}
		}
		// For undirected graphs, standard betweenness divides by 2; Brandes accumulation above already accounts per-source.
		return score / 2.0;
	}
}


