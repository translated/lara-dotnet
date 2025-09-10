namespace Lara;

/// Document management service
public class Documents
{
    private readonly LaraClient _client;
    private readonly S3Client _s3Client;

    /// Initializes a new instance of the Documents class.
    internal Documents(LaraClient client)
    {
        _client = client;
        _s3Client = new S3Client();
    }

    /// Uploads a file and creates a document translation job.
    public async Task<Document> Upload(
        string filePath, 
        string? source, 
        string target, 
        DocumentUploadOptions? options = null
        )
    {
        var fileName = Path.GetFileName(filePath);
        
        // Step 1: Get S3 upload URL
        var uploadUrlParams = new Dictionary<string, object>
        {
            ["filename"] = fileName
        };

        var uploadUrlResponse = await _client.Get("/documents/upload-url", uploadUrlParams);
        
        // The API returns { "status": 200, "content": { "url": "...", "fields": {...} } }
        // We need to extract the content part
        var s3UploadData = uploadUrlResponse.AsWrapped<S3UploadResponse>();

        if (string.IsNullOrEmpty(s3UploadData.Url))
        {
            throw new LaraApiException(500, "InvalidResponse", "S3 upload URL is empty or null");
        }

        // Step 2: Upload file to S3
        await _s3Client.UploadAsync(s3UploadData.Url, s3UploadData.Fields, filePath);

        // Step 3: Create document with S3 key
        var createParams = new Dictionary<string, object>
        {
            ["s3key"] = s3UploadData.S3Key,
            ["target"] = target
        };

        if (!string.IsNullOrEmpty(source))
            createParams["source"] = source;

        if (options != null)
        {
            if (options.AdaptTo != null && options.AdaptTo.Length > 0)
                createParams["adapt_to"] = options.AdaptTo;
            if (options.Glossaries != null && options.Glossaries.Length > 0)
                createParams["glossaries"] = options.Glossaries;
            if (options.Style.HasValue)
                createParams["style"] = options.Style.Value.ToString().ToLowerInvariant();
        }

        var headers = new Dictionary<string, string>();
        if (options?.NoTrace == true)
            headers["X-No-Trace"] = "true";

        var response = await _client.Post("/documents", createParams, null, headers);
        
        // The API returns { "status": 200, "content": { Document } }
        return response.AsWrapped<Document>();
    }

    /// Checks the status of a document.
    public async Task<Document> Status(string id)
    {
        var response = await _client.Get($"/documents/{id}");
        
        // The API returns { "status": 200, "content": { Document } }
        return response.AsWrapped<Document>();
    }

    /// Downloads a translated document with options.
    public async Task<Stream> Download(
        string id, 
        DocumentDownloadOptions? options = null
        )
    {

        var parameters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(options?.OutputFormat))
            parameters["output_format"] = options.OutputFormat;

        var response = await _client.Get($"/documents/{id}/download-url", parameters);
        
        // The API returns { "status": 200, "content": { "url": "..." } }
        var downloadData = response.AsWrapped<DownloadUrlResponse>();

        if (string.IsNullOrEmpty(downloadData.Url))
        {
            throw new LaraApiException(500, "InvalidResponse", "Download URL is empty or null");
        }

        return await _s3Client.DownloadAsync(downloadData.Url);
    }

    /// Translates a file and returns an InputStream to read the translated document.
    public async Task<Stream> Translate(
        string filePath, 
        string? source, 
        string target, 
        DocumentTranslateOptions? options = null)
    {
        var uploadOptions = new DocumentUploadOptions
        {
            AdaptTo = options?.AdaptTo,
            Glossaries = options?.Glossaries,
            Style = options?.Style,
            NoTrace = options?.NoTrace ?? false
        };

        var document = await Upload(filePath, source, target, uploadOptions);
        document = await PollDocumentUntilCompleted(document);

        if (document.Status == DocumentStatus.Error)
        {
            var errorMessage = document.ErrorReason ?? "Translation failed";
            throw new LaraApiException(500, "DocumentError", errorMessage);
        }

        var downloadOptions = new DocumentDownloadOptions
        {
            OutputFormat = options?.OutputFormat
        };

        return await Download(document.Id, downloadOptions);
    }

    /// Polls document until translation is completed
    private async Task<Document> PollDocumentUntilCompleted(Document document)
    {
        const int pollingIntervalMs = 2000;
        const int maxWaitTimeMs = 15 * 60 * 1000; // 15 minutes

        var start = DateTime.UtcNow;
        var current = document;

        while (current.Status != DocumentStatus.Translated && current.Status != DocumentStatus.Error)
        {
            if ((DateTime.UtcNow - start).TotalMilliseconds > maxWaitTimeMs)
            {
                throw new LaraTimeoutException("Timeout waiting for translation to complete");
            }

            await Task.Delay(pollingIntervalMs);
            current = await Status(current.Id);
        }

        return current;
    }
}