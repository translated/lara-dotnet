using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a text block with translatability flag.
public class TextBlock
{
    /// Gets the text content of the block.
    public string Text { get; }

    /// Gets a value indicating whether this text block is translatable.
    public bool Translatable { get; }

    /// Initializes a new instance with text and translatability flag.
    [JsonConstructor]
    public TextBlock(string text, bool translatable = true)
    {
        Text = text;
        Translatable = translatable;
    }

    /// Gets the text content of the block.
    [Obsolete("Use the Text property instead. This property will be removed in a future release.")]
    public string GetText() => Text;

    /// Gets whether this text block is translatable.
    [Obsolete("Use the Translatable property instead. This property will be removed in a future release.")]
    public bool IsTranslatable() => Translatable;

    /// Returns a string representation of the text block.
    public override string ToString() => $"{{{Translatable}, {Text}}}";

    /// Determines whether the specified object is equal to the current text block.
    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj is not TextBlock textBlock) return false;
        return Text.Equals(textBlock.Text);
    }

    /// Returns a hash code for the current text block.
    public override int GetHashCode() => Text.GetHashCode();
}