using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>Original text direction (ltr, rtl, or ttb) and colors.</summary>
public class ImageTextInfo
{
    [JsonPropertyName("direction")]
    public string Direction { get; }
    [JsonPropertyName("text_color")]
    public string TextColor { get; }
    [JsonPropertyName("background_color")]
    public string BackgroundColor { get; }

    [JsonConstructor]
    public ImageTextInfo(string direction, string textColor, string backgroundColor)
    {
        Direction = direction;
        TextColor = textColor;
        BackgroundColor = backgroundColor;
    }
}
