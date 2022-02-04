using Cafebabe.Attribute;
using Cafebabe.Class;

namespace Cafebabe.Field;

public record JavaPooledFieldInfo(JavaFieldAccessFlag AccessFlags, short NamePoolIdx, short DescriptorPoolIdx, JavaPooledAttributeInfo[] Attributes)
{
	public static JavaPooledFieldInfo Read(BinaryReader r)
	{
		var flags = r.ReadUInt16();
		var nameIdx = r.ReadInt16();
		var descIdx = r.ReadInt16();

		var numAttributes = r.ReadInt16();
		var attributes = new JavaPooledAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaPooledAttributeInfo.Read(r);

		return new JavaPooledFieldInfo((JavaFieldAccessFlag)flags, nameIdx, descIdx, attributes);
	}

	public JavaFieldInfo Bake(JavaConstantPool constantPool)
	{
		var name = (string)constantPool.Constants[NamePoolIdx];
		var descriptor = (string)constantPool.Constants[DescriptorPoolIdx];
		var attributes = Attributes.Select(info => info.Bake(constantPool)).ToArray();
		return new JavaFieldInfo(AccessFlags, name, descriptor, attributes);
	}
}