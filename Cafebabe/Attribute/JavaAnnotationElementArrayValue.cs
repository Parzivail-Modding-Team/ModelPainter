namespace Cafebabe.Attribute;

public record JavaAnnotationElementArrayValue(JavaTagType TagType, JavaAnnotationElementValue[] Values) : JavaAnnotationElementValue(TagType);