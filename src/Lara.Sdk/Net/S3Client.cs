using System.Text;

namespace Lara.Sdk;

/// S3 client for handling file uploads and downloads
internal class S3Client
{
    private readonly HttpClient _httpClient;

    public S3Client()
    {
        _httpClient = new HttpClient();
    }

    /// Uploads a file to S3 using pre-signed URL and form fields
    public async Task UploadAsync(string uploadUrl, Dictionary<string, string> fields, string filePath)
    {
        using var multipartContent = new MultipartFormDataContent();

        // Add all form fields first
        foreach (var field in fields)
        {
            multipartContent.Add(new StringContent(field.Value), field.Key);
        }

        var fileName = Path.GetFileName(filePath);
        var fileStream = File.OpenRead(filePath);
        
        // Add the file content
        var streamContent = new StreamContent(fileStream);
        // streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        multipartContent.Add(streamContent, "file", fileName);

        var response = await _httpClient.PostAsync(uploadUrl, multipartContent);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new S3Exception($"S3 upload failed with status {response.StatusCode}: {errorContent}");
        }
    }

    /// Downloads a file from S3 using a pre-signed URL
    public async Task<Stream> DownloadAsync(string downloadUrl)
    {
        var response = await _httpClient.GetAsync(downloadUrl);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new S3Exception($"S3 download failed with status {response.StatusCode}: {errorContent}");
        }

        return await response.Content.ReadAsStreamAsync();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

/// Response structure for S3 upload URL requests
internal class S3UploadResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("fields")]
    public Dictionary<string, string> Fields { get; set; } = new();
    
    public string S3Key => Fields.TryGetValue("key", out var key) ? key : string.Empty;
}

/// Response structure for S3 download URL requests
internal class DownloadUrlResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}