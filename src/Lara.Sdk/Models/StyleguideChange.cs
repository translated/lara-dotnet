using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a single change applied by a styleguide during translation.
public class StyleguideChange
{
    /// Gets the optional identifier for this change.
    public string? Id { get; }

    /// Gets the original translation before the styleguide was applied.
    public string OriginalTranslation { get; }

    /// Gets the refined translation after the styleguide was applied.
    public string RefinedTranslation { get; }

    /// Gets the explanation for why the change was made.
    public string Explanation { get; }

    [JsonConstructor]
    public StyleguideChange(
        string? id,
        string originalTranslation,
        string refinedTranslation,
        string explanation)
    {
        Id = id;
        OriginalTranslation = originalTranslation;
        RefinedTranslation = refinedTranslation;
        Explanation = explanation;
    }
}
