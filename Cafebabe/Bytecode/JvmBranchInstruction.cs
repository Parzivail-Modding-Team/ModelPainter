using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmBranchInstruction(JvmOpcode Opcode, int Destination) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmBranchInstruction(opcode, r.ReadInt16());
	}

	public static JvmInstruction ReadWide(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmBranchInstruction(opcode, r.ReadInt32());
	}
}