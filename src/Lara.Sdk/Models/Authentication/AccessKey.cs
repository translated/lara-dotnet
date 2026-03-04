namespace Lara.Sdk.Models.Authentication;

/// <summary>
/// Represents an access key used for authentication.
/// </summary>
public class AccessKey
{
    /// <summary>
    /// Gets the access key identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the access key secret.
    /// </summary>
    public string Secret { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessKey"/> class.
    /// </summary>
    /// <param name="id">The access key identifier.</param>
    /// <param name="secret">The access key secret.</param>
    public AccessKey(string id, string secret)
    {
        Id = id;
        Secret = secret;
    }
}