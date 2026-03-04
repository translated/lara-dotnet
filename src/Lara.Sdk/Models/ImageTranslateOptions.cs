namespace Lara.Sdk;

/// <summary>
/// Options that control how image translation is performed.
/// </summary>
public class ImageTranslateOptions
{
    /// <summary>
    /// Specifies target domains, audiences, or use cases the translation should adapt to.
    /// </summary>
    public string[]? AdaptTo { get; set; }
    
    /// <summary>
    /// Identifiers of glossaries to apply when translating text within the image.
    /// </summary>
    public string[]? Glossaries { get; set; }
    
    /// <summary>
    /// When true, attempts to avoid leaving trace artifacts of the original text in the translated image.
    /// </summary>
    public bool NoTrace { get; set; }
    
    /// <summary>
    /// Overrides the default translation style used for text detected in the image.
    /// </summary>
    public TranslationStyle? Style { get; set; }
    
    /// <summary>
    /// Configures how source text is removed or hidden from the image during translation.
    /// </summary>
    public ImageTextRemoval? TextRemoval { get; set; }
}