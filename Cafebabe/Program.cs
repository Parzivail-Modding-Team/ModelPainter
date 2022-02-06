using Cafebabe.Class;

namespace Cafebabe;

public class Program
{
	public static void Main(string[] args)
	{
		foreach (var classFile in Directory.GetFiles(@"E:\Forge\MC 1.18.1", "*.class", SearchOption.AllDirectories))
		{
			Console.WriteLine(classFile);
			using var fs = new FileStream(classFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			var c = JavaClassFile.Read(fs);
		}
	}
}