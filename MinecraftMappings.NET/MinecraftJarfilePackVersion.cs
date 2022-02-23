using System.Text.Json.Serialization;

namespace MinecraftMappings.NET;

public record MinecraftJarfilePackVersion(
	[property: JsonPropertyName("resource")] int Resource,
	[property: JsonPropertyName("data")] int Data
);