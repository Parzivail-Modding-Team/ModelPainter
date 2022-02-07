using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaLocalVariableTableAttributeInfo(string Name, JavaLocalVariableTableEntry[] Table) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var tableLength = br.ReadInt16();
		var table = new JavaLocalVariableTableEntry[tableLength];

		for (var i = 0; i < table.Length; i++)
		{
			var startPc = br.ReadInt16();
			var length = br.ReadInt16();

			var dummy = 0;
			var nameAndType = JavaPooledNameAndTypeDescriptor.Read(br, ref dummy);

			var index = br.ReadInt16();

			table[i] = new JavaLocalVariableTableEntry(startPc, length, nameAndType.Bake(constantPool), index);
		}

		return new JavaLocalVariableTableAttributeInfo(name, table);
	}
}