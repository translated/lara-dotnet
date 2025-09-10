namespace Lara;

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
        var response = await _client.Get("/glossaries");
        var glossaries = response.AsWrappedList<Glossary>();
        return glossaries;
    }

    /// Creates a new glossary with a custom name.
    public async Task<Glossary> Create(string name)
    {
        var response = await _client.Post("/glossaries", new HttpParams<object>().Set("name", name).Build());
        return response.AsWrapped<Glossary>();
    }

    /// Gets a glossary by ID.
    public async Task<Glossary?> Get(string id)
    {
        try
        {
            var response = await _client.Get($"/glossaries/{id}");
            return response.AsWrapped<Glossary>();
        }
        catch (LaraApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    /// Deletes a specific glossary.
    public async Task<Glossary> Delete(string id)
    {
        var response = await _client.Delete($"/glossaries/{id}");
        return response.AsWrapped<Glossary>();
    }

    /// Updates the name of a specific glossary.
    public async Task<Glossary> Update(string id, string name)
    {
        var response = await _client.Put($"/glossaries/{id}", new HttpParams<object>().Set("name", name).Build());
        return response.AsWrapped<Glossary>();
    }

    /// Imports a CSV file into an existing glossary.
    public async Task<GlossaryImport> ImportCsv(string id, string csvFilePath, bool? gzip = null)
    {
        var parameters = new HttpParams<object>();
        if (gzip ?? csvFilePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Set("compression", "gzip");
        }

        var files = new Dictionary<string, string> { ["csv"] = csvFilePath };
        var response = await _client.Post($"/glossaries/{id}/import", parameters.Build(), files);
        return response.AsWrapped<GlossaryImport>();
    }

    /// Checks the status of an ongoing glossary import.
    public async Task<GlossaryImport> GetImportStatus(string id)
    {
        var response = await _client.Get($"/glossaries/imports/{id}");
        return response.AsWrapped<GlossaryImport>();
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
        var response = await _client.Get($"/glossaries/{id}/counts");
        return response.AsWrapped<GlossaryCounts>();
    }

    /// Exports a glossary as CSV.
    public async Task<Stream> Export(string id, string contentType, string source)
    {
        var parameters = new Dictionary<string, object>
        {
            ["content_type"] = contentType,
            ["source"] = source
        };

        var response = await _client.Get($"/glossaries/{id}/export", parameters);
        return new MemoryStream(response.RawBytes ?? Array.Empty<byte>());
    }
}