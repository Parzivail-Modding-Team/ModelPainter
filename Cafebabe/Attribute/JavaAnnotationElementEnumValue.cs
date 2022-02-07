namespace Cafebabe.Attribute;

public record JavaAnnotationElementEnumValue(JavaTagType TagType, string Enum, string Value) : JavaAnnotationElementValue(TagType);