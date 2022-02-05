using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmClassChildReferenceInstruction(JvmOpcode Opcode, JavaClassChildReference Reference) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var reference = (JavaPooledClassChildReference)constantpool.Constants[referenceIndex];
		return new JvmClassChildReferenceInstruction(opcode, reference.Bake(constantpool));
	}
}