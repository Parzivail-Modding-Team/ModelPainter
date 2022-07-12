namespace Cafebabe;

public class JavaUtils
{
	public static JavaDescriptor DeconstructFieldDescriptor(string descriptor)
	{
		return TakeFieldDescriptor(ref descriptor);
	}

	public static (JavaDescriptor ReturnType, JavaDescriptor[] ParamTypes) DeconstructMethodDescriptor(string descriptor)
	{
		if (descriptor[0] != '(')
			throw new ArgumentException("Descriptor did not start with '('", nameof(descriptor));

		descriptor = descriptor[1..];

		var paramTypes = new List<JavaDescriptor>();
		while (descriptor[0] != ')')
			paramTypes.Add(TakeFieldDescriptor(ref descriptor));

		descriptor = descriptor[1..];

		var returnType = TakeFieldDescriptor(ref descriptor);

		return (returnType, paramTypes.ToArray());
	}

	private static JavaDescriptor TakeFieldDescriptor(ref string descriptor)
	{
		var firstChar = descriptor[0];
		descriptor = descriptor[1..];

		switch (firstChar)
		{
			case 'V':
				return new JavaDescriptor(DescriptorType.Void);
			case 'B':
				return new JavaDescriptor(DescriptorType.Byte);
			case 'C':
				return new JavaDescriptor(DescriptorType.Character);
			case 'D':
				return new JavaDescriptor(DescriptorType.Double);
			case 'F':
				return new JavaDescriptor(DescriptorType.Float);
			case 'I':
				return new JavaDescriptor(DescriptorType.Integer);
			case 'J':
				return new JavaDescriptor(DescriptorType.Long);
			case 'L':
			{
				var semicolonIndex = descriptor.IndexOf(';');
				if (semicolonIndex == -1)
					throw new ArgumentException("Class reference was not terminated", nameof(descriptor));

				var reference = descriptor[..semicolonIndex];
				descriptor = descriptor[(semicolonIndex + 1)..];

				return new JavaDescriptor(DescriptorType.ClassReference, reference);
			}
			case 'S':
				return new JavaDescriptor(DescriptorType.Short);
			case 'Z':
				return new JavaDescriptor(DescriptorType.Boolean);
			case '[':
				return new JavaDescriptor(DescriptorType.Array, SubType: TakeFieldDescriptor(ref descriptor));
			default:
				throw new ArgumentException("Next character did not start a field descriptor", nameof(descriptor));
		}
	}
}

public record JavaDescriptor(DescriptorType Type, string Reference = null, JavaDescriptor SubType = null);

public enum DescriptorType
{
	Byte,
	Character,
	Double,
	Float,
	Integer,
	Long,
	ClassReference,
	Short,
	Boolean,
	Array,
	Void
}