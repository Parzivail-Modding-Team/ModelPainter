using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaAttributeInfo(string Name)
{
	private delegate JavaAttributeInfo JavaAttributeInfoBaker(JavaConstantPool constantPool, string name, byte[] data);

	private static readonly Dictionary<string, JavaAttributeInfoBaker> Bakery = new()
	{
		["Deprecated"] = ReadSimpleAttribute,
		["SourceFile"] = JavaSourceFileAttributeInfo.Read,
		["LineNumberTable"] = JavaLineNumberTableAttributeInfo.Read,
		["Code"] = JavaCodeAttributeInfo.Read,
		["LocalVariableTable"] = JavaLocalVariableTableAttributeInfo.Read,
		["Signature"] = JavaSignatureAttributeInfo.Read,
		["MethodParameters"] = JavaMethodParametersAttributeInfo.Read,
		["StackMapTable"] = JavaStackMapTableAttributeInfo.Read,
		["LocalVariableTypeTable"] = JavaLocalVariableTypeTableAttributeInfo.Read,
		["InnerClasses"] = JavaInnerClassesAttributeInfo.Read,
		["EnclosingMethod"] = JavaEnclosingMethodAttributeInfo.Read,
		["ConstantValue"] = JavaConstantValueAttributeInfo.Read,
		["NestHost"] = JavaNestHostAttributeInfo.Read,
		["NestMembers"] = JavaClassListAttributeInfo.Read,
		["Record"] = JavaRecordAttributeInfo.Read,
		["Exceptions"] = JavaClassListAttributeInfo.Read,
		["PermittedSubclasses"] = JavaClassListAttributeInfo.Read,
		["BootstrapMethods"] = JavaBootstrapMethodsAttributeInfo.Read,
		["RuntimeVisibleAnnotations"] = JavaAnnotationsAttributeInfo.Read,
		["RuntimeInvisibleAnnotations"] = JavaAnnotationsAttributeInfo.Read,
		["RuntimeVisibleParameterAnnotations"] = JavaParameterAnnotationsAttributeInfo.Read,
		["RuntimeInvisibleParameterAnnotations"] = JavaParameterAnnotationsAttributeInfo.Read,
		["AnnotationDefault"] = JavaAnnotationDefaultAttributeInfo.Read,
		["RuntimeVisibleTypeAnnotations"] = JavaRuntimeVisibleTypeAnnotationsAttributeInfo.Read,
		["RuntimeInvisibleTypeAnnotations"] = JavaRuntimeVisibleTypeAnnotationsAttributeInfo.Read,
	};

	private static JavaAttributeInfo ReadSimpleAttribute(JavaConstantPool constantpool, string name, byte[] data)
	{
		if (data.Length != 0)
			throw new InvalidOperationException($"Simple attribute \"{name}\" cannot have data (found {data.Length} bytes of data)");

		return new JavaAttributeInfo(name);
	}

	public static JavaAttributeInfo Read(JavaConstantPool constantPool, BinaryReader r)
	{
		var nameIdx = r.ReadInt16();

		var dataLength = r.ReadInt32();
		var data = r.ReadBytes(dataLength);

		var name = constantPool.GetString(nameIdx);

		if (Bakery.TryGetValue(name, out var baker))
			return baker.Invoke(constantPool, name, data);

		throw new NotSupportedException($"Unsupported attribute \"{name}\"");
	}
}