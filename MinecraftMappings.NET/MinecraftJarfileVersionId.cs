using System.Text.Json.Serialization;

namespace MinecraftMappings.NET;

public record MinecraftJarfileVersionId(
	[property: JsonPropertyName("id")] string Id
);