using System.Text.Json.Serialization;

namespace Lara;

/// <summary>
/// Represents a translation memory match result.
/// </summary>
/// <param name="Memory">The memory ID where the match was found</param>
/// <param name="Tuid">The translation unit identifier</param>
/// <param name="Language">The language pair</param>
/// <param name="Sentence">The source sentence that matched</param>
/// <param name="Translation">The target translation</param>
/// <param name="Score">The match score (0.0 to 1.0)</param>
public sealed record NGMemoryMatch(
    [property: JsonPropertyName("memory")] string Memory,
    [property: JsonPropertyName("tuid")] string? Tuid,
    [property: JsonPropertyName("language")] List<string> Language,
    [property: JsonPropertyName("sentence")] string Sentence,
    [property: JsonPropertyName("translation")] string Translation,
    [property: JsonPropertyName("score")] float Score)
{
    // Java-style getter methods for compatibility
    public string GetMemory() => Memory;
    public string? GetTuid() => Tuid;
    public List<string> GetLanguage() => Language;
    public string GetSentence() => Sentence;
    public string GetTranslation() => Translation;
    public float GetScore() => Score;

    public override string ToString() =>
        $"NGMemoryMatch{{memory='{Memory}', tuid='{Tuid}', language=[{string.Join(", ", Language)}], sentence='{Sentence}', translation='{Translation}', score={Score}}}";
}