using System.Text.Json.Serialization;

namespace Lara;

/// Represents the priority levels for translation operations.
public enum TranslatePriority
{
    /// Normal priority translation.
    [JsonPropertyName("normal")]
    Normal,

    /// Background priority translation.
    [JsonPropertyName("background")]
    Background
}