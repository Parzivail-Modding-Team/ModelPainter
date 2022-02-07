using System.Text;
using Cafebabe.Method;

namespace Cafebabe.Class;

public record JavaConstantPool(JavaConstantPoolEntry[] Constants)
{
	private delegate object PoolEntryReader(BinaryReader r, ref int poolIdx);

	private static readonly Dictionary<JavaConstantPoolTag, PoolEntryReader> ConstantParsers = new()
	{
		[JavaConstantPoolTag.Utf8] = ReadUnicodeString,
		[JavaConstantPoolTag.Integer] = (BinaryReader reader, ref int _) => reader.ReadInt32(),
		[JavaConstantPoolTag.Float] = (BinaryReader reader, ref int _) => reader.ReadSingle(),
		[JavaConstantPoolTag.Long] = (BinaryReader reader, ref int i) =>
		{
			i++;
			return reader.ReadInt64();
		},
		[JavaConstantPoolTag.Double] = (BinaryReader reader, ref int i) =>
		{
			i++;
			return reader.ReadDouble();
		},
		[JavaConstantPoolTag.Class] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[JavaConstantPoolTag.String] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[JavaConstantPoolTag.FieldReference] = JavaPooledClassChildReference.Read,
		[JavaConstantPoolTag.MethodReference] = JavaPooledClassChildReference.Read,
		[JavaConstantPoolTag.InterfaceMethodReference] = JavaPooledClassChildReference.Read,
		[JavaConstantPoolTag.NameAndType] = JavaPooledNameAndTypeDescriptor.Read,
		[JavaConstantPoolTag.MethodHandle] = JavaMethodHandle.Read,
		[JavaConstantPoolTag.MethodType] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[JavaConstantPoolTag.Dynamic] = JavaPooledDynamicReference.Read,
		[JavaConstantPoolTag.InvokeDynamic] = JavaPooledDynamicReference.Read,
		[JavaConstantPoolTag.Module] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
		[JavaConstantPoolTag.Package] = (BinaryReader reader, ref int _) => reader.ReadInt16(),
	};

	private static object ReadUnicodeString(BinaryReader r, ref int poolIdx)
	{
		var length = r.ReadUInt16();
		return Encoding.UTF8.GetString(r.ReadBytes(length));
	}

	public static JavaConstantPool Read(BinaryReader r)
	{
		var constantPoolSize = r.ReadInt16();

		var constants = new JavaConstantPoolEntry[constantPoolSize];
		constants[0] = new JavaConstantPoolEntry(JavaConstantPoolTag.Invalid, null);

		for (var i = 1; i < constants.Length; i++)
		{
			var tag = (JavaConstantPoolTag)r.ReadByte();
			constants[i] = new JavaConstantPoolEntry(tag, ConstantParsers[tag].Invoke(r, ref i));
		}

		return new JavaConstantPool(constants);
	}

	public string GetClassInfo(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.Class);
		return i == 0 ? null : GetString((short)Constants[i].Data);
	}

	public string GetString(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.Utf8);
		return (string)Constants[i].Data;
	}

	public JavaPooledClassChildReference GetClassChildRef(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.FieldReference, JavaConstantPoolTag.MethodReference, JavaConstantPoolTag.InterfaceMethodReference);
		return (JavaPooledClassChildReference)Constants[i].Data;
	}

	public JavaMethodHandle GetMethodHandle(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.MethodHandle);
		return (JavaMethodHandle)Constants[i].Data;
	}

	public string GetMethodType(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.MethodType);
		return GetString((short)Constants[i].Data);
	}

	public JavaPooledDynamicReference GetClassDynamicRef(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.Dynamic, JavaConstantPoolTag.InvokeDynamic);
		return (JavaPooledDynamicReference)Constants[i].Data;
	}

	public JavaPooledNameAndTypeDescriptor GetNameTypeInfo(short i)
	{
		AssertConstantType(i, JavaConstantPoolTag.NameAndType);
		return (JavaPooledNameAndTypeDescriptor)Constants[i].Data;
	}

	private void AssertConstantType(short i, params JavaConstantPoolTag[] tag)
	{
		if (i == 0)
			return;

		if (Constants[i] == null)
			throw new InvalidOperationException($"Expected [{string.Join(", ", tag)}] constant at {i}, got null");

		if (tag.Contains(Constants[i].Tag))
			return;

		throw new InvalidOperationException($"Expected [{string.Join(", ", tag)}] constant at {i}, got {Constants[i].Tag}");
	}
}