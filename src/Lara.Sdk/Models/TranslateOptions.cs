using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Options for text translation operations. Matches the Java TranslateOptions pattern.
public class TranslateOptions
{
    /// Gets or sets a hint for the source language.
    [JsonPropertyName("source_hint")]
    public string? SourceHint { get; set; }

    /// Gets or sets the memory IDs to adapt the translation to.
    [JsonPropertyName("adapt_to")]
    public string[]? AdaptTo { get; set; }

    /// Gets or sets the translation instructions.
    [JsonPropertyName("instructions")]
    public string[]? Instructions { get; set; }

    /// Gets or sets the glossary IDs to use for translation.
    [JsonPropertyName("glossaries")]
    public string[]? Glossaries { get; set; }

    /// Gets or sets the content type of the text being translated.
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    /// Gets or sets a value indicating whether to preserve multiline formatting.
    [JsonPropertyName("multiline")]
    public bool? Multiline { get; set; }

    /// Gets or sets the timeout for the translation operation in milliseconds.
    [JsonPropertyName("timeout")]
    public int? TimeoutInMillis { get; set; }

    /// Gets or sets the priority of the translation operation.
    [JsonPropertyName("priority")]
    public TranslatePriority? Priority { get; set; }

    /// Gets or sets the cache usage preference.
    [JsonPropertyName("use_cache")]
    public object? UseCache { get; set; } // bool or "overwrite"

    /// Gets or sets the cache TTL in seconds.
    [JsonPropertyName("cache_ttl")]
    public int? CacheTTLSeconds { get; set; }

    /// Gets or sets a value indicating whether to disable tracing for this request.
    [JsonPropertyName("no_trace")]
    public bool? NoTrace { get; set; }

    /// Gets or sets a value indicating whether to return verbose information.
    [JsonPropertyName("verbose")]
    public bool? Verbose { get; set; }

    /// Gets or sets additional headers to include with the request.
    [JsonIgnore]
    public Dictionary<string, object>? Headers { get; set; }

    /// Gets or sets the translation style.
    [JsonPropertyName("style")]
    public TranslationStyle? Style { get; set; }

    /// Converts the options to HTTP parameters. Matches Java toParams() method.
    /// <returns>HTTP parameters for the request.</returns>
    public HttpParams<object> ToParams()
    {
        var parameters = new HttpParams<object>()
            .Set("source_hint", SourceHint)
            .Set("adapt_to", AdaptTo)
            .Set("instructions", Instructions)
            .Set("glossaries", Glossaries)
            .Set("content_type", ContentType)
            .Set("multiline", Multiline)
            .Set("timeout", TimeoutInMillis)
            .Set("priority", Priority?.ToString().ToLowerInvariant())
            .Set("use_cache", UseCache)
            .Set("cache_ttl", CacheTTLSeconds)
            .Set("no_trace", NoTrace)
            .Set("verbose", Verbose)
            .Set("style", Style?.ToString().ToLowerInvariant());

        return parameters;
    }

    /// Gets the headers for the request.
    /// <returns>The headers dictionary.</returns>
    public Dictionary<string, object>? GetHeaders() => Headers;

    /// Gets the no trace setting.
    /// <returns>The no trace setting.</returns>
    public bool? GetNoTrace() => NoTrace;
}