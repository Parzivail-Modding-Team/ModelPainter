using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmClassChildReferenceCountInstruction(JvmOpcode Opcode, JavaClassChildReference Reference, byte Count, byte Extra) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var reference = (JavaPooledClassChildReference)constantpool.Constants[referenceIndex];
		return new JvmClassChildReferenceCountInstruction(opcode, reference.Bake(constantpool), r.ReadByte(), r.ReadByte());
	}
}