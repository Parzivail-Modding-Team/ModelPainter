using System.Text.Json.Serialization;

namespace MinecraftMappings.NET;

public record YarnVersion(
	[property: JsonPropertyName("gameVersion")] string GameVersion,
	[property: JsonPropertyName("separator")] string Separator,
	[property: JsonPropertyName("build")] int Build,
	[property: JsonPropertyName("maven")] string Maven,
	[property: JsonPropertyName("version")] string Version,
	[property: JsonPropertyName("stable")] bool Stable
);