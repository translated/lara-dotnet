using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents glossary term counts per language pair.
public class GlossaryCounts
{
    [JsonPropertyName("unidirectional")]
    public Dictionary<string, int> Unidirectional { get; set; } = new();

    [JsonPropertyName("multidirectional")]
    public int Multidirectional { get; set; }

    public Dictionary<string, int> GetUnidirectional() => Unidirectional;
    public int GetMultidirectional() => Multidirectional;
}