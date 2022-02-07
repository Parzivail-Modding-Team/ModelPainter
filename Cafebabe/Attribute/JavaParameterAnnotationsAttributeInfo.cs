using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaParameterAnnotationsAttributeInfo(string Name, JavaParameterAnnotation[] Annotations) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numParameters = br.ReadByte();
		var parameters = new JavaParameterAnnotation[numParameters];

		for (var i = 0; i < numParameters; i++)
		{
			var numAnnotations = br.ReadInt16();
			var annotations = new JavaAnnotation[numAnnotations];

			for (var j = 0; j < annotations.Length; j++)
				annotations[j] = JavaAnnotation.Read(constantPool, br);

			parameters[i] = new JavaParameterAnnotation(annotations);
		}

		return new JavaParameterAnnotationsAttributeInfo(name, parameters);
	}
}