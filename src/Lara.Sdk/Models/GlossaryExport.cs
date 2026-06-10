using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents the result of starting an asynchronous glossary export.
public class GlossaryExport
{
    /// Gets the identifier of the export job.
    [JsonPropertyName("job_id")]
    public string JobId { get; }

    /// Initializes a new instance of the GlossaryExport class.
    public GlossaryExport(string jobId)
    {
        JobId = jobId;
    }

    public override string ToString()
    {
        return $"GlossaryExport{{jobId='{JobId}'}}";
    }
}
