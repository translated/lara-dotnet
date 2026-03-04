namespace Lara.Sdk;

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
        return await _client.Get<List<Memory>>("/v2/memories");
    }
    
    /// Creates a new memory with external ID
    public async Task<Memory> Create(string name, string? externalId = null)
    {
        var parameters = new HttpParams<object>()
            .Set("name", name);
            
        if (externalId != null)
            parameters.Set("external_id", externalId);
    
        return await _client.Post<Memory>("/v2/memories", parameters.Build());
    }
    
    /// Gets a memory by ID
    public async Task<Memory?> Get(string id)
    {
        try
        {
            return await _client.Get<Memory>($"/v2/memories/{id}");
        }
        catch (LaraApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }
    
    /// Deletes a memory
    public async Task<Memory> Delete(string id)
    {
        return await _client.Delete<Memory>($"/v2/memories/{id}");
    }
    
    /// Updates a memory
    public async Task<Memory> Update(string id, string name)
    {
        var parameters = new HttpParams<object>()
            .Set("name", name);
    
        return await _client.Put<Memory>($"/v2/memories/{id}", parameters.Build());
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
    
        return await _client.Post<List<Memory>>("/v2/memories/connect", parameters.Build());
    }
    
    /// Imports a TMX file 
    public async Task<MemoryImport> ImportTmx(string id, string tmxFilePath, bool? gzip = null)
    {
        var parameters = new HttpParams<object>();
        if (gzip ?? tmxFilePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
        {
            parameters.Set("compression", "gzip");
        }
    
        await using var fileStream = File.OpenRead(tmxFilePath);
        var files = new Dictionary<string, Stream> { ["tmx"] = fileStream };
        return await _client.Post<MemoryImport>($"/v2/memories/{id}/import", parameters.Build(), files);
    }
    
    /// Gets import status
    public async Task<MemoryImport> GetImportStatus(string id)
    {
        return await _client.Get<MemoryImport>($"/v2/memories/imports/{id}");
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
    
        return await _client.Put<MemoryImport>($"/v2/memories/{id}/content", parameters.Build(), headers);
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
    
        return await _client.Put<MemoryImport>("/v2/memories/content", parameters.Build(), headers);
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
    
        return await _client.Delete<MemoryImport>($"/v2/memories/{id}/content", parameters.Build());
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
    
        return await _client.Delete<MemoryImport>("/memories/content", parameters.Build());
    }
}