using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmLocalVarRefConstInstruction(JvmOpcode Opcode, short Reference, short Const) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmLocalVarRefConstInstruction(opcode, r.ReadByte(), r.ReadByte());
	}
}