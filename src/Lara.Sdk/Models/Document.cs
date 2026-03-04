using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents the status of a document translation operation.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentStatus
{
    /// Document has just been created.
    // [JsonPropertyName("initialized")]
    Initialized,

    /// Document is being analyzed for language detection and character count.
    // [JsonPropertyName("analyzing")]
    Analyzing,

    /// Document is paused after analysis, needs user confirmation.
    // [JsonPropertyName("paused")]
    Paused,

    /// Document is ready to be translated.
    // [JsonPropertyName("ready")]
    Ready,

    /// Document is currently being translated.
    // [JsonPropertyName("translating")]
    Translating,

    /// Document has been successfully translated.
    // [JsonPropertyName("translated")]
    Translated,

    /// An error occurred during document processing.
    // [JsonPropertyName("error")]
    Error
}

/// Represents options for restarting document translation.
public class DocumentTranslateOptions: DocumentUploadOptions
{
    /// Gets or sets the output format for the downloaded document.
    public string? OutputFormat { get; set;}

}

/// Represents options for document download operations.
public class DocumentDownloadOptions
{
    /// Gets or sets the output format for the downloaded document.
    public string? OutputFormat { get; set; }
}

/// Represents options for document upload operations.
public class DocumentUploadOptions : DocumentOptions
{
    public string[]? Glossaries { get; set; }
    
    /// Gets or sets a value indicating whether to disable tracing for this request.
    public bool NoTrace { get; set; }
    
    /// Gets or sets the password for password-protected PDF documents.
    public string? Password { get; set; }
    
    /// Gets or sets the extraction parameters for the document.
    public DocumentExtractionParams? ExtractionParams { get; set; }
}

/// Represents document options.
public class DocumentOptions
{
    public string[]? AdaptTo { get; set; }
    
    public TranslationStyle? Style { get; set; }
}

/// Represents a document translation operation.
public class Document
{
    /// Gets the unique identifier of the document.
    public string Id { get; }

    /// Gets the current status of the document translation.
    public DocumentStatus Status { get; }

    /// Gets the number of characters that have been translated.
    public int TranslatedChars { get; }

    /// Gets the total number of characters in the document.
    public int TotalChars { get; }

    /// Gets the filename of the document.
    public string Filename { get; }

    /// Gets the source language of the document, if detected.
    public string? Source { get; }

    /// Gets the target language for translation.
    public string Target { get; }

    /// Gets the options used for this document translation.
    public DocumentOptions? Options { get; }

    /// Gets the error reason if the document status is Error.
    public string? ErrorReason { get; }

    /// Gets the creation timestamp of the document.
    public string CreatedAt { get; }

    /// Gets the last update timestamp of the document.
    public string UpdatedAt { get; }

    /// Initializes a new instance of the Document class.
    [JsonConstructor]
    public Document(
        string id, 
        DocumentStatus status, 
        string filename, 
        string target, 
        string createdAt, 
        string updatedAt, 
        int translatedChars = 0, 
        int totalChars = 0, 
        string? source = null, 
        DocumentOptions? options = null, 
        string? errorReason = null
        )
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

    [Obsolete("Use the Id property instead. This method will be removed in a future release.")]
    public string GetId() => Id;
    [Obsolete("Use the Status property instead. This method will be removed in a future release.")]
    public DocumentStatus GetStatus() => Status;
    [Obsolete("Use the TranslatedChars property instead. This method will be removed in a future release.")]
    public int GetTranslatedChars() => TranslatedChars;
    [Obsolete("Use the TotalChars property instead. This method will be removed in a future release.")]
    public int GetTotalChars() => TotalChars;
    [Obsolete("Use the Filename property instead. This method will be removed in a future release.")]
    public string GetFilename() => Filename;
    [Obsolete("Use the Source property instead. This method will be removed in a future release.")]
    public string? GetSource() => Source;
    [Obsolete("Use the Target property instead. This method will be removed in a future release.")]
    public string GetTarget() => Target;
    [Obsolete("Use the Options property instead. This method will be removed in a future release.")]
    public DocumentOptions? GetOptions() => Options;
    [Obsolete("Use the ErrorReason property instead. This method will be removed in a future release.")]
    public string? GetErrorReason() => ErrorReason;
    [Obsolete("Use the CreatedAt property instead. This method will be removed in a future release.")]
    public string GetCreatedAt() => CreatedAt;
    [Obsolete("Use the UpdatedAt property instead. This method will be removed in a future release.")]
    public string GetUpdatedAt() => UpdatedAt;
}

public abstract class DocumentExtractionParams { }

public class DocxExtractionParams : DocumentExtractionParams
{
    public bool? ExtractComments { get; set; }
    
    public bool? AcceptRevisions { get; set; }
}

// Future parameter types:
// public class PdfExtractionParams : ExtractionParams { ... }
// public class ExcelExtractionParams : ExtractionParams { ... }