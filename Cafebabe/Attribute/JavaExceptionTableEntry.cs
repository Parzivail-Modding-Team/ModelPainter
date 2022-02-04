namespace Cafebabe.Attribute;

public record JavaExceptionTableEntry(short StartPc, short EndPc, short HandlerPc, string CatchType);