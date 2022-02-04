using Cafebabe.Attribute;

namespace Cafebabe.Field;

public record JavaFieldInfo(JavaFieldAccessFlag AccessFlags, string Name, string Descriptor, JavaAttributeInfo[] Attributes);