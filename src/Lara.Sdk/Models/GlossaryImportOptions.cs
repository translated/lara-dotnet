namespace Lara.Sdk;

/// <summary>Options for importing a glossary file.</summary>
public class GlossaryImportOptions
{
    public GlossaryFileFormat ContentType { get; set; } = GlossaryFileFormat.CsvTableUni;

    /// <summary>
    /// Whether the supplied file is already gzip-compressed. True sends <c>compression=gzip</c>;
    /// false (the default) and null omit the compression field. File contents are uploaded unchanged.
    /// </summary>
    public bool? Gzip { get; set; } = false;

    public string? CallbackUrl { get; set; }
}
