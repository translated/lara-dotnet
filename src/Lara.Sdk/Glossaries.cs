namespace Lara.Sdk;

/// Glossary management service
public class Glossaries
{
    private readonly LaraClient _client;
    private readonly long _pollingInterval;

    internal Glossaries(LaraClient client, long? pollingInterval = null)
    {
        _client = client;
        _pollingInterval = pollingInterval ?? 2000L;
    }

    /// Lists all glossaries available to the user.
    public async Task<List<Glossary>> List()
    {
        return await _client.Get<List<Glossary>>("/v2/glossaries");
    }
    
    /// Creates a new glossary with a custom name.
    public async Task<Glossary> Create(string name)
    {
        return await _client.Post<Glossary>("/v2/glossaries", new HttpParams<object>().Set("name", name).Build());
    }
    
    /// Gets a glossary by ID.
    public async Task<Glossary?> Get(string id)
    {
        try
        {
            return await _client.Get<Glossary>($"/v2/glossaries/{id}");
        }
        catch (LaraApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }
    
    /// Deletes a specific glossary.
    public async Task<Glossary> Delete(string id)
    {
        return await _client.Delete<Glossary>($"/v2/glossaries/{id}");
    }
    
    /// Updates the name of a specific glossary.
    public async Task<Glossary> Update(string id, string name)
    {
        return await _client.Put<Glossary>($"/v2/glossaries/{id}", new HttpParams<object>().Set("name", name).Build());
    }

    /// <summary>Lists the account, group, and user shares visible for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <returns>The shares configured for the glossary.</returns>
    public async Task<GlossaryShares> GetShares(string id) =>
        await _client.Get<GlossaryShares>($"/v2/glossaries/{id}/shares");

    /// <summary>Creates or updates the account-level share for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <param name="name">An optional share name visible to recipients.</param>
    /// <returns>The updated glossary.</returns>
    public async Task<Glossary> AddAccountShare(string id, string? name = null) =>
        await _client.Post<Glossary>($"/v2/glossaries/{id}/shares", ShareParams.WithName(name));

    /// <summary>Renames the account-level share for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <param name="name">The new share name visible to recipients.</param>
    /// <returns>The updated glossary.</returns>
    public async Task<Glossary> RenameAccountShare(string id, string name) =>
        await _client.Put<Glossary>($"/v2/glossaries/{id}/shares", ShareParams.WithName(name));

    /// <summary>Revokes the account-level share for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <returns>The updated glossary.</returns>
    public async Task<Glossary> RevokeAccountShare(string id) =>
        await _client.Delete<Glossary>($"/v2/glossaries/{id}/shares");

    /// <summary>Creates or updates a group share for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <param name="groupId">The ID of the group receiving access.</param>
    /// <param name="name">An optional share name visible to recipients.</param>
    /// <returns>The updated glossary.</returns>
    public async Task<Glossary> AddGroupShare(string id, string groupId, string? name = null) =>
        await _client.Post<Glossary>($"/v2/glossaries/{id}/shares/groups/{groupId}", ShareParams.WithName(name));

    /// <summary>Renames a group share for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <param name="groupId">The ID of the group whose share is being renamed.</param>
    /// <param name="name">The new share name visible to recipients.</param>
    /// <returns>The updated glossary.</returns>
    public async Task<Glossary> RenameGroupShare(string id, string groupId, string name) =>
        await _client.Put<Glossary>($"/v2/glossaries/{id}/shares/groups/{groupId}", ShareParams.WithName(name));

    /// <summary>Revokes a group share for a glossary.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <param name="groupId">The ID of the group whose access is being revoked.</param>
    /// <returns>The updated glossary.</returns>
    public async Task<Glossary> RevokeGroupShare(string id, string groupId) =>
        await _client.Delete<Glossary>($"/v2/glossaries/{id}/shares/groups/{groupId}");

    /// Imports a file into an existing glossary, optionally registering a callback URL for completion notification.
    public async Task<GlossaryImport> ImportFile(string id, string filePath, GlossaryImportOptions? options = null)
    {
        options ??= new GlossaryImportOptions();
        var parameters = new HttpParams<object>()
            .Set("content_type", options.ContentType.ToString());

        if (options.Gzip == true)
        {
            parameters.Set("compression", "gzip");
        }
        if (options.CallbackUrl != null)
        {
            parameters.Set("callback_url", options.CallbackUrl);
        }

        await using var fileStream = File.OpenRead(filePath);
        var files = new Dictionary<string, Stream> { ["csv"] = fileStream };
        return await _client.Post<GlossaryImport>($"/v2/glossaries/{id}/import", parameters.Build(), files);
    }

    [Obsolete("Use ImportFile instead.")]
    public async Task<GlossaryImport> ImportCsv(string id, string csvFilePath, bool? gzip = false, string? callbackUrl = null)
    {
        return await ImportFile(id, csvFilePath, new GlossaryImportOptions { Gzip = gzip, CallbackUrl = callbackUrl });
    }

    [Obsolete("Use ImportFile instead.")]
    public async Task<GlossaryImport> ImportCsv(string id, string csvFilePath, GlossaryFileFormat contentType, bool? gzip = false, string? callbackUrl = null)
    {
        if (contentType == GlossaryFileFormat.Tbx)
        {
            throw new ArgumentException("ImportCsv only supports CSV formats; use ImportFile for TBX files.", nameof(contentType));
        }
        return await ImportFile(id, csvFilePath, new GlossaryImportOptions
        {
            ContentType = contentType,
            Gzip = gzip,
            CallbackUrl = callbackUrl
        });
    }
    
    /// Checks the status of an ongoing glossary import.
    public async Task<GlossaryImport> GetImportStatus(string id)
    {
        return await _client.Get<GlossaryImport>($"/v2/glossaries/imports/{id}");
    }
    
    /// Waits for import to complete
    public async Task<GlossaryImport> WaitForImport(
        GlossaryImport glossaryImport, 
        Action<GlossaryImport>? updateCallback = null, 
        TimeSpan maxWaitTime = default)
    {
        var startTime = DateTime.UtcNow;
        
        while (glossaryImport.Progress < 1.0f)
        {
            if (maxWaitTime > TimeSpan.Zero && DateTime.UtcNow - startTime > maxWaitTime)
                throw new LaraTimeoutException();
    
            await Task.Delay(TimeSpan.FromMilliseconds(_pollingInterval));
    
            glossaryImport = await GetImportStatus(glossaryImport.Id);
            updateCallback?.Invoke(glossaryImport);
        }
    
        return glossaryImport;
    }
    
    /// Gets the counts for a glossary.
    public async Task<GlossaryCounts> Counts(string id)
    {
        return await _client.Get<GlossaryCounts>($"/v2/glossaries/{id}/counts");
    }
    
    /// <summary>Exports a glossary in the requested Lara glossary file format.</summary>
    /// <param name="id">The glossary ID.</param>
    /// <param name="contentType">A Lara glossary file format identifier, such as <c>csv/table-uni</c> or <c>tbx</c>.</param>
    /// <param name="source">Required for unidirectional CSV formats; omit for multidirectional CSV and TBX.</param>
    /// <returns>A stream containing the exported glossary.</returns>
    public async Task<Stream> Export(string id, string contentType, string? source)
    {
        var parameters = new Dictionary<string, object>
        {
            ["content_type"] = contentType,
        };
        if (source != null)
            parameters["source"] = source;
        
        return await _client.Get<Stream>($"/v2/glossaries/{id}/export", parameters);
    }

    public async Task<Stream> Export(string id, GlossaryFileFormat contentType)
    {
        return await Export(id, contentType, null);
    }
    public async Task<Stream> Export(string id, GlossaryFileFormat contentType, string? source)
    {
        var parameters = new Dictionary<string, object>
        {
            ["content_type"] = contentType.ToString()
        };
        if (source != null)
            parameters["source"] = source;

        return await _client.Get<Stream>($"/v2/glossaries/{id}/export", parameters);
    }

    /// Starts an asynchronous export of a glossary and returns the export job ID.
    public async Task<GlossaryExport> ExportAsync(string id, string callbackUrl, GlossaryFileFormat contentType, string? source = null)
    {
        var queryParams = new HttpParams<object>()
            .Set("callback_url", callbackUrl)
            .Set("content_type", contentType.ToString())
            .Set("source", source)
            .Build();
        return await _client.Get<GlossaryExport>($"/v2/glossaries/{id}/export/async", queryParams);
    }

    /// Adds or replaces terms in a glossary.
    public async Task<GlossaryImport> AddOrReplaceEntry(string id, List<GlossaryTerm> terms, string? guid = null)
    {
        var parameters = new HttpParams<object>()
            .Set("terms", terms);

        if (guid != null)
        {
            parameters.Set("guid", guid);
        }

        return await _client.Put<GlossaryImport>($"/v2/glossaries/{id}/content", parameters.Build());
    }

    /// Deletes an entry from a glossary.
    public async Task<GlossaryImport> DeleteEntry(string id, GlossaryTerm? term = null, string? guid = null)
    {
        var parameters = new HttpParams<object>();

        if (term != null)
        {
            parameters.Set("term", term);
        }

        if (guid != null)
        {
            parameters.Set("guid", guid);
        }

        return await _client.Delete<GlossaryImport>($"/v2/glossaries/{id}/content", parameters.Build());
    }
}
