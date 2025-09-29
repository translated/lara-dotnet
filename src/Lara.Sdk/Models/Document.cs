using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents the status of a document translation operation.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentStatus
{
    /// Document has just been created.
    [JsonPropertyName("initialized")]
    Initialized,

    /// Document is being analyzed for language detection and character count.
    [JsonPropertyName("analyzing")]
    Analyzing,

    /// Document is paused after analysis, needs user confirmation.
    [JsonPropertyName("paused")]
    Paused,

    /// Document is ready to be translated.
    [JsonPropertyName("ready")]
    Ready,

    /// Document is currently being translated.
    [JsonPropertyName("translating")]
    Translating,

    /// Document has been successfully translated.
    [JsonPropertyName("translated")]
    Translated,

    /// An error occurred during document processing.
    [JsonPropertyName("error")]
    Error
}

/// Represents options for restarting document translation.
public class DocumentTranslateOptions
{
    /// Gets or sets the memory IDs to adapt the translation to.
    [JsonPropertyName("adapt_to")]
    public string[]? AdaptTo { get; set; }
    
    /// Gets or sets the glossary IDs to use for translation.
    [JsonPropertyName("glossaries")]
    public string[]? Glossaries { get; set; }

    /// Gets or sets the translation style to use.
    [JsonPropertyName("style")]
    public TranslationStyle? Style { get; set; }

    /// Gets or sets the output format for the downloaded document.
    [JsonPropertyName("output_format")]
    public string? OutputFormat { get; set; }

    /// Gets or sets a value indicating whether to disable tracing for this request.
    [JsonPropertyName("no_trace")]
    public bool NoTrace { get; set; }

    /// Gets or sets the password for password-protected PDF documents.
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    
    /// Gets or sets the extraction parameters for the document.
    [JsonPropertyName("extraction_params")]
    public DocumentExtractionParams? ExtractionParams { get; set; }
}

/// Represents options for document download operations.
public class DocumentDownloadOptions
{
    /// Gets or sets the output format for the downloaded document.
    [JsonPropertyName("output_format")]
    public string? OutputFormat { get; set; }
}

/// Represents options for document upload operations.
public class DocumentUploadOptions
{
    [JsonPropertyName("adapt_to")]
    public string[]? AdaptTo { get; set; }
    
    [JsonPropertyName("glossaries")]
    public string[]? Glossaries { get; set; }
    
    /// Gets or sets a value indicating whether to disable tracing for this request.
    [JsonPropertyName("no_trace")]
    public bool NoTrace { get; set; }
    
    [JsonPropertyName("style")]
    public TranslationStyle? Style { get; set; }
    
    /// Gets or sets the password for password-protected PDF documents.
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    
    /// Gets or sets the extraction parameters for the document.
    [JsonPropertyName("extraction_params")]
    public DocumentExtractionParams? ExtractionParams { get; set; }
}

/// Represents document options.
public class DocumentOptions
{
    [JsonPropertyName("adapt_to")]
    public string[]? AdaptTo { get; set; }
    
    [JsonPropertyName("style")]
    public TranslationStyle? Style { get; set; }
}

/// Represents a document translation operation.
public class Document
{
    /// Gets the unique identifier of the document.
    [JsonPropertyName("id")]
    public string Id { get; }

    /// Gets the current status of the document translation.
    [JsonPropertyName("status")]
    public DocumentStatus Status { get; }

    /// Gets the number of characters that have been translated.
    [JsonPropertyName("translated_chars")]
    public int TranslatedChars { get; }

    /// Gets the total number of characters in the document.
    [JsonPropertyName("total_chars")]
    public int TotalChars { get; }

    /// Gets the filename of the document.
    [JsonPropertyName("filename")]
    public string Filename { get; }

    /// Gets the source language of the document, if detected.
    [JsonPropertyName("source")]
    public string? Source { get; }

    /// Gets the target language for translation.
    [JsonPropertyName("target")]
    public string Target { get; }

    /// Gets the options used for this document translation.
    [JsonPropertyName("options")]
    public DocumentOptions? Options { get; }

    /// Gets the error reason if the document status is Error.
    [JsonPropertyName("error_reason")]
    public string? ErrorReason { get; }

    /// Gets the creation timestamp of the document.
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; }

    /// Gets the last update timestamp of the document.
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; }

    /// Initializes a new instance of the Document class.
    public Document(string id, DocumentStatus status, string filename, string target, 
        string createdAt, string updatedAt, int translatedChars = 0, int totalChars = 0, 
        string? source = null, DocumentOptions? options = null, string? errorReason = null)
    {
        Id = id;
        Status = status;
        TranslatedChars = translatedChars;
        TotalChars = totalChars;
        Filename = filename;
        Source = source;
        Target = target;
        Options = options;
        ErrorReason = errorReason;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    // getter methods
    public string GetId() => Id;
    public DocumentStatus GetStatus() => Status;
    public int GetTranslatedChars() => TranslatedChars;
    public int GetTotalChars() => TotalChars;
    public string GetFilename() => Filename;
    public string? GetSource() => Source;
    public string GetTarget() => Target;
    public DocumentOptions? GetOptions() => Options;
    public string? GetErrorReason() => ErrorReason;
    public string GetCreatedAt() => CreatedAt;
    public string GetUpdatedAt() => UpdatedAt;
}

/// Represents the available translation styles.
public enum TranslationStyle
{
    /// Faithful translation that stays close to the original text.
    [JsonPropertyName("faithful")]
    Faithful,

    /// Fluid translation that prioritizes natural language flow.
    [JsonPropertyName("fluid")]
    Fluid,

    /// Creative translation that allows for more interpretative freedom.
    [JsonPropertyName("creative")]
    Creative
} 


public abstract class DocumentExtractionParams { }

public class DocxExtractionParams : DocumentExtractionParams
{
    [JsonPropertyName("extract_comments")]
    public bool? ExtractComments { get; set; }

    [JsonPropertyName("accept_revisions")]
    public bool? AcceptRevisions { get; set; }
}

// Future parameter types:
// public class PdfExtractionParams : ExtractionParams { ... }
// public class ExcelExtractionParams : ExtractionParams { ... }