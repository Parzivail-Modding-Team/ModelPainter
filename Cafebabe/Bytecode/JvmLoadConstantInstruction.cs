using Cafebabe.Class;
using Cafebabe.Vm;

namespace Cafebabe.Bytecode;

public record JvmLoadConstantInstruction(JvmOpcode Opcode, object Constant) : JvmInstruction(Opcode)
{
	public static JvmInstruction ReadNarrow(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadByte();
		return new JvmLoadConstantInstruction(opcode, ResolveConstant(constantpool, referenceIndex));
	}

	public static JvmInstruction ReadWide(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		var referenceIndex = r.ReadInt16();
		return new JvmLoadConstantInstruction(opcode, ResolveConstant(constantpool, referenceIndex));
	}

	private static object ResolveConstant(JavaConstantPool constantpool, int referenceIndex)
	{
		var constant = constantpool.Constants[referenceIndex];

		if (constant.Tag == JavaConstantPoolTag.String)
			constant = constantpool.Constants[(short)constant.Data];

		return constant.Data;
	}

	/// <inheritdoc />
	public override void Execute(SandboxedVirtualMachine vm)
	{
		JavaInstructions.LoadConstant(vm, Constant);
	}
}