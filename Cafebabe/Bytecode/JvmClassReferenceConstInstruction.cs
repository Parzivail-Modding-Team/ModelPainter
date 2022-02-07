using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmClassReferenceConstInstruction(JvmOpcode Opcode, string Reference, byte Const) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var constant = r.ReadByte();
		var reference = constantpool.GetClassInfo(referenceIndex);
		return new JvmClassReferenceConstInstruction(opcode, reference, constant);
	}
}