using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lara.Sdk;

public class Memory
{
    /// Gets the unique identifier for the memory.
    public string Id { get; }

    /// Gets the date and time when the memory was created.
    public DateTime CreatedAt { get; }

    /// Gets the date and time when the memory was last updated.
    public DateTime UpdatedAt { get; }

    /// Gets the date and time when the memory was shared.
    public DateTime? SharedAt { get; }

    /// Gets the name of the memory.
    public string Name { get; }

    /// Gets the external ID for integration purposes.
    public string? ExternalId { get; }

    /// Gets the secret key for the memory. ]
    public string? Secret { get; }

    /// Gets the owner ID of the memory.
    public string OwnerId { get; }

    /// Gets the number of collaborators with access to this memory.
    public int CollaboratorsCount { get; }

    /// Initializes a new instance of the Memory class.
    [JsonConstructor]
    public Memory(
        string id, 
        DateTime createdAt, 
        DateTime updatedAt, 
        DateTime? sharedAt, 
        string name, 
        string? externalId, 
        string? secret, 
        string ownerId, 
        int collaboratorsCount)
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
    [Obsolete("Use the Id property instead. This method will be removed in a future release.")]
    public string GetId() => Id;

    /// Gets the date and time when the memory was created.
    [Obsolete("Use the CreatedAt property instead. This method will be removed in a future release.")]
    public DateTime GetCreatedAt() => CreatedAt;

    /// Gets the date and time when the memory was last updated.
    [Obsolete("Use the UpdatedAt property instead. This method will be removed in a future release.")]
    public DateTime GetUpdatedAt() => UpdatedAt;

    /// Gets the date and time when the memory was shared.
    [Obsolete("Use the SharedAt property instead. This method will be removed in a future release.")]
    public DateTime? GetSharedAt() => SharedAt;

    /// Gets the name of the memory.
    [Obsolete("Use the Name property instead. This method will be removed in a future release.")]
    public string GetName() => Name;

    /// Gets the external ID for integration purposes.
    [Obsolete("Use the ExternalId property instead. This method will be removed in a future release.")]
    public string? GetExternalId() => ExternalId;

    /// Gets the secret key for the memory.
    [Obsolete("Use the Secret property instead. This method will be removed in a future release.")]
    public string? GetSecret() => Secret;

    /// Gets the owner ID of the memory.
    [Obsolete("Use the OwnerId property instead. This method will be removed in a future release.")]
    public string GetOwnerId() => OwnerId;

    /// Gets the number of collaborators with access to this memory.
    [Obsolete("Use the CollaboratorsCount property instead. This method will be removed in a future release.")]
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