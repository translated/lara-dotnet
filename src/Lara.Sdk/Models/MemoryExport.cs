using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents the result of starting an asynchronous memory export.
public class MemoryExport
{
    /// Gets the identifier of the export job.
    [JsonPropertyName("job_id")]
    public string JobId { get; }

    /// Initializes a new instance of the MemoryExport class.
    public MemoryExport(string jobId)
    {
        JobId = jobId;
    }

    public override string ToString()
    {
        return $"MemoryExport{{jobId='{JobId}'}}";
    }
}
