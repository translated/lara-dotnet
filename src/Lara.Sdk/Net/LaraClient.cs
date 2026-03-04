using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using Lara.Sdk.Models.Authentication;
using System.Net.Http.Headers;

namespace Lara.Sdk;

/// Main HTTP client for the Lara API providing HMAC authentication
public class LaraClient
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private AuthToken? _authToken;
    private readonly AccessKey? _accessKey;

    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _extraHeaders = new();
    
    public LaraClient(AccessKey accessKey, ClientOptions? options): this(options)
    {
        _accessKey = accessKey;
    }

    public LaraClient(AuthToken authToken, ClientOptions? options) : this(options)
    {
        _authToken = authToken;
    }

    private LaraClient(ClientOptions? options)
    {
        options ??= new ClientOptions();
        
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(options.ServerUrl)
        };
        
        _httpClient.DefaultRequestHeaders.Add("X-Lara-SDK-Name", "lara-dotnet");
        _httpClient.DefaultRequestHeaders.Add("X-Lara-SDK-Version", SdkVersion.Version);
    }
    
    /// Sets an extra header to be included with all requests.
    public void SetExtraHeader(string name, string value)
    {
        _extraHeaders[name] = value;
    }
    
    public async Task<T> Get<T>(string path, Dictionary<string, object>? queryParams = null, Dictionary<string, string>? headers = null)
    {
        if (queryParams?.Count > 0)
        {
            var queryString = string.Join("&", queryParams.Select(kvp => 
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value?.ToString() ?? "")}"));
            path = $"{path}?{queryString}";
        }
        return await Request<T>(HttpMethod.Get, path, null, null, headers);
    }
    
    public async Task<T> Post<T>(string path, Dictionary<string, object>? parameters = null)
    {
        return await Request<T>(HttpMethod.Post, path, parameters, null, null);
    }
    public async Task<T> Post<T>(string path, Dictionary<string, object>? parameters = null,
        Dictionary<string, string>? headers = null)
    {
        return await Request<T>(HttpMethod.Post, path, parameters, null, headers);
    }
    public async Task<T> Post<T>(
        string path, 
        Dictionary<string, object>? parameters = null,
        Dictionary<string, Stream>? files = null,
        Dictionary<string, string>? headers = null
    )
    {
        return await Request<T>(HttpMethod.Post, path, parameters, files, headers);
    }
    
    public async Task<T> Put<T>(
        string path, 
        Dictionary<string, object>? parameters = null,
        Dictionary<string, string>? headers = null
        )
    {
        return await Request<T>(HttpMethod.Put, path, parameters, null, headers);
    }
    
    public async Task<T> Delete<T>(string path, Dictionary<string, object>? parameters = null,
        Dictionary<string, string>? headers = null)
    {
        return await Request<T>(HttpMethod.Delete, path, parameters, null, headers);
    }

    public IAsyncEnumerable<T> PostAndGetStream<T>(
        string path,
        Dictionary<string, object>? parameters = null,
        Dictionary<string, string>? headers = null
    ) => RequestStream<T>(HttpMethod.Post, path, parameters, headers);

    private async Task<T> Request<T>(
        HttpMethod method,
        string path,
        Dictionary<string, object>? parameters = null,
        Dictionary<string, Stream>? files = null,
        Dictionary<string, string>? headers = null,
        bool isRetry = false
    )
    {
        var request = new HttpRequestMessage(method, path);
        SetDateHeader(request);
        
        // Add extra headers
        foreach (var kvp in _extraHeaders)
            request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        
        if (headers != null)
        {
            foreach (var kvp in headers)
            {
                request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }
        }
        
        // Authentication
        var token = await Authenticate();
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        // Body
        if ((parameters?.Count > 0 || files?.Count > 0) && method != HttpMethod.Get)
        {
            if (files?.Count > 0)
            {
                var multipartContent = new MultipartFormDataContent();

                if (parameters?.Count > 0)
                {
                    foreach (var kvp in parameters)
                    {
                        string valueString;
                        if (kvp.Value is IEnumerable<string> stringArray)
                        {
                            valueString = JsonSerializer.Serialize(stringArray, _jsonOptions);
                        }
                        else
                        {
                            valueString = kvp.Value?.ToString() ?? string.Empty;
                        }
                        var stringContent = new StringContent(valueString, Encoding.UTF8);
                        multipartContent.Add(stringContent, kvp.Key);
                    }
                }
                
                foreach (var kvp in files)
                {
                    var streamContent = new StreamContent(kvp.Value);
                    var (fileName, contentType) = GetFileNameAndContentType(kvp.Value);
                    if (!string.IsNullOrWhiteSpace(contentType))
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    multipartContent.Add(streamContent, kvp.Key, fileName);
                }

                request.Content = multipartContent;
            }
            else
            {
                var jsonBody = JsonSerializer.Serialize(parameters, _jsonOptions);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }
        }

        var response = await _httpClient.SendAsync(request);
        
        // Intercept 429 with "jwt expired" message to refresh token
        if (!isRetry && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            if (errorBody.Contains("jwt expired"))
            {
                await Refresh();
                return await Request<T>(method, path, parameters, files, headers, true);
            }
        }

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new LaraApiException((int)response.StatusCode, "ApiError", responseBody);
        }

        if (typeof(T) == typeof(Stream))
        {
            var stream = await response.Content.ReadAsStreamAsync();
            return (T)(object)stream;
        }
        
        if (response.Content.Headers.ContentLength == 0)
            return default!;

        var content = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(content, _jsonOptions)!;

    }

    private async IAsyncEnumerable<T> RequestStream<T>(
        HttpMethod method,
        string path,
        Dictionary<string, object>? parameters = null,
        Dictionary<string, string>? headers = null,
        bool isRetry = false)
    {
        var request = new HttpRequestMessage(method, path);
        SetDateHeader(request);

        foreach (var kvp in _extraHeaders)
            request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        if (headers != null)
            foreach (var kvp in headers)
                request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await Authenticate());

        if (parameters?.Count > 0 && method != HttpMethod.Get)
            request.Content = new StringContent(JsonSerializer.Serialize(parameters, _jsonOptions), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (!isRetry && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await Refresh();
            await foreach (var item in RequestStream<T>(method, path, parameters, headers, true))
                yield return item;
            yield break;
        }

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new LaraApiException((int)response.StatusCode, "ApiError", responseBody);
        }

        await foreach (var result in ProcessStreamResponse<T>(response))
            yield return result;
    }

    private async IAsyncEnumerable<T> ProcessStreamResponse<T>(HttpResponseMessage response)
    {
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (TryDeserializeStreamLine<T>(line, out var result))
                yield return result!;
        }
    }

    private bool TryDeserializeStreamLine<T>(string json, out T? result)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Response may be wrapped in {data: {...}} or be direct {...}
            var targetJson = root.TryGetProperty("data", out var dataElement)
                ? dataElement.GetRawText()
                : json;

            result = JsonSerializer.Deserialize<T>(targetJson, _jsonOptions);
            return result != null;
        }
        catch (JsonException)
        {
            result = default;
            return false;
        }
    }

    private async Task<string> Authenticate()
    {
        if (_authToken != null)
            return _authToken.Token;

        if (_accessKey == null)
            throw new InvalidOperationException("No AccessKey or Credentials available.");

        var path = "/v2/auth";
        var payload = new { id = _accessKey.Id };
        var jsonBody = JsonSerializer.Serialize(payload, _jsonOptions);
        var contentMd5 = ComputeMD5Hash(jsonBody);
        var contentType = "application/json";
        var dateHeader = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, contentType)
        };
        request.Headers.TryAddWithoutValidation("Date", dateHeader);
        request.Content.Headers.TryAddWithoutValidation("Content-MD5", contentMd5);

        // Sign request with HMAC-SHA256
        var signature = Sign(_accessKey.Secret, "POST", path, contentMd5, contentType, dateHeader);
        request.Headers.TryAddWithoutValidation("Authorization", $"Lara:{signature}");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var authenticationResult = JsonSerializer.Deserialize<AuthenticationResult>(content, _jsonOptions)!;

        if (authenticationResult == null || string.IsNullOrEmpty(authenticationResult.Token))
            throw new Exception("Login Failed");

        if (!response.Headers.TryGetValues("x-lara-refresh-token", out var refreshHeaderValues))
            throw new Exception("Login Failed: missing x-lara-refresh-token header.");

        var refreshToken = refreshHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(refreshToken))
            throw new Exception("Login Failed: empty x-lara-refresh-token header.");

        _authToken = new AuthToken(authenticationResult.Token, refreshToken);
        return _authToken.Token;
    }
    
    public async Task<string> Refresh()
    {
        if (_authToken == null || string.IsNullOrEmpty(_authToken.RefreshToken))
            throw new InvalidOperationException("RefreshToken not available.");

        var request = new HttpRequestMessage(HttpMethod.Post, "/v2/auth/refresh");
        SetDateHeader(request);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken.RefreshToken);
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var authenticationResult = JsonSerializer.Deserialize<AuthenticationResult>(content, _jsonOptions)!;

        if (authenticationResult == null || string.IsNullOrEmpty(authenticationResult.Token))
            throw new Exception("Failed to refresh auth token: missing token in body.");

        if (!response.Headers.TryGetValues("x-lara-refresh-token", out var refreshHeaderValues))
            throw new Exception("Failed to refresh auth token: missing x-lara-refresh-token header.");

        var refreshToken = refreshHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(refreshToken))
            throw new Exception("Failed to refresh auth token: empty x-lara-refresh-token header.");

        _authToken = new AuthToken(authenticationResult.Token, refreshToken);
        return _authToken.Token;
    }
    
    private static void SetDateHeader(HttpRequestMessage request)
    {
        request.Headers.Date = DateTimeOffset.UtcNow;
    }

    /// Signs the request using HMAC-SHA256
    private static string Sign(string secret, string method, string path, string contentMd5, string contentType, string date)
    {
        // Handle content type separator (remove charset info)
        var separator = contentType.IndexOf(';');
        if (separator > 0)
            contentType = contentType.Substring(0, separator).Trim();

        var challenge = $"{method}\n{path}\n{contentMd5}\n{contentType}\n{date}";

        var signingKey = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(signingKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(challenge));
        return Convert.ToBase64String(hash);
    }

    /// Computes MD5 hash for content. Returns uppercase hex format.
    private static string ComputeMD5Hash(string input)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash);
    }

    private static (string FileName, string? ContentType) GetFileNameAndContentType(Stream stream)
    {
        if (stream is FileStream fileStream)
        {
            var fileName = Path.GetFileName(fileStream.Name);
            var contentType = GetContentTypeFromExtension(Path.GetExtension(fileName)) ?? "application/octet-stream";
            return (fileName, contentType);
        }

        return ("image.jpg", "image/jpeg");
    }

    private static string? GetContentTypeFromExtension(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".tif" => "image/tiff",
            ".tiff" => "image/tiff",
            ".webp" => "image/webp",
            ".avif" => "image/avif",
            ".heic" => "image/heic",
            ".heif" => "image/heic",
            _ => null
        };
    }
}