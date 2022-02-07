using Cafebabe.Class;

namespace Cafebabe;

public record JavaPooledDynamicReference(short BootstrapMethodAttrIdx, short NameTypePoolIdx)
{
	public static JavaPooledDynamicReference Read(BinaryReader r, ref int poolIdx) => new(r.ReadInt16(), r.ReadInt16());

	public JavaDynamicReference Bake(JavaConstantPool constantPool)
	{
		var nameAndType = constantPool.GetNameTypeInfo(NameTypePoolIdx);
		return new JavaDynamicReference(BootstrapMethodAttrIdx, nameAndType.Bake(constantPool));
	}
}