using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaClassListAttributeInfo(string Name, string[] Classes) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numClasses = br.ReadInt16();
		var classes = new string[numClasses];

		for (var i = 0; i < classes.Length; i++)
		{
			var classIdx = br.ReadInt16();
			classes[i] = constantPool.GetClassInfo(classIdx);
		}

		return new JavaClassListAttributeInfo(name, classes);
	}
}