using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmPrimitiveReferenceInstruction(JvmOpcode Opcode, JavaPrimitiveType PrimitiveType) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var reference = r.ReadByte();
		return new JvmPrimitiveReferenceInstruction(opcode, (JavaPrimitiveType)reference);
	}
}