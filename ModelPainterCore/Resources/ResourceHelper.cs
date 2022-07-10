using System.Reflection;

namespace ModelPainterCore.Resources;

public static class ResourceHelper
{
	public static Stream GetLocalResource(string filename)
	{
		return GetResource(typeof(ResourceHelper), "ModelPainterCore.Resources", filename);
	}

	public static string GetLocalStringResource(string filename)
	{
		using var sr = new StreamReader(GetLocalResource(filename));
		return sr.ReadToEnd();
	}

	public static Stream GetResource(Type referenceType, string domain, string filename)
	{
		var assembly = Assembly.GetAssembly(referenceType) ?? Assembly.GetExecutingAssembly();
		return assembly.GetManifestResourceStream($"{domain}.{filename}");
	}
}