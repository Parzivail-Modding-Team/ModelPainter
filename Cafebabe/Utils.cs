namespace Cafebabe;

internal class Utils
{
	internal static BinaryReader CreateReader(byte[] data, EndiannessAwareBinaryReader.Endianness endianness = EndiannessAwareBinaryReader.Endianness.Big)
	{
		return new EndiannessAwareBinaryReader(new MemoryStream(data), endianness);
	}
}