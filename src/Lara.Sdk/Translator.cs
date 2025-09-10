using System.Text.Json.Serialization;

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

    /// Initializes a new instance of the Translator class with default options.
    /// <param name="credentials">The API credentials.</param>
    public Translator(Credentials credentials) : this(credentials, new ClientOptions())
    {
    }

    /// Initializes a new instance of the Translator class
    public Translator(Credentials credentials, ClientOptions options)
    {
        Client = new LaraClient(credentials, options);
        Memories = new Memories(Client);
        Documents = new Documents(Client);
        Glossaries = new Glossaries(Client);
    }

    /// Gets the list of supported languages
    public async Task<List<string>> Languages()
    {
        var response = await Client.Get("/languages");
        var languages = response.AsWrappedList<string>();
        return languages;
    }


    /// Translates text with options. If source is null, the API will attempt to auto-detect the source language.
    public async Task<TextResult> Translate(string text, string? source, string target, TranslateOptions? options = null)
    {
        return await TranslateAny(text, source, target, options);
    }

    /// Translates multiple texts with options. If source is null, the API will attempt to auto-detect the source language.
    public async Task<TextResult> Translate(List<string> texts, string? source, string target, TranslateOptions? options = null)
    {
        return await TranslateAny(texts, source, target, options);
    }

    /// Translates multiple texts with options. If source is null, the API will attempt to auto-detect the source language.
    public async Task<TextResult> Translate(string[] texts, string? source, string target, TranslateOptions? options = null)
    {
        return await TranslateAny(texts, source, target, options);
    }

    /// Translates text blocks with options. If source is null, the API will attempt to auto-detect the source language.
    public async Task<TextResult> Translate(List<TextBlock> textBlocks, string? source, string target, TranslateOptions? options = null)
    {
        return await TranslateAny(textBlocks, source, target, options);
    }

    /// Translates text blocks with options. If source is null, the API will attempt to auto-detect the source language.
    public async Task<TextResult> Translate(TextBlock[] textBlocks, string? source, string target, TranslateOptions? options = null)
    {
        return await TranslateAny(textBlocks, source, target, options);
    }

    /// Core translation method that handles any input type.
    /// <param name="text">The text content to translate.</param>
    /// <param name="source">The source language code. If null, the API will attempt auto-detection.</param>
    /// <param name="target">The target language code.</param>
    /// <param name="options">Optional translation settings.</param>
    protected async Task<TextResult> TranslateAny(object text, string? source, string target, TranslateOptions? options)
    {
        var parameters = options?.ToParams() ?? new HttpParams<object>();
        parameters            .Set("source", source)
            .Set("target", target)
            .Set("q", text);
        
        if (source != null)
        {
            parameters.Set("source", source);
        }

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

        var response = await Client.Post("/translate", parameters.Build(), null, headers.Count > 0 ? headers : null);
        
        return response.AsWrapped<TextResult>();
    }
}