using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents a glossary match result.
/// </summary>
/// <param name="Glossary">The glossary ID</param>
/// <param name="Language">The language pair</param>
/// <param name="Term">The source term</param>
/// <param name="Translation">The target translation</param>
public sealed record NGGlossaryMatch(
    [property: JsonPropertyName("glossary")] string Glossary,
    [property: JsonPropertyName("language")] List<string> Language,
    [property: JsonPropertyName("term")] string Term,
    [property: JsonPropertyName("translation")] string Translation)
{
    // Java-style getter methods for compatibility
    public string GetGlossary() => Glossary;
    public List<string> GetLanguage() => Language;
    public string GetTerm() => Term;
    public string GetTranslation() => Translation;

    public override string ToString() =>
        $"NGGlossaryMatch{{glossary='{Glossary}', language=[{string.Join(", ", Language)}], term='{Term}', translation='{Translation}'}}";
}