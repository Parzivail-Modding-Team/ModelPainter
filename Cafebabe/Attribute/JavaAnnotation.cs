using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaAnnotation(string Type, KeyValuePair<string, JavaAnnotationElementValue>[] Pairs)
{
	public static JavaAnnotation Read(JavaConstantPool constantPool, BinaryReader r)
	{
		var type = constantPool.GetString(r.ReadInt16());

		var numPairs = r.ReadInt16();
		var pairs = new KeyValuePair<string, JavaAnnotationElementValue>[numPairs];

		for (var i = 0; i < pairs.Length; i++)
		{
			var pairName = constantPool.GetString(r.ReadInt16());
			var pairValue = JavaAnnotationElementValue.Read(constantPool, r);
			pairs[i] = new KeyValuePair<string, JavaAnnotationElementValue>(pairName, pairValue);
		}

		return new JavaAnnotation(type, pairs);
	}
}