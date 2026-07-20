namespace Lara.Sdk;

/// <summary>
/// Represents options for audio transcript translation operations.
/// </summary>
public class AudioTranscriptOptions
{
    /// <summary>
    /// Optional audience or domain adaptation codes to tailor the translation.
    /// </summary>
    public string[]? AdaptTo { get; set; }

    /// <summary>
    /// Optional list of glossary identifiers to apply during translation.
    /// </summary>
    public string[]? Glossaries { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable tracing for this request.
    /// </summary>
    public bool NoTrace { get; set; }

    /// <summary>
    /// Optional translation style to influence the output.
    /// </summary>
    public TranslationStyle? Style { get; set; }
}
