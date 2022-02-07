using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmSipushInstruction(JvmOpcode Opcode, short Value) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmSipushInstruction(opcode, r.ReadInt16());
	}
}