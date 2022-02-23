using System.IO.Compression;
using System.Text.Json;

namespace MinecraftMappings.NET;

public class YarnHelper
{
	public static async Task<YarnVersion[]> GetRemoteVersions()
	{
		using var http = new HttpClient();
		var json = await http.GetStringAsync("https://meta.fabricmc.net/v2/versions/yarn");

		return JsonSerializer.Deserialize<YarnVersion[]>(json) ?? throw new InvalidOperationException("JSON data returned from server did not result in a Yarn version array");
	}


	public static async Task<MappingSet> GetYarnMappings(YarnVersion version)
	{
		var mappingDescriptions = new List<string>();

		var mappingsTarball = $"https://maven.fabricmc.net/net/fabricmc/yarn/{version.Version}/yarn-{version.Version}-tiny.gz";
		using var http = new HttpClient();
		await using var gzStream = new GZipStream(await http.GetStreamAsync(mappingsTarball), CompressionMode.Decompress);
		var sr = new StreamReader(gzStream);

		// Skip header
		await sr.ReadLineAsync();

		while (await sr.ReadLineAsync() is { } line)
			mappingDescriptions.Add(line);

		return ParseMappings(mappingDescriptions);
	}

	private static MappingSet ParseMappings(IEnumerable<string> mappingDescriptions)
	{
		var classes = new List<ClassMapping>();
		var fields = new List<ClassMemberMapping>();
		var methods = new List<ClassMemberMapping>();

		foreach (var mapping in mappingDescriptions)
		{
			var columns = mapping.Split('\t');

			switch (columns[0])
			{
				case "CLASS":
					// CLASS <tab> officialName <tab> intermediaryName <tab> mappedName
					classes.Add(new ClassMapping(columns[1], columns[2], columns[3]));
					break;
				case "FIELD":
					// FIELD <tab> officialNameOfParent <tab> typeSignature <tab> officialName <tab> intermediaryName <tab> mappedName
					fields.Add(new ClassMemberMapping(columns[1], columns[3], columns[2], columns[4], columns[5]));
					break;
				case "METHOD":
					// METHOD <tab> officialNameOfParent <tab> methodSignature <tab> officialName <tab> intermediaryName <tab> mappedName
					methods.Add(new ClassMemberMapping(columns[1], columns[3], columns[2], columns[4], columns[5]));
					break;
			}
		}

		return new MappingSet(classes.ToArray(), methods.ToArray(), fields.ToArray());
	}
}