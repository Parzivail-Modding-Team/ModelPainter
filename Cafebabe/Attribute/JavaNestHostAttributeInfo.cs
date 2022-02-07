using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaNestHostAttributeInfo(string Name, string HostClass) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var host = br.ReadInt16();
		return new JavaNestHostAttributeInfo(name, constantPool.GetClassInfo(host));
	}
}