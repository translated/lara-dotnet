using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents an HTTP response from the Lara API with convenient type conversion methods.
public class ClientResponse
{
    private readonly JsonElement _content;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly byte[]? _rawBytes;
    private readonly string? _mediaType;

    /// Gets the HTTP status code.
    public int StatusCode { get; }

    /// Gets the raw response content as a JsonElement. Only valid for JSON responses.
    public JsonElement Content => _content;

    /// Gets the Content-Type media type of the response (e.g., application/json, text/csv).
    public string? MediaType => _mediaType;

    /// Gets the raw response bytes. Only available for non-JSON responses.
    public byte[]? RawBytes => _rawBytes;

    /// Initializes a new instance of the ClientResponse class.
    public ClientResponse(int statusCode, JsonElement content, JsonSerializerOptions jsonOptions)
    {
        StatusCode = statusCode;
        _content = content;
        _jsonOptions = jsonOptions;
        _rawBytes = null;
        _mediaType = "application/json";
    }

    /// Initializes a new instance of the ClientResponse class for non-JSON content.
    public ClientResponse(int statusCode, byte[] rawBytes, string? mediaType, JsonSerializerOptions jsonOptions)
    {
        StatusCode = statusCode;
        _rawBytes = rawBytes;
        _mediaType = mediaType;
        _jsonOptions = jsonOptions;
        _content = default;
    }

    /// Deserializes the response content as a list of the specified type.
    public List<T> AsList<T>()
    {
        if (_mediaType != null && _mediaType != "application/json")
            throw new InvalidOperationException("Response is not JSON; cannot deserialize.");
        return JsonSerializer.Deserialize<List<T>>(_content.GetRawText(), _jsonOptions)!;
    }

    /// Deserializes the response content from a wrapper structure to the specified type.
    public T AsWrapped<T>()
    {
        if (_mediaType != null && _mediaType != "application/json")
            throw new InvalidOperationException("Response is not JSON; cannot deserialize.");
        var wrapper = JsonSerializer.Deserialize<ResponseWrapper<T>>(_content.GetRawText(), _jsonOptions);
        if (wrapper == null)
            throw new InvalidOperationException("Failed to deserialize response wrapper");
        return wrapper.Content;
    }

    /// Deserializes the response content from a wrapper structure as a list of the specified type.
    public List<T> AsWrappedList<T>()
    {
        if (_mediaType != null && _mediaType != "application/json")
            throw new InvalidOperationException("Response is not JSON; cannot deserialize.");
        var wrapper = JsonSerializer.Deserialize<ResponseWrapper<List<T>>>(_content.GetRawText(), _jsonOptions);
        if (wrapper == null)
            return new List<T>();
        return wrapper.Content;
    }

}

/// Generic response wrapper for API responses with status and content
internal class ResponseWrapper<T>
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("content")]
    public T Content { get; set; } = default!;
}