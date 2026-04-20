using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents the result of a quality estimation operation.
/// </summary>
public class QualityEstimationResult
{
    /// <summary>
    /// Gets the score of the quality estimation.
    /// </summary>
    public float Score { get; }

    [JsonConstructor]
    public QualityEstimationResult(float score)
    {
        Score = score;
    }
}