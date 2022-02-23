using System.IO.Compression;
using System.Text.Json;

namespace MinecraftMappings.NET;

public class MinecraftJarfileHelper
{
	public static MinecraftJarfileVersion GetVersion(string filename)
	{
		using var srcZip = ZipFile.OpenRead(filename);
		var metadataFile = srcZip.GetEntry("version.json");
		if (metadataFile == null)
			throw new InvalidDataException("Archive does not contain version.json");

		using var reader = new StreamReader(metadataFile.Open());
		var json = reader.ReadToEnd();

		var versionId = JsonSerializer.Deserialize<MinecraftJarfileVersionId>(json);
		// TODO: reject or compensate for versions which have a different version.json structure, if any

		return JsonSerializer.Deserialize<MinecraftJarfileVersion>(json) ?? throw new InvalidOperationException("JSON data returned from server did not result in a Yarn version array");
	}
}