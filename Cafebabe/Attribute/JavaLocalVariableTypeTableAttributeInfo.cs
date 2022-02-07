using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaLocalVariableTypeTableAttributeInfo(string Name, JavaLocalVariableTypeEntry[] Entries) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numEntries = br.ReadInt16();
		var entries = new JavaLocalVariableTypeEntry[numEntries];

		for (var i = 0; i < entries.Length; i++)
		{
			var startPc = br.ReadInt16();
			var length = br.ReadInt16();
			var nameIdx = br.ReadInt16();
			var sigIdx = br.ReadInt16();
			var index = br.ReadInt16();
			entries[i] = new JavaLocalVariableTypeEntry(startPc, length, constantPool.GetString(nameIdx), constantPool.GetString(sigIdx), index);
		}

		return new JavaLocalVariableTypeTableAttributeInfo(name, entries);
	}
}