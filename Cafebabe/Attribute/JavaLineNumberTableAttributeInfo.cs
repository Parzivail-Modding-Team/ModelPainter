using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaLineNumberTableAttributeInfo(string Name, JavaLineNumberTableEntry[] Table) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var tableLength = br.ReadInt16();
		var table = new JavaLineNumberTableEntry[tableLength];

		for (var i = 0; i < table.Length; i++)
		{
			var startPc = br.ReadInt16();
			var lineNumber = br.ReadInt16();
			table[i] = new JavaLineNumberTableEntry(startPc, lineNumber);
		}

		return new JavaLineNumberTableAttributeInfo(name, table);
	}
}