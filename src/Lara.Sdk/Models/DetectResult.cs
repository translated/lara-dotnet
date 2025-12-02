using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class DetectResult
{
    [JsonPropertyName("language")]
    public string Language { get;  }
    
    [JsonPropertyName("content_type")]
    public string ContentType { get;  }
    
    public DetectResult(string language, string contentType)
    {
        Language = language;
        ContentType = contentType;
    }
}