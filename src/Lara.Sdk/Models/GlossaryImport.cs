using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a glossary import operation.
public class GlossaryImport
{
    /// Gets the unique identifier for the import operation.
    [JsonPropertyName("id")]
    public string Id { get; }

    /// Gets the progress of the import operation (0.0 to 1.0).
    [JsonPropertyName("progress")]
    public double Progress { get; }

    /// Gets the status message of the import operation.
    [JsonPropertyName("status")]
    public string? Status { get; }

    /// Gets the error message if the import failed.
    [JsonPropertyName("error")]
    public string? Error { get; }

    /// Gets the creation timestamp of the import operation.
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; }

    /// Gets the last update timestamp of the import operation.
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; }

    /// Initializes a new instance of the GlossaryImport class.

    public GlossaryImport(string id, double progress, string? status, string? error, 
        string createdAt, string updatedAt)
    {
        Id = id;
        Progress = progress;
        Status = status;
        Error = error;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    // Java-style getter methods
    public string GetId() => Id;
    public double GetProgress() => Progress;
    public string? GetStatus() => Status;
    public string? GetError() => Error;
    public string GetCreatedAt() => CreatedAt;
    public string GetUpdatedAt() => UpdatedAt;

    /// Returns a string representation of the glossary import.
    public override string ToString()
    {
        return $"GlossaryImport{{id='{Id}', progress={Progress:F2}}}";
    }
}