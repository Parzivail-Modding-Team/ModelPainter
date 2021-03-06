using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmClassReferenceInstruction(JvmOpcode Opcode, string Reference) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var reference = constantpool.GetClassInfo(referenceIndex);
		return new JvmClassReferenceInstruction(opcode, reference);
	}
}