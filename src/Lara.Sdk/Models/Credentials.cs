using System;
using Lara.Sdk.Models.Authentication;

namespace Lara.Sdk;

/// <summary>
/// Represents the credentials required to authenticate with the Lara API.
/// Contains the access key ID and access key secret for HMAC authentication.
/// </summary>
[Obsolete("Use Lara.Sdk.Authentication.AccessKey instead.")]
public class Credentials : AccessKey
{
    /// <summary>
    /// Gets the access key ID used for API authentication.
    /// maps to the base Id property.
    /// </summary>
    public string AccessKeyId => Id;

    /// <summary>
    /// Gets the access key secret used for HMAC signature generation.
    /// maps to the base Secret property.
    /// </summary>
    public string AccessKeySecret => Secret;

    /// <summary>
    /// Initializes a new instance of the <see cref="Credentials"/> class.
    /// </summary>
    /// <param name="accessKeyId">The access key ID.</param>
    /// <param name="accessKeySecret">The access key secret.</param>
    public Credentials(string accessKeyId, string accessKeySecret)
        : base(accessKeyId, accessKeySecret)
    {
    }
}