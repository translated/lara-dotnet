using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class ReasoningFeedback
{
    [JsonPropertyName("applied")]
    public bool Applied { get; set; }

    [JsonPropertyName("error")]
    public IntelligenceError? Error { get; set; }

    [JsonPropertyName("warnings")]
    public List<ReasoningWarning> Warnings { get; set; } = new();
}
