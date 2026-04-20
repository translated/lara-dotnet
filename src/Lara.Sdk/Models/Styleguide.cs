using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a styleguide in the Lara API.
public class Styleguide
{
    /// Gets the unique identifier for the styleguide.
    public string Id { get; }

    /// Gets the name of the styleguide.
    public string Name { get; }

    /// Gets the content of the styleguide.
    public string? Content { get; }

    /// Gets the owner ID of the styleguide.
    public string OwnerId { get; }

    /// Gets the date and time when the styleguide was created.
    public DateTime CreatedAt { get; }

    /// Gets the date and time when the styleguide was last updated.
    public DateTime UpdatedAt { get; }

    [JsonConstructor]
    public Styleguide(
        string id,
        string name,
        string? content,
        string ownerId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Content = content;
        OwnerId = ownerId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public override string ToString()
    {
        return $"Styleguide{{id='{Id}', name='{Name}'}}";
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Styleguide styleguide) return false;
        return Id.Equals(styleguide.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
