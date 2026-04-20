namespace Lara.Sdk;

/// Styleguide management service
public class Styleguides
{
    private readonly LaraClient _client;

    internal Styleguides(LaraClient client)
    {
        _client = client;
    }

    /// Lists all styleguides available to the user.
    public async Task<List<Styleguide>> List()
    {
        return await _client.Get<List<Styleguide>>("/v2/styleguides");
    }

    /// Gets a styleguide by ID.
    public async Task<Styleguide?> Get(string id)
    {
        try
        {
            return await _client.Get<Styleguide>($"/v2/styleguides/{id}");
        }
        catch (LaraApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }
}
