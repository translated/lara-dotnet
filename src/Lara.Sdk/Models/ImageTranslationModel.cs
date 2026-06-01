namespace Lara.Sdk;

/// <summary>
/// Defines the model used to translate text that appears in an image.
/// </summary>
public enum ImageTranslationModel
{
    /// <summary>
    /// Covers the original text with a solid or stylized overlay, without attempting
    /// to reconstruct the underlying image content.
    /// </summary>
    Overlay,

    /// <summary>
    /// Uses inpainting techniques to reconstruct the background.
    /// </summary>
    Inpainting,

    /// <summary>
    /// Uses a generative model to translate text within the image.
    /// </summary>
    Generative,

    /// <summary>
    /// Uses a faster generative model to translate text within the image.
    /// </summary>
    GenerativeFast
}
