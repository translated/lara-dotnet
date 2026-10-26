using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>A collection of resources with total count and optional pagination metadata.</summary>
public class CollectionResult<T>
{
    /// <summary>The resources returned by this request.</summary>
    public List<T> Items { get; }

    /// <summary>Total number of matching resources.</summary>
    public long Total { get; }

    /// <summary>Current page (1-based), or null for an unpaginated collection.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Page { get; }

    /// <summary>Page size, or null for an unpaginated collection.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Size { get; }

    public CollectionResult(List<T> items, long total, long? page = null, int? size = null)
    {
        Items = items;
        Total = total;
        Page = page;
        Size = size;
    }
}
