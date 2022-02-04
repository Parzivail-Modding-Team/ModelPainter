using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaSourceFileAttributeInfo(string Name, string SourceFileName) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = new EndiannessAwareBinaryReader(new MemoryStream(data), EndiannessAwareBinaryReader.Endianness.Big);

		var sourceFileIndex = br.ReadInt16();

		return new JavaSourceFileAttributeInfo(name, (string)constantPool.Constants[sourceFileIndex]);
	}
}