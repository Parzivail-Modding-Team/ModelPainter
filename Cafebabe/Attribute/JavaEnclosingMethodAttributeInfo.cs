using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaEnclosingMethodAttributeInfo(string Name, string Class, JavaNameAndTypeDescriptor Method) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var classIndex = br.ReadInt16();
		var methodIndex = br.ReadInt16();
		return new JavaEnclosingMethodAttributeInfo(name, constantPool.GetClassInfo(classIndex), constantPool.GetNameTypeInfo(methodIndex)?.Bake(constantPool));
	}
}