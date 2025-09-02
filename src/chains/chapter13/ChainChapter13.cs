namespace GraphIABook.Chains.Chapter13;

using System.Text.Json;
using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 13 — SK chain trivial CSV→JSON transformation.
/// Demonstrates a case where a simple linear pipeline is preferable to a graph
/// due to minimal work and no exploitable parallelism.
/// </summary>
public static class ChainChapter13
{
	public static readonly int PreprocessMs = 1;
	public static readonly int ParseMs = 1;
	public static readonly int SerializeMs = 1;

	/// <summary>
	/// Converts a tiny CSV string into a JSON array using a linear SK pipeline.
	/// </summary>
	public static async Task<string> RunAsync(string csv)
	{
		var kernel = Kernel.CreateBuilder().Build();
		var args = new KernelArguments { ["csv"] = csv };

		var preprocess = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var raw = a.TryGetValue("csv", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
			a["lines"] = raw.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n', StringSplitOptions.RemoveEmptyEntries);
			await Task.Delay(PreprocessMs);
			return "preprocessed";
		}, "Preprocess");

		var parse = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var lines = (a["lines"] as string[]) ?? Array.Empty<string>();
			if (lines.Length == 0)
			{
				a["records"] = Array.Empty<Dictionary<string, string>>();
				return "parsed";
			}
			var headers = lines[0].Split(',', StringSplitOptions.TrimEntries);
			var list = new List<Dictionary<string, string>>();
			for (var i = 1; i < lines.Length; i++)
			{
				var cells = lines[i].Split(',', StringSplitOptions.TrimEntries);
				var map = new Dictionary<string, string>(capacity: headers.Length);
				for (var c = 0; c < headers.Length && c < cells.Length; c++)
				{
					map[headers[c]] = cells[c];
				}
				list.Add(map);
			}
			a["records"] = list;
			await Task.Delay(ParseMs);
			return "parsed";
		}, "Parse");

		var serialize = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
		{
			var records = a.TryGetValue("records", out var rv) ? (rv as List<Dictionary<string, string>>) ?? new List<Dictionary<string, string>>() : new List<Dictionary<string, string>>();
			var json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = false });
			await Task.Delay(SerializeMs);
			return json;
		}, "Serialize");

		await kernel.InvokeAsync(preprocess, args);
		await kernel.InvokeAsync(parse, args);
		var result = await kernel.InvokeAsync(serialize, args);
		return result.GetValue<string>() ?? string.Empty;
	}
}




