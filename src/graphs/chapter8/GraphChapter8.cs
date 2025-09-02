namespace GraphIABook.Graphs.Chapter8;

using Microsoft.SemanticKernel;
using SemanticKernel.Graph.Core;
using SemanticKernel.Graph.Nodes;
using GraphIABook.Benchmark._common;

/// <summary>
/// Chapter 8 — SKG graph demonstrating multimodal fusion.
/// Subgraphs: SpeechToText (audio) -> NLP (text), ImageAnalyzer (vision). Both converge into FusionNode,
/// which combines signals and produces an answer. Demonstrates late fusion with simple weighting.
/// </summary>
public static class GraphChapter8
{
    public static readonly int SpeechToTextMs = 80;
    public static readonly int NlpMs = 30;
    public static readonly int ImageAnalyzerMs = 60;
    public static readonly int FusionMs = 40;

    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();
        var args = new KernelArguments { ["input"] = input };

        var executor = CreateExecutor();
        GraphValidationUtils.EnsureAcyclic(executor);

        var result = await executor.ExecuteAsync(kernel, args);
        return result.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    /// Builds the multimodal graph: start -> (ASR->NLP, Image) -> Fusion -> Answer.
    /// Implements late fusion by combining modality confidences and contents.
    /// </summary>
    public static GraphExecutor CreateExecutor()
    {
        var start = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(() => "start", "Start"), nodeId: "start");

        var speechToText = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(SpeechToTextMs);
            var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            var transcript = raw.Contains("voice:") ? raw : $"voice:{raw}";
            a["text"] = transcript.Replace("voice:", string.Empty).Trim();
            a["asr_conf"] = 0.85; // simulated confidence
            return "asr-ok";
        }, "SpeechToText"), nodeId: "asr");

        var nlp = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(NlpMs);
            var text = a.TryGetValue("text", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
            a["intent"] = text.ToLowerInvariant();
            a["nlp_conf"] = 0.9; // simulated confidence
            return "nlp-ok";
        }, "Nlp"), nodeId: "nlp");

        var imageAnalyzer = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(ImageAnalyzerMs);
            var raw = a.ContainsKey("input") ? a["input"]?.ToString() ?? string.Empty : string.Empty;
            // Simulate: extract image tag if present; else synthesize a generic feature
            var hasImage = raw.Contains("image:");
            a["vision_feature"] = hasImage ? "object:phone,color:black" : "object:unknown";
            a["vision_conf"] = hasImage ? 0.8 : 0.4; // lower confidence without explicit image
            return "vision-ok";
        }, "ImageAnalyzer"), nodeId: "vision");

        var fusion = new FunctionGraphNode(KernelFunctionFactory.CreateFromMethod(async (KernelArguments a) =>
        {
            await Task.Delay(FusionMs);
            var intent = a.TryGetValue("intent", out var iv) ? iv?.ToString() ?? string.Empty : string.Empty;
            var vfeat = a.TryGetValue("vision_feature", out var vf) ? vf?.ToString() ?? string.Empty : string.Empty;
            var asrC = a.TryGetValue("asr_conf", out var ac) ? Convert.ToDouble(ac) : 0.0;
            var nlpC = a.TryGetValue("nlp_conf", out var nc) ? Convert.ToDouble(nc) : 0.0;
            var visC = a.TryGetValue("vision_conf", out var vc) ? Convert.ToDouble(vc) : 0.0;
            // Simple late fusion: weighted average of confidences, biased towards text if provided
            var score = (asrC * 0.3) + (nlpC * 0.4) + (visC * 0.3);
            a["fusion_score"] = score;
            return $"answer(graph): intent={intent}; vision={vfeat}; score={score:F2}";
        }, "Fusion"), nodeId: "fusion");

        var executor = new GraphExecutor("ch8_multimodal", "Multimodal late fusion graph");
        executor.AddNode(start)
            .AddNode(speechToText)
            .AddNode(nlp)
            .AddNode(imageAnalyzer)
            .AddNode(fusion);

        executor.SetStartNode("start");
        executor.Connect("start", "asr");
        executor.Connect("asr", "nlp");
        executor.Connect("start", "vision");
        executor.Connect("nlp", "fusion");
        executor.Connect("vision", "fusion");

        executor.ConfigureConcurrency(new GraphConcurrencyOptions
        {
            EnableParallelExecution = true,
            MaxDegreeOfParallelism = 2
        });

        return executor;
    }
}


