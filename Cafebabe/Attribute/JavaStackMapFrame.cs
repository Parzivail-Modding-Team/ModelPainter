namespace Cafebabe.Attribute;

public record JavaStackMapFrame(byte FrameTypeId, JavaStackFrameType FrameType, short OffsetDelta, JavaVerificationTypeInfo[] Stack, JavaVerificationTypeInfo[] Locals);