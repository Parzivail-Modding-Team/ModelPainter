using Cafebabe.Attribute;
using Cafebabe.Class;

namespace Cafebabe.Method;

public record JavaPooledMethodInfo(JavaMethodAccessFlag AccessFlags, short NamePoolIdx, short DescriptorPoolIdx, JavaPooledAttributeInfo[] Attributes)
{
	public static JavaPooledMethodInfo Read(BinaryReader r)
	{
		var flags = r.ReadUInt16();
		var nameIdx = r.ReadInt16();
		var descIdx = r.ReadInt16();

		var numAttributes = r.ReadInt16();
		var attributes = new JavaPooledAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaPooledAttributeInfo.Read(r);

		return new JavaPooledMethodInfo((JavaMethodAccessFlag)flags, nameIdx, descIdx, attributes);
	}

	public JavaMethodInfo Bake(JavaConstantPool constantPool)
	{
		var name = (string)constantPool.Constants[NamePoolIdx];
		var descriptor = (string)constantPool.Constants[DescriptorPoolIdx];
		var attributes = Attributes.Select(info => info.Bake(constantPool)).ToArray();
		return new JavaMethodInfo(AccessFlags, name, descriptor, attributes);
	}
}