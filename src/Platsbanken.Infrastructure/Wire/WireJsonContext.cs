using System.Text.Json.Serialization;

namespace Platsbanken.Infrastructure.Wire;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(WireJobAd))]
[JsonSerializable(typeof(WireSearchResults))]
internal sealed partial class WireJsonContext : JsonSerializerContext;
