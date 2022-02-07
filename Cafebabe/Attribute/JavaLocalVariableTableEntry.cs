namespace Cafebabe.Attribute;

public record JavaLocalVariableTableEntry(short StartPc, short Length, JavaNameAndTypeDescriptor NameAndTypeDescriptor, short Index);