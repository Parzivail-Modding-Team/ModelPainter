using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmBipushInstruction(JvmOpcode Opcode, byte Value) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmBipushInstruction(opcode, r.ReadByte());
	}
}