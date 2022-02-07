using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaConstantValueAttributeInfo(string Name, JavaConstantPoolEntry Value) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var valueIdx = br.ReadInt16();
		return new JavaConstantValueAttributeInfo(name, constantPool.Constants[valueIdx]);
	}
}