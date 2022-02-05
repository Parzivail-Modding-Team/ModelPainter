using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmIndexedInstruction(JvmOpcode Opcode, short Index) : JvmInstruction(Opcode)
{
	public static JvmInstruction ReadNarrow(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var index = r.ReadByte();
		return new JvmIndexedInstruction(opcode, index);
	}
}