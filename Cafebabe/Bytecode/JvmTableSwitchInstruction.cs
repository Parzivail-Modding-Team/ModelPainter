using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmTableSwitchInstruction(JvmOpcode Opcode, int DefaultBranch, int[] BranchOffsets) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		while (r.BaseStream.Position % 4 != 0)
			r.ReadByte(); // Padding byte

		var defaultBranch = r.ReadInt32();
		var lowBranch = r.ReadInt32();
		var highBranch = r.ReadInt32();

		var offsets = new int[highBranch - lowBranch + 1];
		for (var i = 0; i < offsets.Length; i++)
			offsets[i] = r.ReadInt32();

		return new JvmTableSwitchInstruction(opcode, defaultBranch, offsets);
	}
}