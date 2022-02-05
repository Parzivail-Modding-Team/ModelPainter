using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmLoadConstantInstruction(JvmOpcode Opcode, object Constant) : JvmInstruction(Opcode)
{
	public static JvmInstruction ReadNarrow(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadByte();
		return new JvmLoadConstantInstruction(opcode, constantpool.Constants[referenceIndex]);
	}

	public static JvmInstruction ReadWide(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadByte();
		return new JvmLoadConstantInstruction(opcode, constantpool.Constants[referenceIndex]);
	}
}