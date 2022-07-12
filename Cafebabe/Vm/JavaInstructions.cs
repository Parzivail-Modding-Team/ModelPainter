namespace Cafebabe.Vm;

public class JavaInstructions
{
	public static void GetStatic(SandboxedVirtualMachine vm, JavaClassChildReference reference)
	{
		vm.OperandStack.Push(vm.ClassResolver.Resolve(reference.ClassReference).ResolveStaticField(reference.Descriptor));
		vm.ProgramCounter++;
	}

	public static void LoadConstant(SandboxedVirtualMachine vm, object constant)
	{
		vm.OperandStack.Push(constant);
		vm.ProgramCounter++;
	}

	public static void InvokeVirtual(SandboxedVirtualMachine vm, JavaClassChildReference reference)
	{
		var (returnType, parameterTypes) = JavaUtils.DeconstructMethodDescriptor(reference.Descriptor.Type);
		var paramValues = new object[parameterTypes.Length];

		for (var i = paramValues.Length - 1; i >= 0; i--)
			paramValues[i] = vm.OperandStack.Pop();

		var targetObject = vm.OperandStack.Pop();

		var impl = vm.ClassResolver.Resolve(reference.ClassReference);
		if (impl == null)
			throw new InvalidOperationException($"Unable to resolve class: {reference}");

		var method = impl.ResolveMethod(reference.Descriptor);
		if (method == null)
			throw new InvalidOperationException($"Unable to resolve method: {reference}");

		method.Invoke(vm, targetObject, paramValues);
		vm.ProgramCounter++;
	}
}