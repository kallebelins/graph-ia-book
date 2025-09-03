namespace GraphIABook.Chapters;

using GraphIABook.Benchmark._common;
using GraphIABook.Chains.Chapter19;
using GraphIABook.Graphs.Chapter19;

/// <summary>
/// Capítulo 19 — Grafos, Autômatos e Linguagens Formais.
/// Consulte docs/book/26-capitulo-19.md.
/// Compara chain (simulação NFA sequencial) vs grafo (aceitação + tamanho do DFA em paralelo).
/// </summary>
public sealed class Chapter19 : IChapter
{
	public async Task RunChainAsync()
	{
		await RunChain_AutomataAsync();
		await RunChain_LatencySummaryAsync();
	}

	public async Task RunGraphAsync()
	{
		await RunGraph_AutomataAsync();
		await RunGraph_LatencySummaryAsync();
	}

	public async Task RunBenchmarkAsync()
	{
		await RunBenchmark_LatencyABAsync();
		WriteAutomataTheory();
	}

	/// <summary>
	/// Chain: aceitação de palavra e tamanho do DFA (sequencial).
	/// </summary>
	public async Task RunChain_AutomataAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap19/chain/automata", async () =>
		{
			var output = await ChainChapter19.RunAsync("ac");
			return output;
		});
	}

	/// <summary>
	/// Graph: aceitação e tamanho do DFA em ramos paralelos (merge determinístico).
	/// </summary>
	public async Task RunGraph_AutomataAsync()
	{
		await BenchmarkUtils.MeasureAsync("cap19/graph/automata", async () =>
		{
			var output = await GraphChapter19.RunAsync("ac");
			return output;
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do chain para entradas em Σ*.
	/// </summary>
	public async Task RunChain_LatencySummaryAsync()
	{
		var inputs = GetAutomataInputs();
		await BenchmarkUtils.MeasureManyAsync("cap19/chain/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await ChainChapter19.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Sumariza latência (média/p95/p99) do grafo para as mesmas entradas.
	/// </summary>
	public async Task RunGraph_LatencySummaryAsync()
	{
		var inputs = GetAutomataInputs();
		await BenchmarkUtils.MeasureManyAsync("cap19/graph/latency", iterations: inputs.Count, action: async () =>
		{
			var idx = Math.Abs(Environment.TickCount) % inputs.Count;
			_ = await GraphChapter19.RunAsync(inputs[idx]);
		});
	}

	/// <summary>
	/// Benchmark A/B de latência entre chain e graph.
	/// </summary>
	public async Task RunBenchmark_LatencyABAsync()
	{
		var inputs = GetAutomataInputs();
		await AbBenchmarkHarness.RunLatencyABAsync(
			"cap19/benchmark/latency-ab",
			inputs,
			async s => { _ = await ChainChapter19.RunAsync(s); },
			async s => { _ = await GraphChapter19.RunAsync(s); });
	}

	/// <summary>
	/// Escreve resumo teórico (regularidade em DAGs; aceitação de exemplos; tamanho do DFA).
	/// </summary>
	public static void WriteAutomataTheory()
	{
		var sigma = new[] { 'a', 'b', 'c' };
		var g = new Dictionary<int, List<(char label, int to)>>
		{
			[0] = new List<(char, int)> { ('a', 1), ('b', 2) },
			[1] = new List<(char, int)> { ('c', 3) },
			[2] = new List<(char, int)> { ('c', 3) },
			[3] = new List<(char, int)>()
		};

		int dfaSize = DeterminizedSize(g, sigma, 0);
		bool accepts_ac = Accepts(g, 0, new HashSet<int> { 3 }, "ac");
		bool accepts_bc = Accepts(g, 0, new HashSet<int> { 3 }, "bc");
		bool accepts_a = Accepts(g, 0, new HashSet<int> { 3 }, "a");

		BenchmarkUtils.WriteTheory("cap19/theory/regularity-nfa-dfa", new Dictionary<string, object>
		{
			["alphabet"] = new string(sigma),
			["dfa_reachable_states"] = dfaSize,
			["accepts_ac"] = accepts_ac,
			["accepts_bc"] = accepts_bc,
			["accepts_a"] = accepts_a,
			["thesis"] = "Finite labeled DAG induces a regular language via NFA; determinization yields DFA"
		});
	}

	private static List<string> GetAutomataInputs()
	{
		return new List<string>
		{
			"ac","bc","a","b","c","",
			"aa","bb","cc","abc","bac","cab",
			"acc","bcc","ab","ba","ca","cb","aaa","bbb"
		};
	}

	private static bool Accepts(Dictionary<int, List<(char label, int to)>> g, int start, HashSet<int> finals, string input)
	{
		var current = new HashSet<int> { start };
		foreach (var ch in input)
		{
			var next = new HashSet<int>();
			foreach (var q in current)
			{
				if (!g.TryGetValue(q, out var edges)) continue;
				foreach (var (label, to) in edges)
				{
					if (label == ch) next.Add(to);
				}
			}
			current = next;
			if (current.Count == 0) break;
		}
		return current.Overlaps(finals);
	}

	private static int DeterminizedSize(Dictionary<int, List<(char label, int to)>> g, IReadOnlyList<char> sigma, int start)
	{
		var startSet = new HashSet<int> { start };
		var comparer = HashSetComparer<int>.Instance;
		var visited = new HashSet<HashSet<int>>(comparer);
		var queue = new Queue<HashSet<int>>();
		visited.Add(startSet);
		queue.Enqueue(startSet);
		while (queue.Count > 0)
		{
			var S = queue.Dequeue();
			foreach (var a in sigma)
			{
				var T = new HashSet<int>();
				foreach (var q in S)
				{
					if (!g.TryGetValue(q, out var edges)) continue;
					foreach (var (label, to) in edges)
					{
						if (label == a) T.Add(to);
					}
				}
				if (T.Count == 0) continue;
				if (!visited.Contains(T))
				{
					visited.Add(T);
					queue.Enqueue(T);
				}
			}
		}
		return visited.Count;
	}

	private sealed class HashSetComparer<T> : IEqualityComparer<HashSet<T>> where T : notnull
	{
		public static readonly HashSetComparer<T> Instance = new();
		public bool Equals(HashSet<T>? x, HashSet<T>? y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (x is null || y is null) return false;
			return x.SetEquals(y);
		}
		public int GetHashCode(HashSet<T> obj)
		{
			unchecked
			{
				int h = 19;
				foreach (var v in obj.OrderBy(e => e)) h = h * 31 + v!.GetHashCode();
				return h;
			}
		}
	}
}


