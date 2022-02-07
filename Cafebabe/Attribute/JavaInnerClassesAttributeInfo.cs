using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaInnerClassesAttributeInfo(string Name, JavaInnerClassEntry[] Entries) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numEntries = br.ReadInt16();
		var entries = new JavaInnerClassEntry[numEntries];

		for (var i = 0; i < entries.Length; i++)
		{
			var innerInfoIdx = br.ReadInt16();
			var outerInfoIdx = br.ReadInt16();
			var innerNameIdx = br.ReadInt16();
			var flags = br.ReadInt16();
			entries[i] = new JavaInnerClassEntry(constantPool.GetClassInfo(innerInfoIdx), outerInfoIdx == 0 ? null : constantPool.GetClassInfo(outerInfoIdx), constantPool.GetString(innerNameIdx),
				(JavaInnerClassAccessFlag)flags);
		}

		return new JavaInnerClassesAttributeInfo(name, entries);
	}
}