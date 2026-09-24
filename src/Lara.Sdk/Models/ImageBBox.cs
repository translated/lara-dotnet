using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// <summary>Four corners of a paragraph or line, each an [x, y] pair of pixel coordinates.</summary>
public class ImageBBox
{
    [JsonPropertyName("top_left")]
    public int[] TopLeft { get; }
    [JsonPropertyName("top_right")]
    public int[] TopRight { get; }
    [JsonPropertyName("bottom_right")]
    public int[] BottomRight { get; }
    [JsonPropertyName("bottom_left")]
    public int[] BottomLeft { get; }

    [JsonConstructor]
    public ImageBBox(int[] topLeft, int[] topRight, int[] bottomRight, int[] bottomLeft)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
    }
}
