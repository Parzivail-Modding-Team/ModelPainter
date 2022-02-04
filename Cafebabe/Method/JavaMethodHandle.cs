namespace Cafebabe.Method;

public record JavaMethodHandle(byte TypeDescriptor, short NamePoolIdx)
{
	public static JavaMethodHandle Read(BinaryReader r, ref int i)
	{
		var type = r.ReadByte();
		var nameIdx = r.ReadInt16();

		return new JavaMethodHandle(type, nameIdx);
	}
}