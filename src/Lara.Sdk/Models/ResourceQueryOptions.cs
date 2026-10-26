namespace Lara.Sdk;

/// <summary>Options for filtering and paginating resource collections.</summary>
public class ResourceQueryOptions
{
    /// <summary>Filters resources by their visible name. Enables pagination when provided.</summary>
    public string? Search { get; set; }

    /// <summary>Page number (1-based). Enables pagination when provided.</summary>
    public long? Page { get; set; }

    /// <summary>Page size (1-200). Enables pagination when provided.</summary>
    public int? Size { get; set; }

    /// <summary>Converts the options to HTTP parameters.</summary>
    public HttpParams<object> ToParams() => new HttpParams<object>()
        .Set("search", Search)
        .Set("page", Page)
        .Set("size", Size);
}
