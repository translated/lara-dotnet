using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class DetectResult
{
    public string Language { get;  }
    
    public string ContentType { get;  }
    
    public DetectPrediction[] Predictions { get;  }
 
    [JsonConstructor]
    public DetectResult(string language, string contentType, DetectPrediction[] predictions)
    {
        Language = language;
        ContentType = contentType;
        Predictions = predictions;
    }
}