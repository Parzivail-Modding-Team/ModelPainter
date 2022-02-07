using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmLocalVarRefInstruction(JvmOpcode Opcode, short Reference) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmBipushInstruction(opcode, r.ReadByte());
	}
}