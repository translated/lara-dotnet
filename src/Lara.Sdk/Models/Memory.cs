using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class Memory
{
    /// Gets the unique identifier for the memory.
    [JsonPropertyName("id")]
    public string Id { get; }

    /// Gets the date and time when the memory was created.
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; }

    /// Gets the date and time when the memory was last updated.
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; }

    /// Gets the date and time when the memory was shared.
    [JsonPropertyName("shared_at")]
    public DateTime? SharedAt { get; }

    /// Gets the name of the memory.
    [JsonPropertyName("name")]
    public string Name { get; }

    /// Gets the external ID for integration purposes.
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; }

    /// Gets the secret key for the memory.
    [JsonPropertyName("secret")]
    public string? Secret { get; }

    /// Gets the owner ID of the memory.
    [JsonPropertyName("owner_id")]
    public string OwnerId { get; }

    /// Gets the number of collaborators with access to this memory.
    [JsonPropertyName("collaborators_count")]
    public int CollaboratorsCount { get; }

    /// Initializes a new instance of the Memory class.
    public Memory(string id, DateTime createdAt, DateTime updatedAt, DateTime? sharedAt, 
        string name, string? externalId, string? secret, string ownerId, int collaboratorsCount)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        SharedAt = sharedAt;
        Name = name;
        ExternalId = externalId;
        Secret = secret;
        OwnerId = ownerId;
        CollaboratorsCount = collaboratorsCount;
    }

    /// Gets the unique identifier for the memory.
    public string GetId() => Id;

    /// Gets the date and time when the memory was created.
    public DateTime GetCreatedAt() => CreatedAt;

    /// Gets the date and time when the memory was last updated.
    public DateTime GetUpdatedAt() => UpdatedAt;

    /// Gets the date and time when the memory was shared.
    public DateTime? GetSharedAt() => SharedAt;

    /// Gets the name of the memory.
    public string GetName() => Name;

    /// Gets the external ID for integration purposes.
    public string? GetExternalId() => ExternalId;

    /// Gets the secret key for the memory.
    public string? GetSecret() => Secret;

    /// Gets the owner ID of the memory.
    public string GetOwnerId() => OwnerId;

    /// Gets the number of collaborators with access to this memory.
    public int GetCollaboratorsCount() => CollaboratorsCount;

    /// Returns a string representation of the memory.
    public override string ToString()
    {
        return $"Memory{{id='{Id}', name='{Name}'}}";
    }

    /// Determines whether the specified object is equal to the current memory.
    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not Memory memory) return false;
        return Id.Equals(memory.Id);
    }

    /// Returns a hash code for the current memory.
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}