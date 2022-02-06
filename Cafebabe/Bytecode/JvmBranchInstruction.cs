using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmBranchInstruction(JvmOpcode Opcode, short Destination) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmBranchInstruction(opcode, r.ReadInt16());
	}
}