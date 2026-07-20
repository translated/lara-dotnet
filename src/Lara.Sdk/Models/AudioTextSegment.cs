namespace Lara.Sdk;

/// <summary>
/// Represents a translated audio transcript segment.
/// </summary>
public class AudioTextSegment
{
    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the segment start time.
    /// </summary>
    public double? Start { get; set; }

    /// <summary>
    /// Gets or sets the segment end time.
    /// </summary>
    public double? End { get; set; }

    /// <summary>
    /// Gets or sets the source transcript text for the segment.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the translated transcript text for the segment.
    /// </summary>
    public string? Translation { get; set; }
}
