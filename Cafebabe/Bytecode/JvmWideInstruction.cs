using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmWideInstruction(JvmOpcode Opcode, int DefaultBranch, int[] BranchOffsets) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var targetOpcode = (JvmOpcode)r.ReadByte();
		var reference = r.ReadInt16();

		if (targetOpcode == JvmOpcode.iinc)
			return new JvmLocalVarRefConstInstruction(opcode, reference, r.ReadInt16());

		return new JvmLocalVarRefInstruction(targetOpcode, reference);
	}
}