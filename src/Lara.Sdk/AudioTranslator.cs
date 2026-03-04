using Lara.Sdk.Models;

namespace Lara.Sdk;

/// <summary>
/// Provides high-level operations to translate audio files using Lara services.
/// Handles upload to S3, creation of translation jobs, polling for status, and final download.
/// </summary>
public class AudioTranslator
{
    private readonly LaraClient _client;
    private readonly S3Client _s3Client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioTranslator"/> class.
    /// </summary>
    /// <param name="client">The Lara API client used to communicate with backend services.</param>
    internal AudioTranslator(LaraClient client)
    {
        _client = client;
        _s3Client = new S3Client();
    }

    /// <summary>
    /// Uploads an audio file to S3 and creates a translation job.
    /// </summary>
    /// <param name="filePath">The local path to the audio file to upload.</param>
    /// <param name="source">Optional source language code; if null or empty, the language may be auto-detected.</param>
    /// <param name="target">Target language code for translation.</param>
    /// <param name="options">Optional upload and translation options (style, adapt-to, glossaries, tracing).</param>
    /// <returns>The created <see cref="Audio"/> resource containing job status and metadata.</returns>
    /// <exception cref="LaraApiException">Thrown when the S3 upload URL is invalid.</exception>
    public async Task<Audio> Upload(
        string filePath,
        string? source,
        string target,
        AudioUploadOptions? options = null)
    {
        var fileName = Path.GetFileName(filePath);
        
        // Step 1: Get S3 upload URL
        var uploadUrlParams = new Dictionary<string, object>
        {
            ["filename"] = fileName
        };
    
        var uploadUrlResponse = await _client.Get<S3UploadResponse>("/v2/audio/upload-url", uploadUrlParams);

        if (string.IsNullOrEmpty(uploadUrlResponse.Url))
        {
            throw new LaraApiException(500, "InvalidResponse", "S3 upload URL is empty or null");
        }
        
        // Step 2: Upload file to S3
        await _s3Client.UploadAsync(uploadUrlResponse.Url, uploadUrlResponse.Fields, filePath);
        
        // Step 3: Create audio with S3 key
        var createParams = new Dictionary<string, object>
        {
            ["s3key"] = uploadUrlResponse.S3Key,
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
    
        return await _client.Post<Audio>("/v2/audio/translate", createParams, null, headers);
    }
    
    /// <summary>
    /// Retrieves the current status of an audio translation job.
    /// </summary>
    /// <param name="id">The unique identifier of the audio job.</param>
    /// <returns>An <see cref="Audio"/> instance including the latest status and metadata.</returns>
    public async Task<Audio> Status(string id)
    {
        return await _client.Get<Audio>($"/v2/audio/{id}");
    }

    /// <summary>
    /// Downloads the translated audio stream for a completed job.
    /// </summary>
    /// <param name="id">The unique identifier of the audio job.</param>
    /// <returns>A <see cref="Stream"/> containing the translated audio content.</returns>
    /// <exception cref="LaraApiException">Thrown when the download URL is invalid.</exception>
    public async Task<Stream> Download(string id)
    {
        var downloadData = await _client.Get<DownloadUrlResponse>($"/v2/audio/{id}/download-url");
    
        if (string.IsNullOrEmpty(downloadData.Url))
        {
            throw new LaraApiException(500, "InvalidResponse", "Download URL is empty or null");
        }
    
        return await _s3Client.DownloadAsync(downloadData.Url);
    }

    /// <summary>
    /// Translates an audio file end-to-end: uploads, creates the job, polls until completion, and downloads the result.
    /// </summary>
    /// <param name="filePath">The local path to the audio file to translate.</param>
    /// <param name="source">Optional source language code; if null or empty, the language may be auto-detected.</param>
    /// <param name="target">Target language code for translation.</param>
    /// <param name="options">Optional upload and translation options.</param>
    /// <returns>A <see cref="Stream"/> with the translated audio content.</returns>
    /// <exception cref="LaraApiException">Thrown when the translation fails or the job ends in error.</exception>
    /// <exception cref="LaraTimeoutException">Thrown when waiting for completion exceeds the maximum wait time.</exception>
    public async Task<Stream> Translate(
        string filePath,
        string? source,
        string target,
        AudioUploadOptions? options = null)
    {
        var audio = await Upload(filePath, source, target, options);
        audio = await PollAudioUntilCompleted(audio);
        
        if (audio.Status == AudioStatus.Error)
        {
            var errorMessage = audio.ErrorReason ?? "Translation failed";
            throw new LaraApiException(500, "AudioError", errorMessage);
        }

        return await Download(audio.Id);
    }
    
    /// <summary>
    /// Polls the audio translation job status until it is either translated or an error occurs.
    /// </summary>
    /// <param name="audio">The audio job to poll.</param>
    /// <returns>The final <see cref="Audio"/> state (Translated or Error).</returns>
    /// <exception cref="LaraTimeoutException">Thrown if the operation exceeds the maximum wait time.</exception>
    private async Task<Audio> PollAudioUntilCompleted(Audio audio)
    {
        const int pollingIntervalMs = 2000;
        const int maxWaitTimeMs = 15 * 60 * 1000; // 15 minutes
    
        var start = DateTime.UtcNow;
        var current = audio;
    
        while (current.Status != AudioStatus.Translated && current.Status != AudioStatus.Error)
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