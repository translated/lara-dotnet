using System.Text.Json;

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
    /// Translates an image.
    /// The available models for text rendering are:
    /// <list type="bullet">
    /// <item><c>overlay</c></item>
    /// <item><c>inpainting</c></item>
    /// <item><c>generative</c></item>
    /// <item><c>generative_fast</c></item>
    /// </list>
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
            .Set("model", SerializeModel(options))
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
            .Set("verbose", options?.Verbose.ToString().ToLowerInvariant())
            .Set("include_layout", options?.IncludeLayout.ToString().ToLowerInvariant());
    
        var headers = new Dictionary<string, string>();
        if (options?.NoTrace == true)
        {
            headers["X-No-Trace"] = "true";
        }
    
        return await _client.Post<ImageTextResult>("/v2/images/translate-text", parameters.Build(), files, headers);
    }

    /// <summary>
    /// Renders supplied translations onto the original image without translating again.
    /// Overlay and inpainting require BBox, LinesBBoxes, TextInfo, and Alignment on every
    /// paragraph. Generative models accept text-only paragraphs or complete layout.
    /// An omitted model defaults to generative_fast. The caller must dispose the returned stream.
    /// </summary>
    public async Task<Stream> RenderTranslated(
        string imagePath,
        string? source,
        string target,
        IEnumerable<ImageParagraph> paragraphs,
        ImageTranslationModel? model = null,
        bool noTrace = false)
    {
        var renderParagraphs = paragraphs.Select(paragraph =>
        {
            var fields = new HttpParams<object>();
            return fields.Set("text", paragraph.Text)
                .Set("translation", paragraph.Translation)
                .Set("bbox", paragraph.BBox)
                .Set("lines_bboxes", paragraph.LinesBBoxes)
                .Set("text_info", paragraph.TextInfo)
                .Set("alignment", paragraph.Alignment)
                .Build();
        }).ToArray();

        await using var fileStream = File.OpenRead(imagePath);
        var files = new Dictionary<string, Stream> { ["image"] = fileStream };
        var parameters = new HttpParams<object>();
        parameters.Set("target", target)
            .Set("source", source)
            .Set("paragraphs", JsonSerializer.Serialize(renderParagraphs))
            .Set("model", SerializeRenderingModel(model));
        var headers = new Dictionary<string, string>();
        if (noTrace)
        {
            headers["X-No-Trace"] = "true";
        }
        return await _client.Post<Stream>("/v2/images/render-translated", parameters.Build(), files, headers);
    }

    private static string? SerializeRenderingModel(ImageTranslationModel? model) => model switch
    {
        ImageTranslationModel.Overlay => "overlay",
        ImageTranslationModel.Inpainting => "inpainting",
        ImageTranslationModel.Generative => "generative",
        ImageTranslationModel.GenerativeFast => "generative_fast",
        _ => null
    };

    private static string? SerializeModel(ImageTranslateOptions? options)
    {
        if (options?.Model is { } model)
        {
            return SerializeRenderingModel(model);
        }

#pragma warning disable CS0618
        return options?.TextRemoval?.ToString().ToLowerInvariant();
#pragma warning restore CS0618
    }
}
