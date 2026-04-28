using System.Text.Json.Serialization;

namespace Lara.Sdk;

public enum ProfanitiesHandling
{
    [JsonPropertyName("hide")]
    Hide,

    [JsonPropertyName("avoid")]
    Avoid,

    [JsonPropertyName("detect")]
    Detect
}
