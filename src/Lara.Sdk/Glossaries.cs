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
    
    /// Imports a CSV file into an existing glossary.
    public async Task<GlossaryImport> ImportCsv(string id, string csvFilePath, bool? gzip = null)
    {
        return await ImportCsv(id, csvFilePath, GlossaryFileFormat.CsvTableUni, gzip);
    }

    public async Task<GlossaryImport> ImportCsv(string id, string csvFilePath, GlossaryFileFormat contentType, bool? gzip = null)
    {
        var parameters = new HttpParams<object>()
            .Set("content_type", contentType.ToString());
        
        if (gzip ?? csvFilePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Set("compression", "gzip");
        }
    
        await using var fileStream = File.OpenRead(csvFilePath);
        var files = new Dictionary<string, Stream> { ["csv"] = fileStream };
        return await _client.Post<GlossaryImport>($"/v2/glossaries/{id}/import", parameters.Build(), files);
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
    
    /// Exports a glossary as CSV.
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