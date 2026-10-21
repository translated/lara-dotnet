using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// An HTTPS URL or base64 data URL containing a PNG, JPEG or WebP image.
public class ImageContext
{
    [JsonPropertyName("url")]
    public string Url { get; }

    [JsonConstructor]
    public ImageContext(string url)
    {
        Url = url;
    }
}
