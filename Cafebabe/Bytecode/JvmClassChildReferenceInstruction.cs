using Cafebabe.Class;
using Cafebabe.Vm;

namespace Cafebabe.Bytecode;

public record JvmClassChildReferenceInstruction(JvmOpcode Opcode, JavaClassChildReference Reference) : JvmInstruction(Opcode)
{
	public static JvmInstruction Read(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		var reference = constantpool.GetClassChildRef(referenceIndex);
		return new JvmClassChildReferenceInstruction(opcode, reference.Bake(constantpool));
	}

	/// <inheritdoc />
	public override void Execute(SandboxedVirtualMachine vm)
	{
		switch (Opcode)
		{
			case JvmOpcode.getfield:
				break;
			case JvmOpcode.getstatic:
				JavaInstructions.GetStatic(vm, Reference);
				break;
			case JvmOpcode.putfield:
				break;
			case JvmOpcode.putstatic:
				break;
			case JvmOpcode.invokespecial:
				break;
			case JvmOpcode.invokestatic:
				break;
			case JvmOpcode.invokevirtual:
				JavaInstructions.InvokeVirtual(vm, Reference);
				break;
			default:
				throw new InvalidOperationException("Invalid opcode/instruction type combination");
		}
	}
}