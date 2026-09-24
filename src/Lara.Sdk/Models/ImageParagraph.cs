using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents a paragraph extracted from an image, including the original text, its translation, and associated matches from memories and glossaries.
/// </summary>
public class ImageParagraph
{
    /// <summary>
    /// Gets the original text of the paragraph.
    /// </summary>
    public string Text { get; }
    
    /// <summary>
    /// Gets the translated text of the paragraph.
    /// </summary>
    public string Translation { get; }
    
    /// <summary>
    /// Gets an array of memory matches that were adapted to this paragraph.
    /// </summary>
    public NGMemoryMatch[] AdaptedToMatches { get; }
    
    /// <summary>
    /// Gets an array of glossary matches for this paragraph.
    /// </summary>
    [JsonPropertyName("glossaries_matches")]
    public NGGlossaryMatch[] GlossaryMatches { get; }

    /// <summary>
    /// Paragraph corners. This and the other layout properties are populated when
    /// <see cref="ImageTextTranslateOptions.IncludeLayout"/> is <c>true</c>.
    /// </summary>
    [JsonPropertyName("bbox")]
    public ImageBBox? BBox { get; init; }

    /// <summary>Bounding boxes for individual lines.</summary>
    [JsonPropertyName("lines_bboxes")]
    public ImageBBox[]? LinesBBoxes { get; init; }

    /// <summary>Original text direction and colors.</summary>
    public ImageTextInfo? TextInfo { get; init; }

    /// <summary>Text alignment: left, center, or right.</summary>
    public string? Alignment { get; init; }

    /// <summary>Creates a paragraph containing supplied text and its translation.</summary>
    public ImageParagraph(string text, string translation)
        : this(text, translation, Array.Empty<NGMemoryMatch>(), Array.Empty<NGGlossaryMatch>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageParagraph"/> class.
    /// </summary>
    /// <param name="text">The original text of the paragraph.</param>
    /// <param name="translation">The translated text of the paragraph.</param>
    /// <param name="adaptedToMatches">An array of memory matches adapted to this paragraph.</param>
    /// <param name="glossaryMatches">An array of glossary matches for this paragraph.</param>
    [JsonConstructor]
    public ImageParagraph(
        string text, 
        string translation, 
        NGMemoryMatch[] adaptedToMatches,
        NGGlossaryMatch[] glossaryMatches
    )
    {
        Text = text;
        Translation = translation;
        AdaptedToMatches = adaptedToMatches;
        GlossaryMatches = glossaryMatches;
    }
}
