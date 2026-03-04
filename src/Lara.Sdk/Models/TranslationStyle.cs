using System.Text.Json.Serialization;

namespace Lara.Sdk;

public enum TranslationStyle
{
    /// Faithful translation that stays close to the original text.
    [JsonPropertyName("faithful")]
    Faithful,

    /// Fluid translation that prioritizes natural language flow.
    [JsonPropertyName("fluid")]
    Fluid,

    /// Creative translation that allows for more interpretative freedom.
    [JsonPropertyName("creative")]
    Creative
}