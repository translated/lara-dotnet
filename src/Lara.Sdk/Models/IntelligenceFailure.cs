using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class IntelligenceFailure
{
    [JsonPropertyName("applied")]
    public bool Applied { get; set; }

    [JsonPropertyName("error")]
    public IntelligenceError Error { get; set; } = new();
}
