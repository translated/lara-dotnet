using System.Text.Json.Serialization;
using Lara.Sdk.Models.Authentication;

namespace Lara.Sdk;

/// The main Lara SDK translator class providing access to all translation services.
public class Translator
{
    /// The underlying HTTP client for API communication.
    protected readonly LaraClient Client;

    /// Gets the memories service for managing translation memories.
    public readonly Memories Memories;

    /// Gets the documents service for managing document translations.
    public readonly Documents Documents;

    /// Gets the glossaries service for managing translation glossaries.
    public readonly Glossaries Glossaries;

    /// Gets the images service for managing image translations.
    public readonly ImageTranslator Images;

    /// Get the audio service for managing audio translations.
    public readonly AudioTranslator Audio;

    /// Initializes a new instance of the Translator class with default options.
    /// <param name="accessKey">The API credentials.</param>
    public Translator(AccessKey accessKey) : this(accessKey, new ClientOptions())
    {
    }

    /// Initializes a new instance of the Translator class
    public Translator(AccessKey accessKey, ClientOptions options)
    {
        Client = new LaraClient(accessKey, options);
        Memories = new Memories(Client);
        Documents = new Documents(Client);
        Glossaries = new Glossaries(Client);
        Images = new ImageTranslator(Client);
        Audio = new AudioTranslator(Client);
    }

    /// Gets the list of supported languages
    public async Task<List<string>> Languages()
    {
        var response = await Client.Get<List<string>>("/v2/languages");
        return response;
    }
    
    /// Translates text with options. If source is null, the API will attempt to auto-detect the source language.
    [Obsolete("Use Translate<T> directly. This overload will be removed in v2.0.")]
    public async Task<TextResult<string>> Translate(string text, string? source, string target, TranslateOptions? options = null)
    {
        return await Translate<string>(text, source, target, options);
    }

    /// Translates multiple texts with options. If source is null, the API will attempt to auto-detect the source language.
    [Obsolete("Use Translate<T> directly. This overload will be removed in v2.0.")]
    public async Task<TextResult<List<string>>> Translate(List<string> texts, string? source, string target, TranslateOptions? options = null)
    {
        return await Translate<List<string>>(texts, source, target, options);
    }

    /// Translates multiple texts with options. If source is null, the API will attempt to auto-detect the source language.
    [Obsolete("Use Translate<T> directly. This overload will be removed in v2.0.")]
    public async Task<TextResult<string[]>> Translate(string[] texts, string? source, string target, TranslateOptions? options = null)
    {
        return await Translate<string[]>(texts, source, target, options);
    }

    /// Translates text blocks with options. If source is null, the API will attempt to auto-detect the source language.
    [Obsolete("Use Translate<T> directly. This overload will be removed in v2.0.")]
    public async Task<TextResult<List<TextBlock>>> Translate(List<TextBlock> textBlocks, string? source, string target, TranslateOptions? options = null)
    {
        return await Translate<List<TextBlock>>(textBlocks, source, target, options);
    }

    /// Translates text blocks with options. If source is null, the API will attempt to auto-detect the source language.
    [Obsolete("Use Translate<T> directly. This overload will be removed in v2.0.")]
    public async Task<TextResult<TextBlock[]>> Translate(TextBlock[] textBlocks, string? source, string target, TranslateOptions? options = null)
    {
        return await Translate<TextBlock[]>(textBlocks, source, target, options);
    }

    /// Core translation method that handles any input type.
    /// <param name="text">The text content to translate.</param>
    /// <param name="source">The source language code. If null, the API will attempt auto-detection.</param>
    /// <param name="target">The target language code.</param>
    /// <param name="options">Optional translation settings.</param>
    public async Task<TextResult<T>> Translate<T>(T text, string? source, string target, TranslateOptions? options = null)
    {
        return await Translate(text, source, target, options, null);
    }

    /// Core translation method that handles any input type with callback support.
    /// <param name="text">The text content to translate.</param>
    /// <param name="source">The source language code. If null, the API will attempt auto-detection.</param>
    /// <param name="target">The target language code.</param>
    /// <param name="options">Optional translation settings.</param>
    /// <param name="callback">Callback for partial results when reasoning is enabled.</param>
    public async Task<TextResult<T>> Translate<T>(T text, string? source, string target, TranslateOptions? options, Action<TextResult<T>>? callback)
    {
        var parameters = options?.ToParams() ?? new HttpParams<object>();
        parameters.Set("source", source)
            .Set("target", target)
            .Set("q", text);

        var headers = new Dictionary<string, string>();

        if (options?.GetHeaders() != null)
        {
            foreach (var header in options.GetHeaders()!)
            {
                if (header.Value != null)
                {
                    headers[header.Key] = header.Value.ToString()!;
                }
            }
        }

        if (options?.GetNoTrace() == true)
        {
            headers["X-No-Trace"] = "true";
        }

        TextResult<T>? lastResult = null;

        await foreach (var partial in Client.PostAndGetStream<TextResult<T>>("/translate", parameters.Build(), headers.Count > 0 ? headers : null))
        {
            if (options?.Reasoning == true && callback != null)
            {
                callback(partial);
            }
            lastResult = partial;
        }

        if (lastResult == null)
        {
            throw new InvalidOperationException("No translation result received.");
        }

        return lastResult;
    }
    
    /// Detects the language of the given text.
    /// <param name="text">The text content to detect</param>
    /// <param name="hint">An hint for the detection</param>
    /// <param name="passlist">A list of acceptable languages</param>
    public async Task<DetectResult> Detect<T>(T text, string? hint = null, IEnumerable<string>? passlist = null)
    {
        var parameters = new HttpParams<object>()
            .Set("q", text);
        if (hint != null)
        {
            parameters.Set("hint", hint);
        }
        if (passlist != null && passlist.Any())
        {
            parameters.Set("passlist", passlist);
        }
    
        var response = await Client.Post<DetectResult>("/v2/detect", parameters.Build());
        return response;
    }
}