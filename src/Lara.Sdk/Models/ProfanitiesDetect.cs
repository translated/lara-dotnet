using System.Text.Json.Serialization;

namespace Lara.Sdk;

public enum ProfanitiesDetect
{
    [JsonPropertyName("target")]
    Target,

    [JsonPropertyName("source_target")]
    SourceTarget
}
