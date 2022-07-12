using System.Reflection;
using Cafebabe.Attribute;
using Cafebabe.Class;
using Cafebabe.Method;

namespace Cafebabe.Vm;

public class SandboxedVirtualMachine
{
	public IClassResolver ClassResolver { get; }
	public short ProgramCounter { get; set; }
	public Stack<object> OperandStack { get; } = new();

	public SandboxedVirtualMachine(IClassResolver classResolver)
	{
		ClassResolver = classResolver;
	}

	public void Execute(JavaClassFile clazz, JavaMethodInfo method)
	{
		var codeAttributes = method.Attributes.Where(info => info is JavaCodeAttributeInfo).ToArray();
		if (codeAttributes.Length != 1)
			throw new ArgumentException($"Method should have exactly one JavaCodeAttributeInfo, found {codeAttributes.Length}", nameof(method));

		var codeAttribute = (JavaCodeAttributeInfo)codeAttributes[0];

		while (true)
		{
			var instr = codeAttribute.Instructions[ProgramCounter];
			Console.WriteLine(instr.Opcode);
			instr.Execute(this);
		}
	}
}

public interface IClassResolver
{
	ClassImplementation Resolve(string reference);
}

[AttributeUsage(System.AttributeTargets.Field)]
public class JavaFieldNameAttribute : System.Attribute
{
	public string Name { get; }

	public JavaFieldNameAttribute(string name)
	{
		Name = name;
	}
}

[AttributeUsage(System.AttributeTargets.Method)]
public class JavaMethodNameAttribute : System.Attribute
{
	public string Name { get; }
	public string Type { get; }

	public JavaMethodNameAttribute(string name, string type)
	{
		Name = name;
		Type = type;
	}
}

public abstract class ClassImplementation
{
	public JavaDescriptor Type { get; }

	public ClassImplementation(JavaDescriptor type)
	{
		Type = type;
	}

	public virtual MethodImplementation ResolveMethod(JavaNameAndTypeDescriptor reference)
	{
		var method = GetType()
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.First(info => IsMethodReferenceMatch(info, reference));

		return (vm, targetObject, values) => method.Invoke(null, new[] { vm, targetObject, values });
	}

	private static bool IsMethodReferenceMatch(MethodInfo info, JavaNameAndTypeDescriptor reference)
	{
		var attr = info.GetCustomAttribute<JavaMethodNameAttribute>();
		if (attr == null)
			return false;

		return attr.Name == reference.Name && attr.Type == reference.Type;
	}

	public virtual object ResolveStaticField(JavaNameAndTypeDescriptor reference)
	{
		return GetType()
			.GetFields(BindingFlags.Public | BindingFlags.Static)
			.First(info => info.GetCustomAttribute<JavaFieldNameAttribute>()?.Name == reference.Name)
			.GetValue(null);
	}
}

public delegate void MethodImplementation(SandboxedVirtualMachine vm, object targetObject, object[] paramValues);