using Cafebabe.Class;
using Cafebabe.Method;

namespace Cafebabe.Attribute;

public record JavaBootstrapMethodEntry(JavaMethodHandle Method, JavaConstantPoolEntry[] Args);