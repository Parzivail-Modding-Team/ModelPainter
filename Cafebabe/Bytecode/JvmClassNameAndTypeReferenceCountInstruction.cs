using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmClassNameAndTypeReferenceCountInstruction(JvmOpcode Opcode, JavaDynamicReference Reference, byte Count, byte Extra) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var reference = constantpool.GetClassDynamicRef(referenceIndex);
		return new JvmClassNameAndTypeReferenceCountInstruction(opcode, reference.Bake(constantpool), r.ReadByte(), r.ReadByte());
	}
}