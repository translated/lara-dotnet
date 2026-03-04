namespace Lara.Sdk;

/// Configuration options for the Lara API client.
public class ClientOptions
{
    private const string DefaultServerUrl = "https://api.laratranslate.com";
    
    private string? _serverUrl;
    
    /// Gets or sets the server URL for the Lara API.
    public string ServerUrl 
    { 
        get => _serverUrl ?? DefaultServerUrl;
        set
        {
            if (value != null)
            {
                while (value.EndsWith("/"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }
            _serverUrl = value;
        }
    }
    
    /// Gets or sets the connection timeout in milliseconds.
    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.Zero;
    
    /// Gets or sets the read timeout in milliseconds.
    public TimeSpan ReadTimeout { get; set; } = TimeSpan.Zero;

    /// Sets the server URL with trailing slash normalization.
    [Obsolete("Use the ServerUrl property instead. This method will be removed in a future version.")]
    public ClientOptions SetServerUrl(string? serverUrl)
    {
        if (serverUrl != null)
        {
            while (serverUrl.EndsWith("/"))
            {
                serverUrl = serverUrl.Substring(0, serverUrl.Length - 1);
            }
        }
        
        ServerUrl = serverUrl ?? DefaultServerUrl;
        return this;
    }
    
    /// Sets the connection timeout.
    [Obsolete("Use the ConnectionTimeout property instead. This method will be removed in a future version.")]
    public ClientOptions SetConnectionTimeout(TimeSpan timeout)
    {
        ConnectionTimeout = timeout;
        return this;
    }
    
    /// Sets the read timeout.
    [Obsolete("Use the ReadTimeout property instead. This method will be removed in a future version.")]
    public ClientOptions SetReadTimeout(TimeSpan timeout)
    {
        ReadTimeout = timeout;
        return this;
    }
}