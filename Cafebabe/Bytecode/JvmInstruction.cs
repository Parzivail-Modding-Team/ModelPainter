using Cafebabe.Vm;

namespace Cafebabe.Bytecode;

public record JvmInstruction(JvmOpcode Opcode)
{
	public virtual void Execute(SandboxedVirtualMachine vm)
	{
		throw new InvalidOperationException();
	}
}