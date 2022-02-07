using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaAnnotationDefaultAttributeInfo(string Name, JavaAnnotationElementValue Value) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var value = JavaAnnotationElementValue.Read(constantPool, br);
		return new JavaAnnotationDefaultAttributeInfo(name, value);
	}
}