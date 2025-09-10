using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lara;

/// <summary>
/// Represents the result of a text translation operation.
/// </summary>
public sealed class TextResult
{
    [JsonPropertyName("content_type")]
    public string ContentType { get; init; } = string.Empty;

    [JsonPropertyName("source_language")]
    public string SourceLanguage { get; init; } = string.Empty;

    [JsonPropertyName("adapted_to")]
    public List<string>? AdaptedTo { get; init; }

    [JsonPropertyName("glossaries")]
    public List<string>? Glossaries { get; init; }

    [JsonPropertyName("adapted_to_matches")]
    public object? AdaptedToMatches { get; init; }

    [JsonPropertyName("glossaries_matches")]
    public object? GlossariesMatches { get; init; }

    [JsonPropertyName("translation")]
    public object? Translation { get; init; }

    public string? SingleTranslation
    {
        get
        {
            if (Translation is string str)
                return str;
            if (Translation is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
                return jsonElement.GetString();
            return null;
        }
    }
    
    public List<string>? MultipleTranslations
    {
        get
        {
            if (Translation is List<string> list)
                return list;
            if (Translation is string[] array)
                return array.ToList();
            if (Translation is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
            {
                try
                {
                    var translations = new List<string>();
                    foreach (var item in jsonElement.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            translations.Add(item.GetString()!);
                        }
                        else if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                        {
                            translations.Add(textProp.GetString()!);
                        }
                    }
                    return translations;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }

    public List<TextBlock>? TextBlocks => Translation switch
    {
        List<TextBlock> list => list,
        TextBlock[] array => array.ToList(),
        JsonElement jsonElement when jsonElement.ValueKind == JsonValueKind.Array => 
            ParseJsonArray(jsonElement),
        _ => null
    };

    private static List<TextBlock> ParseJsonArray(JsonElement jsonElement)
    {
        var result = new List<TextBlock>();
        
        foreach (var item in jsonElement.EnumerateArray())
        {
            if (item.TryGetProperty("text", out var text) && 
                text.ValueKind == JsonValueKind.String)
            {
                var translatable = !item.TryGetProperty("translatable", out var trans) || 
                                 trans.ValueKind != JsonValueKind.False;
                result.Add(new TextBlock(text.GetString() ?? string.Empty, translatable));
            }
        }
        
        return result;
    }

    /// <summary>
    /// Convenience property alias for TextBlocks
    /// </summary>
    public List<TextBlock>? TextBlock => TextBlocks;

    public string GetContentType() => ContentType;
    public string GetSourceLanguage() => SourceLanguage;
    public List<string>? GetAdaptedTo() => AdaptedTo;
    public List<string>? GetGlossaries() => Glossaries;
    public string? GetTranslation() => SingleTranslation;
    public object? GetAdaptedToMatches() => AdaptedToMatches;
    public object? GetGlossariesMatches() => GlossariesMatches;

    public override string ToString()
    {
        if (SingleTranslation != null)
            return SingleTranslation;
        if (MultipleTranslations != null)
            return string.Join(", ", MultipleTranslations);
        return Translation?.ToString() ?? string.Empty;
    }
}