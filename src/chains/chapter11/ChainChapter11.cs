namespace GraphIABook.Chains.Chapter11;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 11 — Demonstrative applications with three sequential (CHAIN) flows:
/// Tourism concierge, Finance fraud check, and Health triage.
/// Each flow is linear to contrast against the graph version.
/// </summary>
public static class ChainChapter11
{
    // Tourism (concierge): interpret -> hotels -> weather -> events -> answer
    public static readonly int TInterpretMs = 4;
    public static readonly int THotelsMs = 10;
    public static readonly int TWeatherMs = 7;
    public static readonly int TEventsMs = 6;
    public static readonly int TAnswerMs = 5;

    // Finance (fraud): ingest -> geolocation -> history -> external risk -> decision
    public static readonly int FIngestMs = 3;
    public static readonly int FGeoMs = 6;
    public static readonly int FHistoryMs = 6;
    public static readonly int FExternalMs = 8;
    public static readonly int FDecisionMs = 4;

    // Health (triage): symptoms -> text analysis -> emr -> imaging -> classify
    public static readonly int HSymptomsMs = 4;
    public static readonly int HTextMs = 6;
    public static readonly int HEmrMs = 6;
    public static readonly int HImagingMs = 9;
    public static readonly int HClassifyMs = 5;

    /// <summary>
    /// Runs the tourism linear pipeline.
    /// </summary>
    public static async Task<string> RunTourismAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var interpret = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var text = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            a["topic"] = text.Trim();
            await Task.Delay(TInterpretMs);
            return "interpreted";
        }, "Interpret");

        var hotels = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var topic = a.TryGetValue("topic", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["hotels"] = $"H({topic})";
            await Task.Delay(THotelsMs);
            return "hotels";
        }, "Hotels");

        var weather = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var topic = a.TryGetValue("topic", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["weather"] = $"W({topic})";
            await Task.Delay(TWeatherMs);
            return "weather";
        }, "Weather");

        var eventsFn = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var topic = a.TryGetValue("topic", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["events"] = $"E({topic})";
            await Task.Delay(TEventsMs);
            return "events";
        }, "Events");

        var answer = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var h = a.TryGetValue("hotels", out var hv) ? hv?.ToString() ?? string.Empty : string.Empty;
            var w = a.TryGetValue("weather", out var wv) ? wv?.ToString() ?? string.Empty : string.Empty;
            var e = a.TryGetValue("events", out var ev) ? ev?.ToString() ?? string.Empty : string.Empty;
            await Task.Delay(TAnswerMs);
            return $"Tourism: {h}|{w}|{e}";
        }, "Answer");

        await kernel.InvokeAsync(interpret, args);
        await kernel.InvokeAsync(hotels, args);
        await kernel.InvokeAsync(weather, args);
        var result = await kernel.InvokeAsync(eventsFn, args);
        _ = await kernel.InvokeAsync(answer, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Runs the finance linear pipeline.
    /// </summary>
    public static async Task<string> RunFinanceAsync(string transaction)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["tx"] = transaction };

        var ingest = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["norm"] = (a["tx"]?.ToString() ?? string.Empty).Trim().ToLowerInvariant();
            await Task.Delay(FIngestMs);
            return "ingested";
        }, "Ingest");

        var geo = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["geo"] = "geo-ok";
            await Task.Delay(FGeoMs);
            return "geo";
        }, "Geo");

        var history = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["hist"] = "hist-ok";
            await Task.Delay(FHistoryMs);
            return "history";
        }, "History");

        var external = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["risk"] = "risk-ext";
            await Task.Delay(FExternalMs);
            return "external";
        }, "External");

        var decision = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var score = 0;
            if ((a["geo"]?.ToString() ?? string.Empty).Contains("ok")) score += 1;
            if ((a["hist"]?.ToString() ?? string.Empty).Contains("ok")) score += 1;
            if ((a["risk"]?.ToString() ?? string.Empty).Contains("ext")) score += 1;
            await Task.Delay(FDecisionMs);
            return score >= 2 ? "approve" : "review";
        }, "Decision");

        await kernel.InvokeAsync(ingest, args);
        await kernel.InvokeAsync(geo, args);
        await kernel.InvokeAsync(history, args);
        await kernel.InvokeAsync(external, args);
        var result = await kernel.InvokeAsync(decision, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Runs the health triage linear pipeline.
    /// </summary>
    public static async Task<string> RunHealthAsync(string symptoms)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["symptoms"] = symptoms };

        var s = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["s"] = (a["symptoms"]?.ToString() ?? string.Empty).Trim();
            await Task.Delay(HSymptomsMs);
            return "symptoms";
        }, "Symptoms");

        var text = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["t"] = $"nlp({a["s"]})";
            await Task.Delay(HTextMs);
            return "text";
        }, "Text");

        var emr = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["e"] = "emr";
            await Task.Delay(HEmrMs);
            return "emr";
        }, "EMR");

        var imaging = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            a["img"] = "xray";
            await Task.Delay(HImagingMs);
            return "imaging";
        }, "Imaging");

        var classify = KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            var summary = $"triage({a["t"]}|{a["e"]}|{a["img"]})";
            await Task.Delay(HClassifyMs);
            return summary;
        }, "Classify");

        await kernel.InvokeAsync(s, args);
        await kernel.InvokeAsync(text, args);
        await kernel.InvokeAsync(emr, args);
        await kernel.InvokeAsync(imaging, args);
        var result = await kernel.InvokeAsync(classify, args);
        return result.GetValue<string>() ?? string.Empty;
    }
}


