namespace Lara.Sdk;

/// <summary>
/// Options for translating text detected within images.
/// </summary>
public class ImageTextTranslateOptions
{
    /// <summary>
    /// Target locales or audience variants to adapt the translation to,
    /// such as specific regions or language variants.
    /// </summary>
    public string[]? AdaptTo { get; set; }
    
    /// <summary>
    /// Identifiers of glossaries to apply during translation to enforce
    /// custom terminology or vocabulary.
    /// </summary>
    public string[]? Glossaries { get; set; }
    
    /// <summary>
    /// When set to <c>true</c>, requests that translation traces or
    /// additional logging data are not recorded.
    /// </summary>
    public bool NoTrace { get; set; }
    
    /// <summary>
    /// Optional translation style that guides formality, tone, or other
    /// stylistic preferences for the translated text.
    /// </summary>
    public TranslationStyle? Style { get; set; }
    
    /// <summary>
    /// When set to <c>true</c>, enables more detailed translation output
    /// or diagnostic information where supported.
    /// </summary>
    public bool Verbose { get; set; }
}