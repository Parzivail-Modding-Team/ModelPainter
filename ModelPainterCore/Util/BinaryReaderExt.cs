using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Mathematics;

namespace ModelPainterCore.Util;

public static class BinaryReaderExt
{
	public static Vector3 ReadVector3(this BinaryReader r)
	{
		return r.ReadStruct<Vector3>();
	}

	public static Vector2 ReadVector2(this BinaryReader r)
	{
		return r.ReadStruct<Vector2>();
	}

	public static T ReadStruct<T>(this BinaryReader br) where T : struct
	{
		var s = new T();
		var structSpan = GetStructSpan(ref s);
		var fileData = br.ReadBytes(structSpan.Length);
		fileData.CopyTo(structSpan);
		return s;
	}

	private static Span<byte> GetStructSpan<T>(ref T data) where T : struct
	{
		return MemoryMarshal.Cast<T, byte>(MemoryMarshal.CreateSpan(ref data, 1));
	}

	public static string ReadNullTermString(this BinaryReader reader)
	{
		using var data = new MemoryStream();
		while (true)
		{
			var b = reader.ReadByte();
			if (b == 0)
				return Encoding.UTF8.GetString(data.ToArray());
			data.WriteByte(b);
		}
	}
}