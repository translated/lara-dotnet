using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class ReasoningWarning
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
