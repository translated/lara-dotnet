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

    /// Creates a new styleguide with a name and content.
    public async Task<Styleguide> Create(string name, string content)
    {
        var parameters = new HttpParams<object>()
            .Set("name", name)
            .Set("content", content);
        return await _client.Post<Styleguide>("/v2/styleguides", parameters.Build());
    }

    /// Updates a styleguide. Pass null for fields you don't want to change.
    public async Task<Styleguide> Update(string id, string? name = null, string? content = null)
    {
        var parameters = new HttpParams<object>();
        if (name != null) parameters.Set("name", name);
        if (content != null) parameters.Set("content", content);
        return await _client.Put<Styleguide>($"/v2/styleguides/{id}", parameters.Build());
    }

    /// Deletes a specific styleguide.
    public async Task<Styleguide> Delete(string id)
    {
        return await _client.Delete<Styleguide>($"/v2/styleguides/{id}");
    }
}
