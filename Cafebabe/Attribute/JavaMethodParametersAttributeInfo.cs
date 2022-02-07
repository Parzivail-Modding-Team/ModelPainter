using Cafebabe.Class;
using Cafebabe.Method;

namespace Cafebabe.Attribute;

public record JavaMethodParametersAttributeInfo(string Name, JavaMethodParameterEntry[] Entries) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numEntries = br.ReadByte();
		var entries = new JavaMethodParameterEntry[numEntries];

		for (var i = 0; i < entries.Length; i++)
		{
			var nameIdx = br.ReadInt16();
			var flags = br.ReadInt16();
			entries[i] = new JavaMethodParameterEntry(constantPool.GetString(nameIdx), (JavaMethodParameterAccessFlag)flags);
		}

		return new JavaMethodParametersAttributeInfo(name, entries);
	}
}