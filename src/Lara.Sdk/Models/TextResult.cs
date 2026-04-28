using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents the result of a text translation operation.
/// </summary>
public class TextResult<T>
{

    public T Translation { get; }

    public string ContentType { get; }

    public string SourceLanguage { get; }

    public List<string>? AdaptedTo { get; }

    public List<string>? Glossaries { get; }

    public object? AdaptedToMatches { get; }

    public object? GlossariesMatches { get; }

    public ProfanitiesResult? Profanities { get; }

    public StyleguideResults<T>? StyleguideResults { get; }

    [Obsolete("Use the Translation property directly. This property will be removed in a future release.")]
    public string? SingleTranslation => Translation as string;

    [Obsolete("Use the Translation property directly. This property will be removed in a future release.")]
    public List<string>? MultipleTranslations
    {
        get
        {
            if (Translation is List<string> list)
                return list;
            if (Translation is string[] array)
                return array.ToList();
            return null;
        }
    }

    [Obsolete("Use the Translation property directly. This property will be removed in a future release.")]
    public List<TextBlock>? TextBlocks
    {
        get
        {
            if (Translation is List<TextBlock> list)
                return list;
            if (Translation is TextBlock[] array)
                return array.ToList();
            return null;
        }
    }

    [JsonConstructor]
    public TextResult(
        string contentType,
        string sourceLanguage,
        List<string>? adaptedTo,
        List<string>? glossaries,
        object? adaptedToMatches,
        object? glossariesMatches,
        ProfanitiesResult? profanities,
        StyleguideResults<T>? styleguideResults,
        T translation)
    {
        ContentType = contentType;
        SourceLanguage = sourceLanguage;
        AdaptedTo = adaptedTo;
        Glossaries = glossaries;
        AdaptedToMatches = adaptedToMatches;
        GlossariesMatches = glossariesMatches;
        Profanities = profanities;
        StyleguideResults = styleguideResults;
        Translation = translation;
    }
}