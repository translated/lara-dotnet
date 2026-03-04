using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>
/// Represents the status of an audio translation operation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AudioStatus
{
    /// <summary>
    /// Audio has just been created.
    /// </summary>
    // [JsonPropertyName("initialized")]
    Initialized,

    /// <summary>
    /// Audio is being analyzed for language detection and duration.
    /// </summary>
    // [JsonPropertyName("analyzing")]
    Analyzing,

    /// <summary>
    /// Audio is paused after analysis, needs user confirmation.
    /// </summary>
    // [JsonPropertyName("paused")]
    Paused,

    /// <summary>
    /// Audio is ready to be translated.
    /// </summary>
    // [JsonPropertyName("ready")]
    Ready,

    /// <summary>
    /// Audio is currently being translated.
    /// </summary>
    // [JsonPropertyName("translating")]
    Translating,

    /// <summary>
    /// Audio has been successfully translated.
    /// </summary>
    // [JsonPropertyName("translated")]
    Translated,

    /// <summary>
    /// An error occurred during audio processing.
    /// </summary>
    // [JsonPropertyName("error")]
    Error
}

/// <summary>
/// Common options that can be applied to audio translation operations.
/// </summary>
public class AudioOptions
{
    /// <summary>
    /// Optional audience or domain adaptation codes to tailor the translation.
    /// </summary>
    public string[]? AdaptTo { get; set; }
    
    /// <summary>
    /// Optional translation style (e.g., Faithful, Fluid, Creative) to influence the output.
    /// </summary>
    public TranslationStyle? Style { get; set; }
}

/// <summary>
/// Represents options for audio upload operations.
/// </summary>
public class AudioUploadOptions : AudioOptions
{
    /// <summary>
    /// Optional list of glossary identifiers to apply during translation.
    /// </summary>
    public string[]? Glossaries { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to disable tracing for this request.
    /// </summary>
    public bool NoTrace { get; set; }
}

/// <summary>
/// Represents an audio translation job and its metadata.
/// </summary>
public class Audio
{
    /// <summary>
    /// Gets the unique identifier of the audio job.
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// Gets the current status of the audio job.
    /// </summary>
    public AudioStatus Status { get; }
    
    /// <summary>
    /// Gets the total number of seconds already translated.
    /// </summary>
    public int TranslatedSeconds { get; }
    
    /// <summary>
    /// Gets the total number of seconds in the audio.
    /// </summary>
    public int TotalSeconds { get; }
    
    /// <summary>
    /// Gets the filename of the audio file.
    /// </summary>
    public string Filename { get; }

    /// <summary>
    /// Gets the source language of the audio, if detected.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// Gets the target language for translation.
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Gets the options used for this audio translation.
    /// </summary>
    public AudioOptions? Options { get; }

    /// <summary>
    /// Gets the error reason if the audio status is <see cref="AudioStatus.Error"/>.
    /// </summary>
    public string? ErrorReason { get; }

    /// <summary>
    /// Gets the creation timestamp of the audio.
    /// </summary>
    public string CreatedAt { get; }

    /// <summary>
    /// Gets the last update timestamp of the audio.
    /// </summary>
    public string UpdatedAt { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Audio"/> class.
    /// </summary>
    /// <param name="id">Unique identifier of the audio job.</param>
    /// <param name="status">Current status of the job.</param>
    /// <param name="filename">Original filename of the uploaded audio.</param>
    /// <param name="target">Target language code for translation.</param>
    /// <param name="createdAt">Creation timestamp.</param>
    /// <param name="updatedAt">Last update timestamp.</param>
    /// <param name="translatedSeconds">Number of seconds already translated (default 0).</param>
    /// <param name="totalSeconds">Total length of the audio in seconds (default 0).</param>
    /// <param name="source">Optional detected source language.</param>
    /// <param name="options">Optional translation options used for this job.</param>
    /// <param name="errorReason">Optional error description if the job has failed.</param>
    [JsonConstructor]
    public Audio(
        string id,
        AudioStatus status,
        string filename,
        string target,
        string createdAt,
        string updatedAt,
        int translatedSeconds = 0,
        int totalSeconds = 0,
        string? source = null,
        AudioOptions? options = null,
        string? errorReason = null
    )
    {
        Id = id;
        Status = status;
        TranslatedSeconds = translatedSeconds;
        TotalSeconds = totalSeconds;
        Filename = filename;
        Source = source;
        Target = target;
        Options = options;
        ErrorReason = errorReason;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
        
    
}