using System.Text.Json.Serialization;

namespace Lara.Sdk;

public enum ProfanityFilter
{
  [JsonPropertyName("detect")]
  Detect,

  [JsonPropertyName("avoid")]
  Avoid,

  [JsonPropertyName("hide")]
  Hide
}