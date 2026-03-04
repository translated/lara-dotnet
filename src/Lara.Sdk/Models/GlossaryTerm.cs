using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a glossary term with language and value.
public class GlossaryTerm
{
    /// Gets the language code for this term.
    public string Language { get; }

    /// Gets the value/translation for this term.
    public string Value { get; }

    /// Initializes a new instance of the GlossaryTerm class.
    [JsonConstructor]
    public GlossaryTerm(string language, string value)
    {
        Language = language;
        Value = value;
    }

}
