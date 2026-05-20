/// <summary>
/// Represents the export format for translation memories, providing predefined formats.
/// This class uses a smart enum pattern to define supported memory export formats.
/// </summary>
public sealed class MemoryExportFormat
{
    /// <summary>
    /// Translation Memory eXchange (TMX) format.
    /// </summary>
    public static readonly MemoryExportFormat TMX = new MemoryExportFormat("tmx");

    /// <summary>
    /// JSON-based translation memory format (JTM).
    /// </summary>
    public static readonly MemoryExportFormat JTM = new MemoryExportFormat("jtm");

    /// <summary>
    /// Gets the string value representing the memory export format.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryExportFormat"/> class with the specified value.
    /// </summary>
    /// <param name="value">The string value of the format.</param>
    private MemoryExportFormat(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Returns the string representation of the memory export format.
    /// </summary>
    /// <returns>The value of the format.</returns>
    public override string ToString() => Value;
}