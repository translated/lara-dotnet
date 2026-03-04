namespace Lara.Sdk;

/// <summary>
/// Defines the strategy used to remove or hide text that appears in an image.
/// </summary>
public enum ImageTextRemoval
{
    /// <summary>
    /// Covers the original text with a solid or stylized overlay, without attempting
    /// to reconstruct the underlying image content.
    /// </summary>
    Overlay,

    /// <summary>
    /// Uses inpainting techniques to  reconstruct the background..
    /// </summary>
    Inpainting
}