namespace Cafebabe.Attribute;

public record JavaAnnotationElementClassValue(JavaTagType TagType, string Class) : JavaAnnotationElementValue(TagType);