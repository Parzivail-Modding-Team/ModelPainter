using Cafebabe.Attribute;
using Cafebabe.Class;

namespace Cafebabe.Method;

public record JavaMethodInfo(JavaMethodAccessFlag AccessFlags, string Name, string Descriptor, JavaAttributeInfo[] Attributes)
{
	public static JavaMethodInfo Read(JavaConstantPool constantPool, BinaryReader r)
	{
		var flags = r.ReadUInt16();
		var nameIdx = r.ReadInt16();
		var descIdx = r.ReadInt16();

		var numAttributes = r.ReadInt16();
		var attributes = new JavaAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaAttributeInfo.Read(constantPool, r);

		var name = constantPool.GetString(nameIdx);
		var descriptor = constantPool.GetString(descIdx);
		return new JavaMethodInfo((JavaMethodAccessFlag)flags, name, descriptor, attributes);
	}
}