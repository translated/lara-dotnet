using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents the results of styleguide processing during translation.
public class StyleguideResults<T>
{
    /// Gets the original translation before styleguide changes.
    public T OriginalTranslation { get; }

    /// Gets the list of changes applied by the styleguide.
    public List<StyleguideChange> Changes { get; }

    [JsonConstructor]
    public StyleguideResults(T originalTranslation, List<StyleguideChange> changes)
    {
        OriginalTranslation = originalTranslation;
        Changes = changes;
    }
}
