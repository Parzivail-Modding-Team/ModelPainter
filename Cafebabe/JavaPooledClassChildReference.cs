using Cafebabe.Class;

namespace Cafebabe;

public record JavaPooledClassChildReference(short ClassRefPoolIdx, short NameTypePoolIdx)
{
	public static JavaPooledClassChildReference Read(BinaryReader r, ref int poolIdx) => new(r.ReadInt16(), r.ReadInt16());

	public JavaClassChildReference Bake(JavaConstantPool constantPool)
	{
		var classReference = (string)constantPool.Constants[(short)constantPool.Constants[ClassRefPoolIdx]];
		var nameAndType = (JavaPooledNameAndTypeDescriptor)constantPool.Constants[NameTypePoolIdx];
		return new JavaClassChildReference(classReference, nameAndType.Bake(constantPool));
	}
}