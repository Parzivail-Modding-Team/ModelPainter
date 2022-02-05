using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmClassReferenceInstruction(JvmOpcode Opcode, string Reference) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var reference = (string)constantpool.Constants[(short)constantpool.Constants[referenceIndex]];
		return new JvmClassReferenceInstruction(opcode, reference);
	}
}