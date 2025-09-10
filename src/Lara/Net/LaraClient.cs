using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.IO;

namespace Lara;

/// Main HTTP client for the Lara API providing HMAC authentication
public class LaraClient
{
    private const string SigningAlgorithm = "HmacSHA256";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _baseUrl;
    private readonly TimeSpan _connectionTimeout;
    private readonly TimeSpan _readTimeout;
    private readonly string _accessKeyId;
    private readonly byte[] _signingKey;
    private readonly Dictionary<string, string> _extraHeaders = new();
    private readonly HttpClient _httpClient;

    /// Initializes a new instance of the <see cref="LaraClient"/> class with default options.
    public LaraClient(Credentials credentials) : this(credentials, new ClientOptions())
    {
    }

    /// Initializes a new instance of the <see cref="LaraClient"/> class.
    public LaraClient(Credentials credentials, ClientOptions options)
    {
        _accessKeyId = credentials.AccessKeyId;
        _signingKey = Encoding.UTF8.GetBytes(credentials.AccessKeySecret);
        
        _baseUrl = options.ServerUrl;
        _connectionTimeout = options.ConnectionTimeout;
        _readTimeout = options.ReadTimeout;
        
        _httpClient = new HttpClient();
        if (_connectionTimeout > TimeSpan.Zero)
            _httpClient.Timeout = _connectionTimeout;
    }

    /// Sets an extra header to be included with all requests.
    public void SetExtraHeader(string name, string value)
    {
        _extraHeaders[name] = value;
    }

    // GET method overloads
public async Task<ClientResponse> Get(
        string path, 
        Dictionary<string, object>? parameters = null, 
        Dictionary<string, string>? headers = null
        )
    {
        return await Request("GET", path, parameters, null, headers);
    }

    // DELETE method overloads
    public async Task<ClientResponse> Delete(
        string path, 
        Dictionary<string, object>? parameters = null, 
        Dictionary<string, string>? headers = null
        )
    {
        return await Request("DELETE", path, parameters, null, headers);
    }

    // POST method
    public async Task<ClientResponse> Post(
        string path, 
        Dictionary<string, object>? parameters = null, 
        Dictionary<string, string>? files = null, 
        Dictionary<string, string>? headers = null
        )
    {
        return await Request("POST", path, parameters, files, headers);
    }

    // PUT method overloads
    public async Task<ClientResponse> Put(
        string path, 
        Dictionary<string, object>? parameters = null, 
        Dictionary<string, string>? files = null,
        Dictionary<string, string>? headers = null
        )
    {
        return await Request("PUT", path, parameters, files, headers);
    }

    /// Performs an HTTP request with HMAC authentication
    private async Task<ClientResponse> Request(string method, string path, Dictionary<string, object>? parameters = null,
        Dictionary<string, string>? files = null, Dictionary<string, string>? headers = null)
    {
        path = NormalizePath(path);
        parameters = Prune(parameters);
        files = Prune(files);

        string? contentMd5 = null;
        string? contentType = null;
        HttpContent? requestContent = null;

        // Handle request body
        if (parameters != null || files != null)
        {
            if (files == null)
            {
                // JSON content
                var jsonBody = JsonSerializer.Serialize(parameters, JsonOptions);
                requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                contentType = "application/json";
                contentMd5 = ComputeMD5Hash(jsonBody);
            }
            else
            {
                // Multipart content
                var multipartContent = new MultipartFormDataContent();
                
                // Add parameters
                if (parameters != null)
                {
                    foreach (var kvp in parameters)
                    {
                        multipartContent.Add(new StringContent(kvp.Value.ToString()!), kvp.Key);
                    }
                }
            
                // Add files
                foreach (var kvp in files)
                {
                    var filePath = kvp.Value;
                    var fileName = Path.GetFileName(filePath);
                    var fileStream = File.OpenRead(filePath);
                    var streamContent = new StreamContent(fileStream);
                    multipartContent.Add(streamContent, kvp.Key, fileName);
                }

                requestContent = multipartContent;
                contentType = "multipart/form-data";
            }
        }

        // Build request - Always use POST
        var url = _baseUrl + path;
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = requestContent
        };

