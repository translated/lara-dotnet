namespace Lara.Sdk;

/// <summary>Options for importing a glossary file.</summary>
public class GlossaryImportOptions
{
    public GlossaryFileFormat ContentType { get; set; } = GlossaryFileFormat.CsvTableUni;

    /// <summary>Null detects gzip from the filename; true marks an already compressed file.</summary>
    public bool? Gzip { get; set; }

    public string? CallbackUrl { get; set; }
}
