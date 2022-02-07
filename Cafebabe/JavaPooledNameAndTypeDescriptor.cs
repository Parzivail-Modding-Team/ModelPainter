using Cafebabe.Class;

namespace Cafebabe;

public record JavaPooledNameAndTypeDescriptor(short NamePoolIdx, short TypeDescriptor)
{
	public static JavaPooledNameAndTypeDescriptor Read(BinaryReader r, ref int poolIdx) => new(r.ReadInt16(), r.ReadInt16());

	public JavaNameAndTypeDescriptor Bake(JavaConstantPool constantPool)
	{
		var name = constantPool.GetString(NamePoolIdx);
		var type = constantPool.GetString(TypeDescriptor);
		return new JavaNameAndTypeDescriptor(name, type);
	}
}