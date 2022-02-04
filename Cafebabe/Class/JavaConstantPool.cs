using System.Text;
using Cafebabe.Method;

namespace Cafebabe.Class;

public record JavaConstantPool(object[] Constants)
{
	private delegate object PoolEntryReader(BinaryReader r, ref int poolIdx);

	private const int ConstantPoolTagUnicodeString = 1;
	private const int ConstantPoolTagInteger = 3;
	private const int ConstantPoolTagFloat = 4;
	private const int ConstantPoolTagLong = 5;
	private const int ConstantPoolTagDouble = 6;
	private const int ConstantPoolTagClassReference = 7;
	private const int ConstantPoolTagStringReference = 8;
	private const int ConstantPoolTagFieldReference = 9;
	private const int ConstantPoolTagMethodReference = 10;
	private const int ConstantPoolTagInterfaceMethodReference = 11;
	private const int ConstantPoolTagNameAndTypeDescriptor = 12;
	private const int ConstantPoolTagMethodHandle = 15;
	private const int ConstantPoolTagMethodType = 16;
	private const int ConstantPoolTagDynamic = 17;
	private const int ConstantPoolTagInvokeDynamic = 18;
	private const int ConstantPoolTagModule = 19;
	private const int ConstantPoolTagPackage = 20;

	private static readonly Dictionary<byte, PoolEntryReader> ConstantParsers = new()
	{
		[ConstantPoolTagUnicodeString] = ReadUnicodeString,
		[ConstantPoolTagInteger] = (BinaryReader reader, ref int _) => reader.ReadInt32(),
		[ConstantPoolTagFloat] = (BinaryReader reader, ref int _) => reader.ReadSingle(),
		[ConstantPoolTagLong] = (BinaryReader reader, ref int i) =>
		{
			i++;
			return reader.ReadInt64();
		},
		[ConstantPoolTagDouble] = (BinaryReader reader, ref int i) =>
		{
			i++;
			return reader.ReadDouble();
		},
		[ConstantPoolTagClassReference] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[ConstantPoolTagStringReference] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[ConstantPoolTagFieldReference] = JavaPooledClassChildReference.Read,
		[ConstantPoolTagMethodReference] = JavaPooledClassChildReference.Read,
		[ConstantPoolTagInterfaceMethodReference] = JavaPooledClassChildReference.Read,
		[ConstantPoolTagNameAndTypeDescriptor] = JavaPooledNameAndTypeDescriptor.Read,
		[ConstantPoolTagMethodHandle] = JavaMethodHandle.Read,
		[ConstantPoolTagMethodType] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[ConstantPoolTagDynamic] = (BinaryReader reader, ref int _) => reader.ReadUInt32(),
		[ConstantPoolTagInvokeDynamic] = (BinaryReader reader, ref int _) => reader.ReadUInt32(),
		[ConstantPoolTagModule] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[ConstantPoolTagPackage] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
	};

	private static object ReadUnicodeString(BinaryReader r, ref int poolIdx)
	{
		var length = r.ReadUInt16();
		return Encoding.UTF8.GetString(r.ReadBytes(length));
	}

	public static JavaConstantPool Read(BinaryReader r)
	{
		var constantPoolSize = r.ReadInt16();

		var constants = new object[constantPoolSize];

		for (var i = 1; i < constants.Length; i++)
		{
			var tag = r.ReadByte();
			constants[i] = ConstantParsers[tag].Invoke(r, ref i);
		}

		return new JavaConstantPool(constants);
	}
}