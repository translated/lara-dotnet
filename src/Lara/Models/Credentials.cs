namespace Lara;

/// Represents the credentials required to authenticate with the Lara API.
/// Contains the access key ID and access key secret for HMAC authentication.
public class Credentials
{
    /// Gets the access key ID used for API authentication.
    public string AccessKeyId { get; }

    /// Gets the access key secret used for HMAC signature generation.
    public string AccessKeySecret { get; }

    /// Initializes a new instance of the <see cref="Credentials"/> class.
    public Credentials(string accessKeyId, string accessKeySecret)
    {
        if (string.IsNullOrEmpty(accessKeyId))
            throw new ArgumentException("Access key ID cannot be null or empty.", nameof(accessKeyId));
        
        if (string.IsNullOrEmpty(accessKeySecret))
            throw new ArgumentException("Access key secret cannot be null or empty.", nameof(accessKeySecret));

        AccessKeyId = accessKeyId;
        AccessKeySecret = accessKeySecret;
    }
} 