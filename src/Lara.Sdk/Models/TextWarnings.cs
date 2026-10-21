using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class TextWarnings
{
    [JsonPropertyName("reasoning")]
    public ReasoningFeedback? Reasoning { get; set; }

    [JsonPropertyName("references")]
    public ReferenceRetrievalFailure? References { get; set; }

    [JsonPropertyName("prosa")]
    public IntelligenceFailure? Prosa { get; set; }
}
