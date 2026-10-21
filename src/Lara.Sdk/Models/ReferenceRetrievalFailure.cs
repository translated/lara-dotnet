using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Reports failed or partially applied reference retrieval.
public class ReferenceRetrievalFailure
{
    /// A JSON boolean false or the string "partial".
    [JsonPropertyName("applied")]
    public object Applied { get; set; } = false;

    [JsonPropertyName("error")]
    public IntelligenceError Error { get; set; } = new();
}
