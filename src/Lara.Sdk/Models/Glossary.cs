using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a glossary in the Lara API.
public class Glossary
{
    /// Glossary type enumeration
    public enum Type
    {
        [JsonPropertyName("csv/table-uni")]
        CsvTableUni
    }

    /// Gets the unique identifier for the glossary.
    public string Id { get; }

    /// Gets the date and time when the glossary was created.
    public DateTime CreatedAt { get; }

    /// Gets the date and time when the glossary was last updated.
    public DateTime UpdatedAt { get; }

    /// Gets the name of the glossary.
    public string Name { get; }

    /// Gets the owner ID of the glossary.
    public string OwnerId { get; }

    /// Initializes a new instance of the Glossary class.
    [JsonConstructor]
    public Glossary(
        string id, 
        DateTime createdAt, 
        DateTime updatedAt, 
        string name, 
        string ownerId
        )
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Name = name;
        OwnerId = ownerId;
    }

    /// Gets the unique identifier for the glossary.
    [Obsolete("Use the Id property instead. This method will be removed in a future release.")]
    public string GetId() => Id;

    /// Gets the date and time when the glossary was created.
    [Obsolete("Use the CreatedAt property instead. This method will be removed in a future release.")]
    public DateTime GetCreatedAt() => CreatedAt;

    /// Gets the date and time when the glossary was last updated.
    [Obsolete("Use the UpdatedAt property instead. This method will be removed in a future release.")]
    public DateTime GetUpdatedAt() => UpdatedAt;

    /// Gets the name of the glossary.
    [Obsolete("Use the Name property instead. This method will be removed in a future release.")]
    public string GetName() => Name;

    /// Gets the owner ID of the glossary.
    [Obsolete("Use the OwnerId property instead. This method will be removed in a future release.")]
    public string GetOwnerId() => OwnerId;

    /// Returns a string representation of the glossary.
    public override string ToString()
    {
        return $"Glossary{{id='{Id}', name='{Name}'}}";
    }

    /// Determines whether the specified object is equal to the current glossary.
    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Glossary glossary) return false;
        return Id.Equals(glossary.Id);
    }

    /// Returns a hash code for the current glossary.
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}