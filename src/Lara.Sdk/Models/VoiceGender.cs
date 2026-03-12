using System.Text.Json.Serialization;

namespace Lara.Sdk;

public enum VoiceGender
{
    /// Male voice for audio translation output.
    [JsonPropertyName("male")]
    Male,

    /// Female voice for audio translation output.
    [JsonPropertyName("female")]
    Female
}
