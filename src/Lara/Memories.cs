namespace Lara;

/// Translation memory management service
public class Memories
{
    private readonly LaraClient _client;
    private readonly long _pollingInterval;

    /// Initializes a new instance of the Memories class 
    internal Memories(LaraClient client, long? pollingInterval = null)
    {
        _client = client;
        _pollingInterval = pollingInterval ?? 2000L;
    }

    /// Lists all memories.
    public async Task<List<Memory>> List()
    {
        var response = await _client.Get("/memories");
        var memories = response.AsWrappedList<Memory>();
        return memories;
    }

    /// Creates a new memory with external ID
    public async Task<Memory> Create(string name, string? externalId = null)
    {
        var parameters = new HttpParams<object>()
            .Set("name", name);
            
        if (externalId != null)
            parameters.Set("external_id", externalId);

        var response = await _client.Post("/memories", parameters.Build());
        return response.AsWrapped<Memory>();
    }

    /// Gets a memory by ID
    public async Task<Memory?> Get(string id)
    {
        try
        {
            var response = await _client.Get($"/memories/{id}");
            return response.AsWrapped<Memory>();
        }
        catch (LaraApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    /// Deletes a memory
    public async Task<Memory> Delete(string id)
    {
        var response = await _client.Delete($"/memories/{id}");
        return response.AsWrapped<Memory>();
    }

    /// Updates a memory
    public async Task<Memory> Update(string id, string name)
    {
        var parameters = new HttpParams<object>()
            .Set("name", name);

        var response = await _client.Put($"/memories/{id}", parameters.Build());
        return response.AsWrapped<Memory>();
    }

    /// Connects to a memory.
    public async Task<Memory?> Connect(string id)
    {
        var result = await Connect(new List<string> { id });
        return result.Count > 0 ? result[0] : null;
    }

    /// Connects to multiple memories
    public async Task<List<Memory>> Connect(List<string> ids)
    {
        var parameters = new HttpParams<object>()
            .Set("ids", ids);

        var response = await _client.Post("/memories/connect", parameters.Build());
        return response.AsList<Memory>();
    }
    
    /// Imports a TMX file 
    public async Task<MemoryImport> ImportTmx(string id, string tmxFilePath, bool? gzip = null)
    {
        var parameters = new HttpParams<object>();
        if (gzip ?? tmxFilePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Set("compression", "gzip");
        }

        var files = new Dictionary<string, string> { ["tmx"] = tmxFilePath };
        var response = await _client.Post($"/memories/{id}/import", parameters.Build(), files);
        return response.AsWrapped<MemoryImport>();
    }

    /// Gets import status
    public async Task<MemoryImport> GetImportStatus(string id)
    {
        var response = await _client.Get($"/memories/imports/{id}");
        return response.AsWrapped<MemoryImport>();
    }

    /// Waits for import to complete
    public async Task<MemoryImport> WaitForImport(
        MemoryImport memoryImport, 
        Action<MemoryImport>? updateCallback = null, 
        TimeSpan maxWaitTime = default)
    {
        var startTime = DateTime.UtcNow;
        
        while (memoryImport.Progress < 1.0f)
        {
            if (maxWaitTime > TimeSpan.Zero && DateTime.UtcNow - startTime > maxWaitTime)
                throw new LaraTimeoutException();

            await Task.Delay(TimeSpan.FromMilliseconds(_pollingInterval));

            memoryImport = await GetImportStatus(memoryImport.Id);
            updateCallback?.Invoke(memoryImport);
        }

        return memoryImport;
    }

    /// Adds a translation unit to a memory 
    public async Task<MemoryImport> AddTranslation(
        string id, 
        string source, 
        string target, 
        string sentence, 
        string translation, 
        string? tuid = null, 
        string? sentenceBefore = null, 
        string? sentenceAfter = null, 
        Dictionary<string, string>? headers = null
        )
    {
        var parameters = new HttpParams<object>()
            .Set("source", source)
            .Set("target", target)
            .Set("sentence", sentence)
            .Set("translation", translation);
        if (tuid != null)
            parameters.Set("tuid", tuid);
        if (sentenceBefore != null)
            parameters.Set("sentence_before", sentenceBefore);
        if (sentenceAfter != null)
            parameters.Set("sentence_after", sentenceAfter);

        var response = await _client.Put($"/memories/{id}/content", parameters.Build(), null, headers);
        return response.AsWrapped<MemoryImport>();
    }

    /// Adds a translation unit to multiple memories
    public async Task<MemoryImport> AddTranslation(
        List<string> ids, 
        string source, 
        string target, 
        string sentence, 
        string translation,
        string? tuid = null, 
        string? sentenceBefore = null, 
        string? sentenceAfter = null, 
        Dictionary<string, string>? headers = null
        )
    {
        var parameters = new HttpParams<object>()
            .Set("ids", ids)
            .Set("source", source)
            .Set("target", target)
            .Set("sentence", sentence)
            .Set("translation", translation);
        if (tuid != null)
            parameters.Set("tuid", tuid);
        if (sentenceBefore != null)
            parameters.Set("sentence_before", sentenceBefore);
        if (sentenceAfter != null)
            parameters.Set("sentence_after", sentenceAfter);
    
        var response = await _client.Put("/memories/content", parameters.Build(), null, headers);
        return response.AsWrapped<MemoryImport>();
    }

    /// Deletes a translation unit from a memory
    public async Task<MemoryImport> DeleteTranslation(
        string id, 
        string source, 
        string target, 
        string sentence, 
        string translation,
        string? tuid = null, 
        string? sentenceBefore = null, 
        string? sentenceAfter = null
        )
    {
        var parameters = new HttpParams<object>()
            .Set("source", source)
            .Set("target", target)
            .Set("sentence", sentence)
            .Set("translation", translation);
        if (tuid != null)
            parameters.Set("tuid", tuid);
        if (sentenceBefore != null)
            parameters.Set("sentence_before", sentenceBefore);
        if (sentenceAfter != null)
            parameters.Set("sentence_after", sentenceAfter);

        var response = await _client.Delete($"/memories/{id}/content", parameters.Build());
        return response.AsWrapped<MemoryImport>();
    }

    /// Deletes a translation unit from multiple memories
    public async Task<MemoryImport> DeleteTranslation(
        List<string> ids, 
        string source, 
        string target, 
        string sentence, 
        string translation,
        string? tuid = null, 
        string? sentenceBefore = null, 
        string? sentenceAfter = null
        )
    {
        var parameters = new HttpParams<object>()
            .Set("ids", ids)
            .Set("source", source)
            .Set("target", target)
            .Set("sentence", sentence)
            .Set("translation", translation);
        if (tuid != null)
            parameters.Set("tuid", tuid);
        if (sentenceBefore != null)
            parameters.Set("sentence_before", sentenceBefore);
        if (sentenceAfter != null)
            parameters.Set("sentence_after", sentenceAfter);

        var response = await _client.Delete("/memories/content", parameters.Build());
        return response.AsWrapped<MemoryImport>();
    }
}