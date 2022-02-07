using Cafebabe.Class;

namespace Cafebabe;

public class Program
{
	public static void Main(string[] args)
	{
		// foreach (var classFile in Directory.GetFiles(@"E:\Forge\MC 1.18.1", "*.class", SearchOption.AllDirectories))
		foreach (var classFile in Directory.GetFiles(@"E:\Forge\Mods\PSWG\PSWG15\build\libs\pswg-0.0.59+1.18.1", "*.class", SearchOption.AllDirectories))
		{
			Console.WriteLine(classFile);
			using var fs = new FileStream(classFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			var c = JavaClassFile.Read(fs);
		}
	}
}