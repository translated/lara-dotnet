using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Represents a text block with translatability flag.
public class TextBlock
{
    /// Gets the text content of the block.
    [JsonPropertyName("text")]
    public string Text { get; }

    /// Gets a value indicating whether this text block is translatable.
    [JsonPropertyName("translatable")]
    public bool Translatable { get; }

    /// Initializes a new instance with text only (defaults to translatable).
    /// <param name="text">The text content.</param>
    public TextBlock(string text) : this(text, true)
    {
    }

    /// Initializes a new instance with text and translatability flag.
    public TextBlock(string text, bool translatable)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Translatable = translatable;
    }

    /// Gets the text content of the block.
    public string GetText() => Text;

    /// Gets whether this text block is translatable.
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