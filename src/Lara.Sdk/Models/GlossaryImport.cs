using System;
using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a glossary import operation.
public class GlossaryImport
{
    /// Gets the unique identifier for the import operation.
    public string Id { get; }

    /// Gets the progress of the import operation (0.0 to 1.0).
    public double Progress { get; }

    /// Gets the beginning index of the import operation.
    public int Begin { get; }

    /// Gets the ending index of the import operation.
    public int End { get; }

    /// Gets the channel number for the import operation.
    public int Channel { get; }

    /// Gets the total size of the import operation.
    public int Size { get; }

    /// Initializes a new instance of the GlossaryImport class.
    [JsonConstructor]
    public GlossaryImport(string id, double progress, int begin, int end, int channel, int size)
    {
        Id = id;
        Progress = progress;
        Begin = begin;
        End = end;
        Channel = channel;
        Size = size;
    }

    // Java-style getter methods (deprecated)
    [Obsolete("Use the Id property directly instead.")]
    public string GetId() => Id;

    [Obsolete("Use the Progress property directly instead.")]
    public double GetProgress() => Progress;

    /// Returns a string representation of the glossary import.
    public override string ToString()
    {
        return $"GlossaryImport{{id='{Id}', progress={Progress:F2}, begin={Begin}, end={End}, channel={Channel}, size={Size}}}";
    }
}