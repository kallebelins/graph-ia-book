namespace GraphIABook;

using GraphIABook.Chapters;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		int? chapter = null;
		string? mode = null;

		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] is "-c" or "--chapter")
			{
				if (i + 1 < args.Length && int.TryParse(args[i + 1], out int c))
				{
					chapter = c;
					i++;
				}
			}
			else if (args[i] is "-m" or "--mode")
			{
				if (i + 1 < args.Length)
				{
					mode = args[i + 1];
					i++;
				}
			}
		}

		if (chapter is null)
		{
			Console.Write("Capítulo (número, ex: 1): ");
			var input = Console.ReadLine();
			_ = int.TryParse(input, out var cVal);
			chapter = cVal;
		}

		if (string.IsNullOrWhiteSpace(mode))
		{
			Console.Write("Modo [(c)hain | (g)raph | (b)enchmark]: ");
			mode = Console.ReadLine();
		}

		var chapterInstance = CreateChapter(chapter!.Value);
		if (chapterInstance is null)
		{
			Console.Error.WriteLine($"Capítulo {chapter} não implementado.");
			return 1;
		}

		mode = mode?.Trim().ToLowerInvariant();
		switch (mode)
		{
			case "c":
			case "chain":
				await chapterInstance.RunChainAsync();
				break;
			case "g":
			case "graph":
				await chapterInstance.RunGraphAsync();
				break;
			case "b":
			case "benchmark":
			default:
				await chapterInstance.RunBenchmarkAsync();
				break;
		}

		return 0;
	}

	private static IChapter? CreateChapter(int chapter)
	{
		return chapter switch
		{
			0 => new Chapter0e(),
			1 => new Chapter1(),
			2 => new Chapter2(),
			3 => new Chapter3(),
			4 => new Chapter4(),
			5 => new Chapter5(),
			6 => new Chapter6(),
			7 => new Chapter7(),
			8 => new Chapter8(),
			9 => new Chapter9(),
			10 => new Chapter10(),
			11 => new Chapter11(),
			12 => new Chapter12(),
			13 => new Chapter13(),
			14 => new Chapter14(),
			15 => new Chapter15(),
			16 => new Chapter16(),
			17 => new Chapter17(),
			18 => new Chapter18(),
			19 => new Chapter19(),
			20 => new Chapter20(),
			21 => new Chapter21(),
			22 => new Chapter22(),
			23 => new Chapter23(),
			24 => new Chapter24(),
			25 => new Chapter25(),
			26 => new Chapter26(),
			_ => null
		};
	}
}


