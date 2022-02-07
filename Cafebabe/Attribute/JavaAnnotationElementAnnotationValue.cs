namespace Cafebabe.Attribute;

public record JavaAnnotationElementAnnotationValue(JavaTagType TagType, JavaAnnotation Annotation) : JavaAnnotationElementValue(TagType);