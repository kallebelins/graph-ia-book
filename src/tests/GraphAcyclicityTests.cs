using GraphIABook.Graphs.Chapter1;
using GraphIABook.Graphs.Chapter2;
using GraphIABook.Graphs.Chapter3;
using GraphIABook.Graphs.Chapter4;
using GraphIABook.Graphs.Chapter5;
using GraphIABook.Graphs.Chapter6;
using GraphIABook.Graphs.Chapter7;
using GraphIABook.Graphs.Chapter8;
using GraphIABook.Graphs.Chapter9;
using GraphIABook.Graphs.Chapter10;
using GraphIABook.Graphs.Chapter11;
using GraphIABook.Graphs.Chapter12;
using GraphIABook.Graphs.Chapter13;
using GraphIABook.Graphs.Chapter14;
using GraphIABook.Graphs.Chapter15;
using GraphIABook.Graphs.Chapter16;
using GraphIABook.Graphs.Chapter17;
using GraphIABook.Graphs.Chapter18;
using GraphIABook.Graphs.Chapter19;
using GraphIABook.Graphs.Chapter20;
using GraphIABook.Graphs.Chapter21;
using GraphIABook.Graphs.Chapter22;
using GraphIABook.Graphs.Chapter23;
using GraphIABook.Graphs.Chapter24;
using GraphIABook.Graphs.Chapter25;
using GraphIABook.Graphs.Chapter26;
using SemanticKernel.Graph.Core;
using Xunit;

namespace GraphIABook.Tests;

public sealed class GraphAcyclicityTests
{
    public static IEnumerable<object[]> AllGraphExecutors()
    {
        yield return new object[] { "ch1", (Func<GraphExecutor>)(() => GraphChapter1.CreateExecutor()) };
        yield return new object[] { "ch2", (Func<GraphExecutor>)(() => GraphChapter2.CreateExecutor()) };
        yield return new object[] { "ch3", (Func<GraphExecutor>)(() => GraphChapter3.CreateExecutor()) };
        yield return new object[] { "ch4", (Func<GraphExecutor>)(() => GraphChapter4.CreateExecutor()) };
        yield return new object[] { "ch5", (Func<GraphExecutor>)(() => GraphChapter5.CreateExecutor()) };
        yield return new object[] { "ch6", (Func<GraphExecutor>)(() => GraphChapter6.CreateExecutor()) };
        yield return new object[] { "ch7", (Func<GraphExecutor>)(() => GraphChapter7.CreateExecutor()) };
        yield return new object[] { "ch8", (Func<GraphExecutor>)(() => GraphChapter8.CreateExecutor()) };
        yield return new object[] { "ch9", (Func<GraphExecutor>)(() => GraphChapter9.CreateExecutor()) };
        yield return new object[] { "ch10", (Func<GraphExecutor>)(() => GraphChapter10.CreateExecutor()) };
        yield return new object[] { "ch11", (Func<GraphExecutor>)(() => GraphChapter11.CreateExecutor()) };
        yield return new object[] { "ch12", (Func<GraphExecutor>)(() => GraphChapter12.CreateExecutor()) };
        yield return new object[] { "ch13", (Func<GraphExecutor>)(() => GraphChapter13.CreateExecutor()) };
        yield return new object[] { "ch14", (Func<GraphExecutor>)(() => GraphChapter14.CreateExecutor(includeAlternate: false)) };
        yield return new object[] { "ch15", (Func<GraphExecutor>)(() => GraphChapter15.CreateExecutor()) };
        yield return new object[] { "ch16", (Func<GraphExecutor>)(() => GraphChapter16.CreateExecutor()) };
        yield return new object[] { "ch17", (Func<GraphExecutor>)(() => GraphChapter17.CreateExecutor()) };
        yield return new object[] { "ch18", (Func<GraphExecutor>)(() => GraphChapter18.CreateExecutor()) };
        yield return new object[] { "ch19", (Func<GraphExecutor>)(() => GraphChapter19.CreateExecutor()) };
        yield return new object[] { "ch20", (Func<GraphExecutor>)(() => GraphChapter20.CreateExecutor()) };
        yield return new object[] { "ch21", (Func<GraphExecutor>)(() => GraphChapter21.CreateExecutor()) };
        yield return new object[] { "ch22", (Func<GraphExecutor>)(() => GraphChapter22.CreateExecutor()) };
        yield return new object[] { "ch23", (Func<GraphExecutor>)(() => GraphChapter23.CreateExecutor()) };
        yield return new object[] { "ch24", (Func<GraphExecutor>)(() => GraphChapter24.CreateExecutor()) };
        yield return new object[] { "ch25", (Func<GraphExecutor>)(() => GraphChapter25.CreateExecutor()) };
        yield return new object[] { "ch26", (Func<GraphExecutor>)(() => GraphChapter26.CreateExecutor()) };
    }

    [Theory]
    [MemberData(nameof(AllGraphExecutors))]
    public void Graphs_Are_Acyclic(string id, Func<GraphExecutor> factory)
    {
        var executor = factory();
        var plan = GraphPlanCompiler.Compile(executor);
        Assert.False(plan.HasCycles);
        Assert.NotNull(plan.TopologicalOrder);
        Assert.NotEmpty(plan.TopologicalOrder!);
    }
}


