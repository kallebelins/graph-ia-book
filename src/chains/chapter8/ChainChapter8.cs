namespace GraphIABook.Chains.Chapter8;

using Microsoft.SemanticKernel;

/// <summary>
/// Chapter 8 — SK chain focusing on multimodal context, intentionally modeling a voice-only
/// sequential pipeline to highlight limitations of chains in multimodal scenarios.
/// Stages: (Audio) SpeechToText -> (Text) NLP -> Answer. Image is ignored.
/// </summary>
public static class ChainChapter8
{
    public static readonly int SpeechToTextMs = 80;  // simulates ASR latency
    public static readonly int NlpMs = 30;           // simulates simple NLP processing
    public static readonly int AnswerMs = 20;        // formatting/answer synthesis

    /// <summary>
    /// Runs a sequential voice-only pipeline. Input may contain placeholders for multiple modalities,
    /// but this chain ignores image by design to illustrate duplication/explosion issues.
    /// </summary>
    public static async Task<string> RunAsync(string input)
    {
        var kernel = Kernel.CreateBuilder().Build();

        var speechToText = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                await Task.Delay(SpeechToTextMs);
                var raw = (a["input"]?.ToString() ?? string.Empty);
                // Simulate: extract "voice" segment if present, else passthrough
                var transcript = raw.Contains("voice:") ? raw : $"voice:{raw}";
                a["text"] = transcript.Replace("voice:", string.Empty).Trim();
                return "asr-ok";
            },
            "SpeechToText");

        var nlp = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                await Task.Delay(NlpMs);
                var text = a.TryGetValue("text", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                a["intent"] = text.ToLowerInvariant();
                return "nlp-ok";
            },
            "Nlp");

        var answer = KernelFunctionFactory.CreateFromMethod(
            async (KernelArguments a) =>
            {
                await Task.Delay(AnswerMs);
                var intent = a.TryGetValue("intent", out var v) ? v?.ToString() ?? string.Empty : string.Empty;
                return $"answer(chain): {intent}";
            },
            "Answer");

        var args = new KernelArguments { ["input"] = input };
        await kernel.InvokeAsync(speechToText, args);
        await kernel.InvokeAsync(nlp, args);
        var result = await kernel.InvokeAsync(answer, args);
        return result.GetValue<string>() ?? string.Empty;
    }
}


