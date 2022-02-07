using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaRecordAttributeInfo(string Name, JavaRecordComponentInfo[] Components) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numComponents = br.ReadInt16();
		var components = new JavaRecordComponentInfo[numComponents];

		for (var i = 0; i < components.Length; i++)
			components[i] = JavaRecordComponentInfo.Read(constantPool, br);

		return new JavaRecordAttributeInfo(name, components);
	}
}