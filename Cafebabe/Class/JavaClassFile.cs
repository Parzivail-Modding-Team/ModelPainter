using Cafebabe.Attribute;
using Cafebabe.Field;
using Cafebabe.Method;

namespace Cafebabe.Class;

public class JavaClassFile
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

		var thisClassPoolIdx = br.ReadInt16();
		var thisClass = (string)constantPool.Constants[(short)constantPool.Constants[thisClassPoolIdx]];

		var superClassPoolIdx = br.ReadInt16();
		var superClass = (string)constantPool.Constants[(short)constantPool.Constants[superClassPoolIdx]];

		var numInterfaces = br.ReadInt16();
		var interfaces = new short[numInterfaces];
		for (var i = 0; i < interfaces.Length; i++)
			interfaces[i] = br.ReadInt16();

		var numFields = br.ReadInt16();
		var fields = new JavaFieldInfo[numFields];
		for (var i = 0; i < fields.Length; i++)
			fields[i] = JavaPooledFieldInfo.Read(br).Bake(constantPool);

		var numMethods = br.ReadInt16();
		var methods = new JavaMethodInfo[numMethods];
		for (var i = 0; i < methods.Length; i++)
			methods[i] = JavaPooledMethodInfo.Read(br).Bake(constantPool);

		var numAttributes = br.ReadInt16();
		var attributes = new JavaAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaPooledAttributeInfo.Read(br).Bake(constantPool);

		return null;
	}
}