        // Set headers
        var dateHeader = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
        request.Headers.TryAddWithoutValidation("X-HTTP-Method-Override", method);
        request.Headers.TryAddWithoutValidation("Date", dateHeader);
        request.Headers.TryAddWithoutValidation("X-Lara-SDK-Name", "lara-dotnet");
        request.Headers.TryAddWithoutValidation("X-Lara-SDK-Version", SdkVersion.Version);
        
        if (contentMd5 != null)
        {
            var added = request.Headers.TryAddWithoutValidation("Content-MD5", contentMd5);
            if (!added && request.Content != null)
            {
                // Some headers need to be added to Content.Headers instead
                request.Content.Headers.TryAddWithoutValidation("Content-MD5", contentMd5);
            }
        }
        if (contentType != null)
            request.Headers.TryAddWithoutValidation("Content-Type", contentType);

        // Add extra headers
        foreach (var kvp in _extraHeaders)
            request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);

        // Add custom headers
        if (headers != null)
        {
            foreach (var kvp in headers)
                request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        }

        // Sign request - use the actual content type that will be sent
        var actualContentType = request.Content?.Headers?.ContentType?.ToString() ?? contentType ?? "";
        var signature = Sign(method, path, contentMd5 ?? "", actualContentType, dateHeader);
        request.Headers.TryAddWithoutValidation("Authorization", $"Lara {_accessKeyId}:{signature}");
        


        // Send request
        var response = await _httpClient.SendAsync(request);
        var mediaType = response.Content.Headers.ContentType?.MediaType;

        if (response.IsSuccessStatusCode)
        {
            if (string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase))
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<JsonElement>(responseBody, JsonOptions);
                return new ClientResponse((int)response.StatusCode, content, JsonOptions);
            }
            else
            {
                var rawBytes = await response.Content.ReadAsByteArrayAsync();
                return new ClientResponse((int)response.StatusCode, rawBytes, mediaType, JsonOptions);
            }
        }
        else
        {
            // Parse error response (assume JSON; if not JSON, include raw text)
            var responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                var errorContent = JsonSerializer.Deserialize<JsonElement>(responseBody, JsonOptions);
                if (errorContent.TryGetProperty("error", out var errorElement))
                {
                    var errorType = errorElement.TryGetProperty("type", out var typeElement) ? typeElement.GetString() : "UnknownError";
                    var errorMessage = errorElement.TryGetProperty("message", out var messageElement) ? messageElement.GetString() : "An unknown error occurred";
                    throw new LaraApiException((int)response.StatusCode, errorType ?? "UnknownError", errorMessage ?? "An unknown error occurred");
                }
                else
                {
                    throw new LaraApiException((int)response.StatusCode, "UnknownError", responseBody);
                }
            }
            catch (JsonException)
            {
                throw new LaraApiException((int)response.StatusCode, "ParseError", $"Failed to parse error response: {responseBody}");
            }
        }
    }


    /// Filters out null values from dictionaries.
    private static Dictionary<string, T>? Prune<T>(Dictionary<string, T>? dictionary)
    {
        if (dictionary == null) return null;
        
        var filtered = dictionary.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return filtered.Count == 0 ? null : filtered;
    }

    /// Normalizes the API path to start with a forward slash.
    private static string NormalizePath(string path)
    {
        return path.StartsWith("/") ? path : "/" + path;
    }

    /// Generates the current date string in HTTP header format.
    private static string GetHttpDate()
    {
        return DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
    }

    /// Signs the request using HMAC-SHA256
    private string Sign(string method, string path, string contentMd5, string contentType, string date)
    {
        // Handle content type separator (remove charset info)
        var separator = contentType.IndexOf(';');
        if (separator > 0)
            contentType = contentType.Substring(0, separator).Trim();

        var challenge = $"{method}\n{path}\n{contentMd5}\n{contentType}\n{date}";

        using var hmac = new HMACSHA256(_signingKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(challenge));
        return Convert.ToBase64String(hash);
    }

    /// Computes MD5 hash for content. Returns uppercase hex format to match Java implementation.
    private static string ComputeMD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash); // Returns uppercase hex
    }
}

 