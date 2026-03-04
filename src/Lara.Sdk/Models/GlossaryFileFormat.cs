namespace Lara.Sdk;

/// <summary>
/// Represents the file format for glossaries, providing predefined formats for CSV table structures.
/// This class uses a smart enum pattern to define supported glossary file formats.
/// </summary>
public sealed class GlossaryFileFormat
{
    
    /// <summary>
    /// Represents the CSV table format for single-language glossaries.
    /// </summary>
    public static readonly GlossaryFileFormat CsvTableUni = new GlossaryFileFormat("csv/table-uni");

    /// <summary>
    /// Represents the CSV table format for multi-language glossaries.
    /// </summary>
    public static readonly GlossaryFileFormat CsvTableMulti = new GlossaryFileFormat("csv/table-multi");
    
    /// <summary>
    /// Gets the string value representing the glossary file format.
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GlossaryFileFormat"/> class with the specified value.
    /// </summary>
    /// <param name="value">The string value of the format.</param>
    private GlossaryFileFormat(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Returns the string representation of the glossary file format.
    /// </summary>
    /// <returns>The value of the format.</returns>
    public override string ToString() => Value;
}
