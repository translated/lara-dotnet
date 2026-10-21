using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class IntelligenceError
{
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
