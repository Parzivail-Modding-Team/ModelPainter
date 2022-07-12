using System.IO.Compression;
using Cafebabe;
using Cafebabe.Class;
using Cafebabe.Vm;

namespace Sandbox;

public class Program
{
	private const string YarnModelClass = "net/minecraft/class_3879";

	public static void Main(string[] args)
	{
		Console.WriteLine("Getting jarfile version...");
		const string jarfile = @"E:\colby\Desktop\temp\java\TestProgram.jar";
		// var minecraftVersion = MinecraftJarfileHelper.GetVersion(jarfile);
		//
		// Console.WriteLine("Getting remote yarn versions...");
		// var remoteYarnVersions = YarnHelper.GetRemoteVersions().ConfigureAwait(false).GetAwaiter().GetResult();
		// Console.WriteLine("Finding compatible yarn versions...");
		// var compatibleYarnVersions = remoteYarnVersions
		// 	.Where(yarnVersion => yarnVersion.GameVersion == minecraftVersion.Name)
		// 	.OrderByDescending(yarnVersion => yarnVersion.Build)
		// 	.ToArray();
		// if (compatibleYarnVersions.Length == 0)
		// 	throw new InvalidOperationException("No compatible yarn mapping found");
		//
		// Console.WriteLine("Parsing mappings...");
		// var yarnMapping = compatibleYarnVersions[0];
		// var mappings = YarnHelper.GetYarnMappings(yarnMapping).ConfigureAwait(false).GetAwaiter().GetResult();
		// Console.WriteLine("Creating mapper...");
		//
		// var intermediaryClassMapper = mappings.CreateIntermediaryClassMapper();
		//
		// var baseModelClassNames = new HashSet<string>
		// {
		// 	intermediaryClassMapper[YarnModelClass].Official, // Obfuscated official
		// 	YarnModelClass, // Yarn intermediary
		// 	"net/minecraft/client/model/Model", // Fabric
		// 	"net/minecraft/client/model/ModelBase",
		// };
		//
		// Console.WriteLine("Iterating jarfile...");

		using var zip = ZipFile.Open(jarfile, ZipArchiveMode.Read);
		var classes = new Dictionary<string, JavaClassFile>();

		var vm = new SandboxedVirtualMachine(new TestClassResolver());

		foreach (var entry in zip.Entries)
		{
			if (Path.GetExtension(entry.Name) != ".class") continue;

			using var fs = entry.Open();
			var c = JavaClassFile.Read(fs);

			vm.Execute(c, c.Methods.First(info => info.Name == "main"));

			classes[c.ThisClass] = c;
		}

		// var officialMapper = mappings.CreateOfficialMapper();
		//
		// foreach (var (thisClass, c) in classes)
		// {
		// 	if (IsModel(baseModelClassNames, classes, thisClass))
		// 		Console.WriteLine($"{officialMapper.Classes[thisClass].Mapped} extends {officialMapper.Classes[c.SuperClass].Mapped}");
		// }
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

public class TestClassResolver : IClassResolver
{
	private readonly Dictionary<string, ClassImplementation> _implementations = new()
	{
		[JavaLangSystem.Name] = new JavaLangSystem(),
		[JavaLangString.Name] = new JavaLangString(),
		[JavaIoPrintStream.Name] = new JavaIoPrintStream()
	};

	/// <inheritdoc />
	public ClassImplementation Resolve(string reference)
	{
		return _implementations[reference];
	}
}

public class JavaLangSystem : ClassImplementation
{
	public const string Name = "java/lang/System";

	[JavaFieldName("out")] public static readonly TextWriter Out = Console.Out;

	/// <inheritdoc />
	public JavaLangSystem() : base(new JavaDescriptor(DescriptorType.ClassReference, Name))
	{
	}
}

public class JavaLangString : ClassImplementation
{
	public const string Name = "java/lang/String";

	/// <inheritdoc />
	public JavaLangString() : base(new JavaDescriptor(DescriptorType.ClassReference, Name))
	{
	}

	[JavaMethodName("length", "()I")]
	public static void StrLength(SandboxedVirtualMachine vm, object targetObject, object[] paramValues)
	{
		vm.OperandStack.Push(((string)targetObject).Length);
	}
}

internal class JavaIoPrintStream : ClassImplementation
{
	public const string Name = "java/io/PrintStream";

	/// <inheritdoc />
	public JavaIoPrintStream() : base(new JavaDescriptor(DescriptorType.ClassReference, Name))
	{
	}

	[JavaMethodName("println", "(Ljava/lang/String;)V")]
	public static void PrintlnStr(SandboxedVirtualMachine vm, object targetObject, object[] paramValues)
	{
		((TextWriter)targetObject).WriteLine(paramValues[0]);
	}

	[JavaMethodName("println", "(I)V")]
	public static void PrintlnInt(SandboxedVirtualMachine vm, object targetObject, object[] paramValues)
	{
		((TextWriter)targetObject).WriteLine(paramValues[0]);
	}
}