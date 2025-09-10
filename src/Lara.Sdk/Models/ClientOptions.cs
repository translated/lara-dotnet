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
        private set => _serverUrl = value;
    }
    
    /// Gets or sets the connection timeout in milliseconds.
    public TimeSpan ConnectionTimeout { get; private set; } = TimeSpan.Zero;
    
    /// Gets or sets the read timeout in milliseconds.
    public TimeSpan ReadTimeout { get; private set; } = TimeSpan.Zero;

    /// Sets the server URL with trailing slash normalization.

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
    public ClientOptions SetConnectionTimeout(TimeSpan timeout)
    {
        ConnectionTimeout = timeout;
        return this;
    }
    
    /// Sets the read timeout.
    public ClientOptions SetReadTimeout(TimeSpan timeout)
    {
        ReadTimeout = timeout;
        return this;
    }
}