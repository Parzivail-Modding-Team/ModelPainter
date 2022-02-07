using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaBootstrapMethodsAttributeInfo(string Name, JavaBootstrapMethodEntry[] Methods) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numBootstrapMethods = br.ReadInt16();
		var bootstrapMethods = new JavaBootstrapMethodEntry[numBootstrapMethods];

		for (var i = 0; i < bootstrapMethods.Length; i++)
		{
			var methodRef = br.ReadInt16();

			var numBootstrapArgs = br.ReadInt16();
			var bootstrapArgs = new JavaConstantPoolEntry[numBootstrapArgs];

			for (var j = 0; j < bootstrapArgs.Length; j++)
			{
				var argIdx = br.ReadInt16();
				bootstrapArgs[j] = constantPool.Constants[argIdx];
			}

			bootstrapMethods[i] = new JavaBootstrapMethodEntry(constantPool.GetMethodHandle(methodRef), bootstrapArgs);
		}

		return new JavaBootstrapMethodsAttributeInfo(name, bootstrapMethods);
	}
}