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

    /// <summary>Lists the account, group, and user shares visible for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <returns>The shares configured for the styleguide.</returns>
    public async Task<StyleguideShares> GetShares(string id) =>
        await _client.Get<StyleguideShares>($"/v2/styleguides/{id}/shares");

    /// <summary>Creates or updates the account-level share for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <param name="name">An optional share name visible to recipients.</param>
    /// <returns>The updated styleguide.</returns>
    public async Task<Styleguide> AddAccountShare(string id, string? name = null) =>
        await _client.Post<Styleguide>($"/v2/styleguides/{id}/shares", ShareParams.WithName(name));

    /// <summary>Renames the account-level share for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <param name="name">The new share name visible to recipients.</param>
    /// <returns>The updated styleguide.</returns>
    public async Task<Styleguide> RenameAccountShare(string id, string name) =>
        await _client.Put<Styleguide>($"/v2/styleguides/{id}/shares", ShareParams.WithName(name));

    /// <summary>Revokes the account-level share for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <returns>The updated styleguide.</returns>
    public async Task<Styleguide> RevokeAccountShare(string id) =>
        await _client.Delete<Styleguide>($"/v2/styleguides/{id}/shares");

    /// <summary>Creates or updates a group share for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <param name="groupId">The ID of the group receiving access.</param>
    /// <param name="name">An optional share name visible to recipients.</param>
    /// <returns>The updated styleguide.</returns>
    public async Task<Styleguide> AddGroupShare(string id, string groupId, string? name = null) =>
        await _client.Post<Styleguide>($"/v2/styleguides/{id}/shares/groups/{groupId}", ShareParams.WithName(name));

    /// <summary>Renames a group share for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <param name="groupId">The ID of the group whose share is being renamed.</param>
    /// <param name="name">The new share name visible to recipients.</param>
    /// <returns>The updated styleguide.</returns>
    public async Task<Styleguide> RenameGroupShare(string id, string groupId, string name) =>
        await _client.Put<Styleguide>($"/v2/styleguides/{id}/shares/groups/{groupId}", ShareParams.WithName(name));

    /// <summary>Revokes a group share for a styleguide.</summary>
    /// <param name="id">The styleguide ID.</param>
    /// <param name="groupId">The ID of the group whose access is being revoked.</param>
    /// <returns>The updated styleguide.</returns>
    public async Task<Styleguide> RevokeGroupShare(string id, string groupId) =>
        await _client.Delete<Styleguide>($"/v2/styleguides/{id}/shares/groups/{groupId}");

}
