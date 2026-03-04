using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class DetectPrediction
{
    public string Language { get; }
    public float Confidence { get; }

    [JsonConstructor]
    public DetectPrediction(string language, float confidence)
    {
        Language = language;
        Confidence = confidence;
    }
}