using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents the result of text extraction and translation from an image, including source language, paragraphs, and associated resources.
/// </summary>
public class ImageTextResult
{
    /// <summary>
    /// Gets the source language of the text extracted from the image.
    /// </summary>
    public string SourceLanguage { get; }
    
    /// <summary>
    /// Gets an array of paragraphs extracted from the image.
    /// </summary>
    public ImageParagraph[] Paragraphs { get; }
    
    /// <summary>
    /// Gets an array of identifiers for the memories adapted to in the translation process.
    /// </summary>
    public string[] AdaptedTo { get; }
    
    /// <summary>
    /// Gets an array of identifiers for the glossaries used in the translation process.
    /// </summary>
    public string[] Glossaries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTextResult"/> class.
    /// </summary>
    /// <param name="sourceLanguage">The source language of the text.</param>
    /// <param name="paragraphs">An array of paragraphs from the image.</param>
    /// <param name="adaptedTo">An array of memory identifiers adapted to.</param>
    /// <param name="glossaries">An array of glossary identifiers used.</param>
    [JsonConstructor]
    public ImageTextResult(
        string sourceLanguage,
        ImageParagraph[] paragraphs,
        string[] adaptedTo,
        string[] glossaries
    )
    {
        SourceLanguage = sourceLanguage;
        Paragraphs = paragraphs;
        AdaptedTo = adaptedTo;
        Glossaries = glossaries;
    }
}