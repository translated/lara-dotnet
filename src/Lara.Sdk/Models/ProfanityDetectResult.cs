using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class ProfanityMatch
{
    public string Text { get; }

    public int StartCharIndex { get; }

    public int EndCharIndex { get; }

    public double Score { get; }

    [JsonConstructor]
    public ProfanityMatch(string text, int startCharIndex, int endCharIndex, double score)
    {
        Text = text;
        StartCharIndex = startCharIndex;
        EndCharIndex = endCharIndex;
        Score = score;
    }
}

public class ProfanityDetectResult
{
    public string MaskedText { get; }

    public ProfanityMatch[] Profanities { get; }

    public string? Error { get; }

    [JsonConstructor]
    public ProfanityDetectResult(string maskedText, ProfanityMatch[] profanities, string? error = null)
    {
        MaskedText = maskedText;
        Profanities = profanities;
        Error = error;
    }
}
