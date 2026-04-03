using System.Text.Json.Serialization;

namespace Lara.Sdk;

public enum ProfanityFormat
{
    [JsonPropertyName("text")]
    Text,

    [JsonPropertyName("html")]
    Html,

    [JsonPropertyName("xml")]
    Xml,

    [JsonPropertyName("xliff")]
    Xliff
}
