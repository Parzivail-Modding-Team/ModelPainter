using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaSourceFileAttributeInfo(string Name, string SourceFileName) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var sourceFileIndex = br.ReadInt16();
		return new JavaSourceFileAttributeInfo(name, constantPool.GetString(sourceFileIndex));
	}
}