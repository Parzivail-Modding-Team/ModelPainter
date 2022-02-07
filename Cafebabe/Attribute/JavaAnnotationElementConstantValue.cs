using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaAnnotationElementConstantValue(JavaTagType TagType, JavaConstantPoolEntry Constant) : JavaAnnotationElementValue(TagType);