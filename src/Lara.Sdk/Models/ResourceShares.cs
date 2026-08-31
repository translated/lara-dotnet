using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Describes one account, group, or user share.
public class ResourceShareEntry
{
    [JsonConstructor]
    public ResourceShareEntry(string id, string name, string shareName, DateTime sharedAt, string permissions)
    {
        Id = id;
        Name = name;
        ShareName = shareName;
        SharedAt = sharedAt;
        Permissions = permissions;
    }

    public string Id { get; }
    public string Name { get; }
    public string ShareName { get; }
    public DateTime SharedAt { get; }

    /// Permission granted by the share: "read" or "read_write".
    public string Permissions { get; }
}

public class MemoryShares
{
    [JsonConstructor]
    public MemoryShares(Memory memory, ResourceShareEntry? account, List<ResourceShareEntry> groups, List<ResourceShareEntry> users)
    {
        Memory = memory;
        Account = account;
        Groups = groups;
        Users = users;
    }

    public Memory Memory { get; }
    public ResourceShareEntry? Account { get; }
    public List<ResourceShareEntry> Groups { get; }
    public List<ResourceShareEntry> Users { get; }
}

public class GlossaryShares
{
    [JsonConstructor]
    public GlossaryShares(Glossary glossary, ResourceShareEntry? account, List<ResourceShareEntry> groups, List<ResourceShareEntry> users)
    {
        Glossary = glossary;
        Account = account;
        Groups = groups;
        Users = users;
    }

    public Glossary Glossary { get; }
    public ResourceShareEntry? Account { get; }
    public List<ResourceShareEntry> Groups { get; }
    public List<ResourceShareEntry> Users { get; }
}

public class StyleguideShares
{
    [JsonConstructor]
    public StyleguideShares(Styleguide styleguide, ResourceShareEntry? account, List<ResourceShareEntry> groups, List<ResourceShareEntry> users)
    {
        Styleguide = styleguide;
        Account = account;
        Groups = groups;
        Users = users;
    }

    public Styleguide Styleguide { get; }
    public ResourceShareEntry? Account { get; }
    public List<ResourceShareEntry> Groups { get; }
    public List<ResourceShareEntry> Users { get; }
}
