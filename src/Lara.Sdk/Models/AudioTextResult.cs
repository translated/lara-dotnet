using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents a translated audio transcript returned by Lara.
/// </summary>
public class AudioTextResult
{
    private List<AudioTextSegment>? _segments = new();

    /// <summary>
    /// Gets or sets the transcript identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the detected source language.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the target language.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the source filename.
    /// </summary>
    public string? Filename { get; set; }

    /// <summary>
    /// Gets or sets the audio duration.
    /// </summary>
    public double? Duration { get; set; }

    /// <summary>
    /// Gets or sets the source transcript text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the translated transcript text.
    /// </summary>
    public string? Translation { get; set; }

    /// <summary>
    /// Gets or sets transcript segments. Missing or null payload values are exposed as an empty list.
    /// </summary>
    [JsonPropertyName("segments")]
    public List<AudioTextSegment> Segments
    {
        get => _segments ?? new List<AudioTextSegment>();
        set => _segments = value ?? new List<AudioTextSegment>();
    }
}
