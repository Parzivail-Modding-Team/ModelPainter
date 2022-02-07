using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaSignatureAttributeInfo(string Name, string Signature) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var signatureIndex = br.ReadInt16();
		return new JavaSignatureAttributeInfo(name, constantPool.GetString(signatureIndex));
	}
}