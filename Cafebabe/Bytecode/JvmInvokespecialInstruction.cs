using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public record JvmInvokespecialInstruction(JvmOpcode Opcode, JavaClassChildReference MethodReference) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var methodReferenceIndex = r.ReadInt16();
		var methodReference = (JavaPooledClassChildReference)constantpool.Constants[methodReferenceIndex];
		return new JvmInvokespecialInstruction(opcode, methodReference.Bake(constantpool));
	}
}