using System.Text.Json.Serialization;

namespace MinecraftMappings.NET;

public record MinecraftJarfileVersion(
	string Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("release_target")] string ReleaseTarget,
	[property: JsonPropertyName("world_version")] int WorldVersion,
	[property: JsonPropertyName("series_id")] string SeriesId,
	[property: JsonPropertyName("protocol_version")] int ProtocolVersion,
	[property: JsonPropertyName("build_time")] DateTime BuildTime,
	[property: JsonPropertyName("java_component")] string JavaComponent,
	[property: JsonPropertyName("java_version")] int JavaVersion,
	[property: JsonPropertyName("stable")] bool Stable
) : MinecraftJarfileVersionId(Id);