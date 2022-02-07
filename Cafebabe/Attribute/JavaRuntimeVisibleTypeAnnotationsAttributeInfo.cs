using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaRuntimeVisibleTypeAnnotationsAttributeInfo(string Name) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		// TODO
		return new JavaRuntimeVisibleTypeAnnotationsAttributeInfo(name);
	}
}