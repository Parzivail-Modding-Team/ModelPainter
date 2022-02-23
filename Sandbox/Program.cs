using System.IO.Compression;
using Cafebabe.Class;
using MinecraftMappings.NET;

namespace Sandbox;

public class Program
{
	private const string YarnModelClass = "net/minecraft/class_3879";

	public static void Main(string[] args)
	{
		Console.WriteLine("Getting jarfile version...");
		const string jarfile = @"R:\MultiMC\libraries\com\mojang\minecraft\1.18.1\minecraft-1.18.1-client.jar";
		var minecraftVersion = MinecraftJarfileHelper.GetVersion(jarfile);

		Console.WriteLine("Getting remote yarn versions...");
		var remoteYarnVersions = YarnHelper.GetRemoteVersions().ConfigureAwait(false).GetAwaiter().GetResult();
		Console.WriteLine("Finding compatible yarn versions...");
		var compatibleYarnVersions = remoteYarnVersions
			.Where(yarnVersion => yarnVersion.GameVersion == minecraftVersion.Name)
			.OrderByDescending(yarnVersion => yarnVersion.Build)
			.ToArray();
		if (compatibleYarnVersions.Length == 0)
			throw new InvalidOperationException("No compatible yarn mapping found");

		Console.WriteLine("Parsing mappings...");
		var yarnMapping = compatibleYarnVersions[0];
		var mappings = YarnHelper.GetYarnMappings(yarnMapping).ConfigureAwait(false).GetAwaiter().GetResult();
		Console.WriteLine("Creating mapper...");

		var intermediaryClassMapper = mappings.CreateIntermediaryClassMapper();

		var baseModelClassNames = new HashSet<string>
		{
			intermediaryClassMapper[YarnModelClass].Official, // Obfuscated official
			YarnModelClass, // Yarn intermediary
			"net/minecraft/client/model/Model", // Fabric
			"net/minecraft/client/model/ModelBase",
		};

		Console.WriteLine("Iterating jarfile...");

		using var zip = ZipFile.Open(jarfile, ZipArchiveMode.Read);
		var classes = new Dictionary<string, JavaClassFile>();

		foreach (var entry in zip.Entries)
		{
			if (Path.GetExtension(entry.Name) != ".class") continue;

			using var fs = entry.Open();
			var c = JavaClassFile.Read(fs);

			classes[c.ThisClass] = c;
		}

		var officialMapper = mappings.CreateOfficialMapper();

		foreach (var (thisClass, c) in classes)
		{
			if (IsModel(baseModelClassNames, classes, thisClass))
				Console.WriteLine($"{officialMapper.Classes[thisClass].Mapped} extends {officialMapper.Classes[c.SuperClass].Mapped}");
		}
	}

	private static bool IsModel(HashSet<string> validModelBaseClasses, Dictionary<string, JavaClassFile> classes, string thisClass)
	{
		while (true)
		{
			if (!classes.TryGetValue(thisClass, out var c))
				return false;

			if (validModelBaseClasses.Contains(c.SuperClass))
				return true;

			thisClass = c.SuperClass;
		}
	}
}