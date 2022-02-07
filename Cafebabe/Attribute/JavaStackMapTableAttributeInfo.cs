using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaStackMapTableAttributeInfo(string Name, JavaStackMapFrame[] Entries) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var numEntries = br.ReadInt16();
		var entries = new JavaStackMapFrame[numEntries];

		for (var i = 0; i < entries.Length; i++)
		{
			var frameType = br.ReadByte();
			switch (frameType)
			{
				case <= 63:
					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.Same, 0, Array.Empty<JavaVerificationTypeInfo>(), Array.Empty<JavaVerificationTypeInfo>());
					break;
				case <= 127:
				{
					var stack = JavaVerificationTypeInfo.Read(constantPool, br);
					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.SameLocals1StackItem, 0, new[] { stack }, Array.Empty<JavaVerificationTypeInfo>());
					break;
				}
				case <= 246:
					throw new NotSupportedException($"Unsupported frame type \"{frameType}\"");
				case <= 247:
				{
					var offsetDelta = br.ReadInt16();
					var stack = JavaVerificationTypeInfo.Read(constantPool, br);
					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.SameLocals1StackItemExtended, offsetDelta, new[] { stack }, Array.Empty<JavaVerificationTypeInfo>());
					break;
				}
				case <= 250:
				{
					var offsetDelta = br.ReadInt16();
					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.Chop, offsetDelta, Array.Empty<JavaVerificationTypeInfo>(), Array.Empty<JavaVerificationTypeInfo>());
					break;
				}
				case <= 251:
				{
					var offsetDelta = br.ReadInt16();
					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.SameFrameExtended, offsetDelta, Array.Empty<JavaVerificationTypeInfo>(), Array.Empty<JavaVerificationTypeInfo>());
					break;
				}
				case <= 254:
				{
					var offsetDelta = br.ReadInt16();

					var stack = new JavaVerificationTypeInfo[frameType - 251];
					for (var j = 0; j < stack.Length; j++)
						stack[j] = JavaVerificationTypeInfo.Read(constantPool, br);

					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.Append, offsetDelta, stack, Array.Empty<JavaVerificationTypeInfo>());
					break;
				}
				case <= 255:
				{
					var offsetDelta = br.ReadInt16();
					var numLocals = br.ReadInt16();

					var locals = new JavaVerificationTypeInfo[numLocals];
					for (var j = 0; j < locals.Length; j++)
						locals[j] = JavaVerificationTypeInfo.Read(constantPool, br);

					var numStackItems = br.ReadInt16();

					var stack = new JavaVerificationTypeInfo[numStackItems];
					for (var j = 0; j < stack.Length; j++)
						stack[j] = JavaVerificationTypeInfo.Read(constantPool, br);

					entries[i] = new JavaStackMapFrame(frameType, JavaStackFrameType.FullFrame, offsetDelta, stack, locals);
					break;
				}
			}
		}

		return new JavaStackMapTableAttributeInfo(name, entries);
	}
}