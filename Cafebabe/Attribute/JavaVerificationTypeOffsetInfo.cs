namespace Cafebabe.Attribute;

public record JavaVerificationTypeOffsetInfo(JavaVerificationTypeTag Tag, short Offset) : JavaVerificationTypeInfo(Tag);