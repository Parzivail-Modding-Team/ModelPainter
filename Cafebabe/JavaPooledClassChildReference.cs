using Cafebabe.Class;

namespace Cafebabe;

public record JavaPooledClassChildReference(short ClassRefPoolIdx, short NameTypePoolIdx)
{
	public static JavaPooledClassChildReference Read(BinaryReader r, ref int poolIdx) => new(r.ReadInt16(), r.ReadInt16());

	public JavaClassChildReference Bake(JavaConstantPool constantPool)
	{
		var classReference = constantPool.GetClassInfo(ClassRefPoolIdx);
		var nameAndType = constantPool.GetNameTypeInfo(NameTypePoolIdx);
		return new JavaClassChildReference(classReference, nameAndType.Bake(constantPool));
	}
}