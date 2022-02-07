using Cafebabe.Attribute;
using Cafebabe.Field;
using Cafebabe.Method;

namespace Cafebabe.Class;

public record JavaClassFile(short MajorVersion, short MinorVersion, JavaClassAccessFlag AccessFlags, string ThisClass, string SuperClass, string[] Interfaces, JavaFieldInfo[] Fields,
	JavaMethodInfo[] Methods, JavaAttributeInfo[] Attributes)
{
	private const uint ClassFileMagic = 0xCAFEBABE;

	public static JavaClassFile Read(Stream stream)
	{
		var br = new EndiannessAwareBinaryReader(stream, EndiannessAwareBinaryReader.Endianness.Big);

		var magic = br.ReadUInt32();
		if (magic != ClassFileMagic)
			throw new InvalidDataException($"Expected class magic 0x{ClassFileMagic:X8}, got 0x{magic:X8}");

		var minorVersion = br.ReadInt16();
		var majorVersion = br.ReadInt16();

		var constantPool = JavaConstantPool.Read(br);

		var accessFlags = (JavaClassAccessFlag)br.ReadUInt16();

		var thisClass = constantPool.GetClassInfo(br.ReadInt16());
		var superClass = constantPool.GetClassInfo(br.ReadInt16());

		var numInterfaces = br.ReadInt16();
		var interfaces = new string[numInterfaces];
		for (var i = 0; i < interfaces.Length; i++)
			interfaces[i] = constantPool.GetClassInfo(br.ReadInt16());

		var numFields = br.ReadInt16();
		var fields = new JavaFieldInfo[numFields];
		for (var i = 0; i < fields.Length; i++)
			fields[i] = JavaFieldInfo.Read(constantPool, br);

		var numMethods = br.ReadInt16();
		var methods = new JavaMethodInfo[numMethods];
		for (var i = 0; i < methods.Length; i++)
			methods[i] = JavaMethodInfo.Read(constantPool, br);

		var numAttributes = br.ReadInt16();
		var attributes = new JavaAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaAttributeInfo.Read(constantPool, br);

		return new JavaClassFile(majorVersion, minorVersion, accessFlags, thisClass, superClass, interfaces, fields, methods, attributes);
	}
}