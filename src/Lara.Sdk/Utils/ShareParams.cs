namespace Lara.Sdk;

internal static class ShareParams
{
    public static Dictionary<string, object>? WithName(string? name) =>
        new HttpParams<object>().Set("name", name).Build();
}
