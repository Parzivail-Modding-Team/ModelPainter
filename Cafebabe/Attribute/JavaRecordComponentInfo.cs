using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaRecordComponentInfo(string Name, string Descriptor, JavaAttributeInfo[] Attributes)
{
	public static JavaRecordComponentInfo Read(JavaConstantPool constantPool, BinaryReader r)
	{
		var name = constantPool.GetString(r.ReadInt16());
		var descriptor = constantPool.GetString(r.ReadInt16());

		var numAttributes = r.ReadInt16();
		var attributes = new JavaAttributeInfo[numAttributes];

		for (var j = 0; j < attributes.Length; j++)
			attributes[j] = JavaAttributeInfo.Read(constantPool, r);

		return new JavaRecordComponentInfo(name, descriptor, attributes);
	}
}