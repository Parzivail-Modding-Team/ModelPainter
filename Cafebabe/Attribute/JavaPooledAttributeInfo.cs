using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaPooledAttributeInfo(short NamePoolIdx, byte[] Data)
{
	private delegate JavaAttributeInfo JavaAttributeInfoBaker(JavaConstantPool constantPool, string name, byte[] data);

	private static readonly Dictionary<string, JavaAttributeInfoBaker> Bakery = new()
	{
		["SourceFile"] = JavaSourceFileAttributeInfo.Read,
		["LineNumberTable"] = JavaLineNumberTableAttributeInfo.Read,
		["Code"] = JavaCodeAttributeInfo.Read,
	};

	public static JavaPooledAttributeInfo Read(BinaryReader r)
	{
		var nameIdx = r.ReadInt16();

		var dataLength = r.ReadInt32();
		var data = r.ReadBytes(dataLength);

		return new JavaPooledAttributeInfo(nameIdx, data);
	}

	public JavaAttributeInfo Bake(JavaConstantPool constantPool)
	{
		// TODO: decode Data: https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7-300
		var name = (string)constantPool.Constants[NamePoolIdx];

		if (Bakery.TryGetValue(name, out var baker))
			return baker.Invoke(constantPool, name, Data);

		Console.WriteLine($"No baker for attribute \"{name}\"");
		return new JavaAttributeInfo(name);
	}
}