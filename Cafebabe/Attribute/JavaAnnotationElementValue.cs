using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaAnnotationElementValue(JavaTagType TagType)
{
	public static JavaAnnotationElementValue Read(JavaConstantPool constantPool, BinaryReader r)
	{
		var tag = (JavaTagType)r.ReadByte();

		switch (tag)
		{
			case JavaTagType.Byte:
			case JavaTagType.Char:
			case JavaTagType.Double:
			case JavaTagType.Float:
			case JavaTagType.Int:
			case JavaTagType.Long:
			case JavaTagType.Short:
			case JavaTagType.Boolean:
			case JavaTagType.String:
			{
				var constantIdx = r.ReadInt16();
				return new JavaAnnotationElementConstantValue(tag, constantPool.Constants[constantIdx]);
			}
			case JavaTagType.EnumClass:
			{
				var type = constantPool.GetString(r.ReadInt16());
				var value = constantPool.GetString(r.ReadInt16());
				return new JavaAnnotationElementEnumValue(tag, type, value);
			}
			case JavaTagType.Class:
			{
				var type = constantPool.GetString(r.ReadInt16());
				return new JavaAnnotationElementClassValue(tag, type);
			}
			case JavaTagType.AnnotationInterface:
			{
				var annotation = JavaAnnotation.Read(constantPool, r);
				return new JavaAnnotationElementAnnotationValue(tag, annotation);
			}
			case JavaTagType.ArrayType:
			{
				var numValues = r.ReadInt16();
				var values = new JavaAnnotationElementValue[numValues];

				for (var i = 0; i < values.Length; i++)
					values[i] = JavaAnnotationElementValue.Read(constantPool, r);

				return new JavaAnnotationElementArrayValue(tag, values);
			}
			default:
				throw new NotSupportedException($"Unsupported tag type \"{tag}\"");
		}
	}
}