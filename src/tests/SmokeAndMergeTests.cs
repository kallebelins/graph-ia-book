using GraphIABook.Chains.Chapter2;
using GraphIABook.Graphs.Chapter2;
using Xunit;

namespace GraphIABook.Tests;

public sealed class SmokeAndMergeTests
{
    [Fact]
    public async Task Chapter2_Chain_RunAsync_ReturnsAnswer()
    {
        var result = await ChainChapter2.RunAsync("Hello World");
        Assert.NotNull(result);
        Assert.Contains("linear", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Chapter2_Graph_RunAsync_ReturnsMergedAnswer()
    {
        var input = "Hello World";
        var result = await GraphChapter2.RunAsync(input);
        Assert.NotNull(result);
        Assert.Contains("Answer: graph(", result);
        Assert.Contains("|", result);
        Assert.EndsWith(")", result);
    }
}


