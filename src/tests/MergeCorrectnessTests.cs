using GraphIABook.Graphs.Chapter2;
using Microsoft.SemanticKernel;
using Xunit;

namespace GraphIABook.Tests;

public sealed class MergeCorrectnessTests
{
    [Fact]
    public async Task Chapter2_Graph_Merge_UsesBothBranches()
    {
        var input = "A B C";
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };
        var executor = GraphChapter2.CreateExecutor();

        var result = await executor.ExecuteAsync(kernel, args);
        var answer = result.GetValue<string>() ?? string.Empty;

        Assert.Contains("graph(", answer);
        Assert.Contains("|", answer);

        var norm = input.Trim().ToLowerInvariant();
        var expectedA = string.Join('-', norm.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2));
        var expectedB = new string(norm.Reverse().ToArray());

        Assert.Contains(expectedA, answer);
        Assert.Contains(expectedB, answer);
    }
}


