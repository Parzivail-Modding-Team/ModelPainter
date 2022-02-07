using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaVerificationTypeInfo(JavaVerificationTypeTag Tag)
{
	public static JavaVerificationTypeInfo Read(JavaConstantPool constantPool, BinaryReader r)
	{
		var tag = (JavaVerificationTypeTag)r.ReadByte();
		switch (tag)
		{
			case JavaVerificationTypeTag.Top:
			case JavaVerificationTypeTag.Integer:
			case JavaVerificationTypeTag.Float:
			case JavaVerificationTypeTag.Null:
			case JavaVerificationTypeTag.Double:
			case JavaVerificationTypeTag.Long:
			case JavaVerificationTypeTag.UninitializedThis:
				return new JavaVerificationTypeInfo(tag);
			case JavaVerificationTypeTag.Object:
			{
				var nameIndex = r.ReadInt16();
				return new JavaVerificationTypeNameInfo(tag, constantPool.GetClassInfo(nameIndex));
			}
			case JavaVerificationTypeTag.Uninitialized:
			{
				var data = r.ReadInt16();
				return new JavaVerificationTypeOffsetInfo(tag, data);
			}
			default:
				throw new NotSupportedException($"Unsupported verification tag \"{tag}\"");
		}
	}
}