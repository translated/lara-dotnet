namespace Lara.Sdk;

/// <summary>
/// Provides functionality for translating images and extracting/translating text from images using the Lara API.
/// </summary>
public class ImageTranslator
{
    private readonly LaraClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTranslator"/> class.
    /// </summary>
    /// <param name="client">The LaraClient instance used for API communication.</param>
    internal ImageTranslator(LaraClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Translates an image by overlaying translated text.
    /// </summary>
    /// <param name="imagePath">The path to the image file to translate.</param>
    /// <param name="source">The source language code, or null for auto-detection.</param>
    /// <param name="target">The target language code.</param>
    /// <param name="options">Optional translation options.</param>
    /// <returns>A stream containing the translated image.</returns>
    public async Task<Stream> Translate(
        string imagePath,
        string? source,
        string target,
        ImageTranslateOptions? options = null
    )
    {
        await using var fileStream = File.OpenRead(imagePath);
        var files = new Dictionary<string, Stream>() { ["image"] = fileStream };
        
        var parameters = new HttpParams<object>();
        parameters.Set("target", target)
            .Set("source", source)
            .Set("style", options?.Style?.ToString().ToLowerInvariant())
            .Set("text_removal", options?.TextRemoval?.ToString().ToLowerInvariant())
            .Set("adapt_to", options?.AdaptTo)
            .Set("glossaries", options?.Glossaries);
    
        var headers = new Dictionary<string, string>();
        if (options?.NoTrace == true)
        {
            headers["X-No-Trace"] = "true";
        }
    
        return await _client.Post<Stream>("/v2/images/translate", parameters.Build(), files, headers);
    }
    
    /// <summary>
    /// Extracts and translates text from an image, returning structured results.
    /// </summary>
    /// <param name="imagePath">The path to the image file to process.</param>
    /// <param name="source">The source language code, or null for auto-detection.</param>
    /// <param name="target">The target language code.</param>
    /// <param name="options">Optional text translation options.</param>
    /// <returns>An <see cref="ImageTextResult"/> containing the extracted and translated text.</returns>
    public async Task<ImageTextResult> TranslateText(
        string imagePath,
        string? source,
        string target,
        ImageTextTranslateOptions? options = null
    )
    {
        await using var fileStream = File.OpenRead(imagePath);
        var files = new Dictionary<string, Stream>() { ["image"] = fileStream };
        
        var parameters = new HttpParams<object>();
        parameters.Set("target", target)
            .Set("source", source)
            .Set("style", options?.Style?.ToString().ToLowerInvariant())
            .Set("adapt_to", options?.AdaptTo)
            .Set("glossaries", options?.Glossaries)
            .Set("verbose", options?.Verbose);
    
        var headers = new Dictionary<string, string>();
        if (options?.NoTrace == true)
        {
            headers["X-No-Trace"] = "true";
        }
    
        return await _client.Post<ImageTextResult>("/v2/images/translate-text", parameters.Build(), files, headers);
    }
}