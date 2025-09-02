namespace GraphIABook.Graphs.Chapter11;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;

/// <summary>
/// Chapter 11 — Demonstrative applications with three GRAPH flows:
/// Tourism concierge, Finance fraud detection, and Health triage.
/// Each graph leverages parallel branches and a deterministic merge.
/// </summary>
public static class GraphChapter11
{
    // Tourism graph: parallel hotels + weather + events -> fuse -> answer
    public static readonly int THotelsMs = 10;
    public static readonly int TWeatherMs = 7;
    public static readonly int TEventsMs = 6;
    public static readonly int TFuseMs = 4;

    // Finance graph: parallel geo + history + external + ml -> decision
    public static readonly int FGeoMs = 6;
    public static readonly int FHistoryMs = 6;
    public static readonly int FExternalMs = 8;
    public static readonly int FMLMs = 7;
    public static readonly int FDecisionMs = 4;

    // Health graph: parallel text + emr + imaging -> fuse -> classify
    public static readonly int HTextMs = 6;
    public static readonly int HEmrMs = 6;
    public static readonly int HImagingMs = 9;
    public static readonly int HFuseMs = 5;
    public static readonly int HClassifyMs = 5;

    public static async Task<string> RunTourismAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateTourismExecutor();
        var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
        if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 11 (Tourism).");

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    public static GraphExecutor CreateTourismExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");
        var hotels = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var topic = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["hotels"] = $"H({topic.Trim()})";
            await Task.Delay(THotelsMs);
            return "hotels";
        }, "Hotels"), nodeId: "hotels");

        var weather = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var topic = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["weather"] = $"W({topic.Trim()})";
            await Task.Delay(TWeatherMs);
            return "weather";
        }, "Weather"), nodeId: "weather");

        var eventsNode = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var topic = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["events"] = $"E({topic.Trim()})";
            await Task.Delay(TEventsMs);
            return "events";
        }, "Events"), nodeId: "events");

        var fuse = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var h = a.TryGetValue("hotels", out var hv) ? hv?.ToString() ?? string.Empty : string.Empty;
            var w = a.TryGetValue("weather", out var wv) ? wv?.ToString() ?? string.Empty : string.Empty;
            var e = a.TryGetValue("events", out var ev) ? ev?.ToString() ?? string.Empty : string.Empty;
            await Task.Delay(TFuseMs);
            return $"Tourism: {h}|{w}|{e}";
        }, "Fuse"), nodeId: "fuse");

        start.ConnectTo(hotels);
        start.ConnectTo(weather);
        start.ConnectTo(eventsNode);
        hotels.ConnectTo(fuse);
        weather.ConnectTo(fuse);
        eventsNode.ConnectTo(fuse);

        var executor = new GraphExecutor("ch11_tourism", "Tourism concierge with parallel queries and fuse");
        executor.AddNode(start)
            .AddNode(hotels)
            .AddNode(weather)
            .AddNode(eventsNode)
            .AddNode(fuse);
        executor.SetStartNode("start");
        executor.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 4 });
        return executor;
    }

    public static async Task<string> RunFinanceAsync(string tx)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["tx"] = tx };

        var executor = CreateFinanceExecutor();
        var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
        if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 11 (Finance).");

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    public static GraphExecutor CreateFinanceExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var geo = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["geo"] = "ok";
            await Task.Delay(FGeoMs);
            return "geo";
        }, "Geo"), nodeId: "geo");

        var hist = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["hist"] = "ok";
            await Task.Delay(FHistoryMs);
            return "hist";
        }, "History"), nodeId: "hist");

        var ext = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["risk"] = "ext";
            await Task.Delay(FExternalMs);
            return "external";
        }, "External"), nodeId: "external");

        var ml = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["ml"] = 0.6; // dummy score
            await Task.Delay(FMLMs);
            return "ml";
        }, "ML"), nodeId: "ml");

        var decision = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var score = 0;
            if ((a["geo"]?.ToString() ?? string.Empty) == "ok") score += 1;
            if ((a["hist"]?.ToString() ?? string.Empty) == "ok") score += 1;
            if ((a["risk"]?.ToString() ?? string.Empty) == "ext") score += 1;
            var mlScore = a.TryGetValue("ml", out var mv) && mv is double d ? d : 0.0;
            var approved = score >= 2 && mlScore >= 0.5;
            await Task.Delay(FDecisionMs);
            return approved ? "approve" : "review";
        }, "Decision"), nodeId: "decision");

        start.ConnectTo(geo);
        start.ConnectTo(hist);
        start.ConnectTo(ext);
        start.ConnectTo(ml);
        geo.ConnectTo(decision);
        hist.ConnectTo(decision);
        ext.ConnectTo(decision);
        ml.ConnectTo(decision);

        var executor = new GraphExecutor("ch11_finance", "Finance fraud with parallel checks and decision");
        executor.AddNode(start)
            .AddNode(geo)
            .AddNode(hist)
            .AddNode(ext)
            .AddNode(ml)
            .AddNode(decision);
        executor.SetStartNode("start");
        executor.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 4 });
        return executor;
    }

    public static async Task<string> RunHealthAsync(string symptoms)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["symptoms"] = symptoms };

        var executor = CreateHealthExecutor();
        var (isAcyclic, _) = GraphIABook.Benchmark._common.GraphValidationUtils.Analyze(executor);
        if (!isAcyclic) throw new InvalidOperationException("Graph must be acyclic for Chapter 11 (Health).");

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    public static GraphExecutor CreateHealthExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var text = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var s = a.ContainsKey("symptoms") ? a["symptoms"]?.ToString() ?? string.Empty : string.Empty;
            a["t"] = $"nlp({s.Trim()})";
            await Task.Delay(HTextMs);
            return "text";
        }, "Text"), nodeId: "text");

        var emr = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["e"] = "emr";
            await Task.Delay(HEmrMs);
            return "emr";
        }, "EMR"), nodeId: "emr");

        var imaging = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["img"] = "xray";
            await Task.Delay(HImagingMs);
            return "imaging";
        }, "Imaging"), nodeId: "imaging");

        var fuse = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var t = a.TryGetValue("t", out var tv) ? tv?.ToString() ?? string.Empty : string.Empty;
            var e = a.TryGetValue("e", out var ev) ? ev?.ToString() ?? string.Empty : string.Empty;
            var i = a.TryGetValue("img", out var iv) ? iv?.ToString() ?? string.Empty : string.Empty;
            a["fusion"] = $"{t}|{e}|{i}";
            await Task.Delay(HFuseMs);
            return "fused";
        }, "Fuse"), nodeId: "fuse");

        var classify = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var fusion = a.TryGetValue("fusion", out var fv) ? fv?.ToString() ?? string.Empty : string.Empty;
            await Task.Delay(HClassifyMs);
            return $"triage({fusion})";
        }, "Classify"), nodeId: "classify");

        start.ConnectTo(text);
        start.ConnectTo(emr);
        start.ConnectTo(imaging);
        text.ConnectTo(fuse);
        emr.ConnectTo(fuse);
        imaging.ConnectTo(fuse);
        fuse.ConnectTo(classify);

        var executor = new GraphExecutor("ch11_health", "Health triage with multimodal fusion and classification");
        executor.AddNode(start)
            .AddNode(text)
            .AddNode(emr)
            .AddNode(imaging)
            .AddNode(fuse)
            .AddNode(classify);
        executor.SetStartNode("start");
        executor.ConfigureConcurrency(new GraphConcurrencyOptions { EnableParallelExecution = true, MaxDegreeOfParallelism = 4 });
        return executor;
    }
}


