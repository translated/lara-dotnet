using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Profanities result returned within a translation response.
/// Each field can be a single <see cref="ProfanityDetectResult"/> (single-string translation)
/// or an array of them (batch translation), matching the API's polymorphic shape.
public class ProfanitiesResult
{
    public object Target { get; }

    public object? Source { get; }

    [JsonConstructor]
    public ProfanitiesResult(object target, object? source = null)
    {
        Target = target;
        Source = source;
    }
}
