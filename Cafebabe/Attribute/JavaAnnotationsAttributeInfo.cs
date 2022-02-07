using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaAnnotationsAttributeInfo(string Name, JavaAnnotation[] Annotations) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numAnnotations = br.ReadInt16();
		var annotations = new JavaAnnotation[numAnnotations];

		for (var i = 0; i < annotations.Length; i++)
			annotations[i] = JavaAnnotation.Read(constantPool, br);

		return new JavaAnnotationsAttributeInfo(name, annotations);
	}
}