using Cafebabe.Attribute;

namespace Cafebabe.Method;

public record JavaMethodInfo(JavaMethodAccessFlag AccessFlags, string Name, string Descriptor, JavaAttributeInfo[] Attributes);