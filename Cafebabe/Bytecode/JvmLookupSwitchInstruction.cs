using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmLookupSwitchInstruction(JvmOpcode Opcode, int DefaultBranch, Dictionary<int, int> BranchOffsets) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		while (r.BaseStream.Position % 4 != 0)
			r.ReadByte(); // Padding byte

		var defaultBranch = r.ReadInt32();
		var numPairs = r.ReadInt32();

		var offsets = new Dictionary<int, int>();
		for (var i = 0; i < numPairs; i++)
		{
			var key = r.ReadInt32();
			var value = r.ReadInt32();
			offsets[key] = value;
		}

		return new JvmLookupSwitchInstruction(opcode, defaultBranch, offsets);
	}
}