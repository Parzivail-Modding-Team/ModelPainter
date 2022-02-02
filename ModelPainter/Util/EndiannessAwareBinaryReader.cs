using System.Text;

namespace ModelPainter.Util;

public class EndiannessAwareBinaryReader : BinaryReader
{
	public enum Endianness
	{
		Little,
		Big
	}

	private static readonly Endianness SystemEndianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

	public Endianness StreamEndianness { get; }

	private const int MaxSingleReadBytes = 16;
	private readonly byte[] _buffer = new byte[MaxSingleReadBytes];

	public EndiannessAwareBinaryReader(Stream input, Endianness endianness) : base(input)
	{
		StreamEndianness = endianness;
	}

	public override string ReadString() => string.Empty;

	public string ReadUtf()
	{
		var length = ReadInt16();
		var bytes = ReadBytes(length);
		return Encoding.UTF8.GetString(bytes);
	}

	public override char ReadChar() => ReadChar(StreamEndianness);

	public override short ReadInt16() => ReadInt16(StreamEndianness);

	public override int ReadInt32() => ReadInt32(StreamEndianness);

	public override float ReadSingle() => ReadSingle(StreamEndianness);

	public override long ReadInt64() => ReadInt64(StreamEndianness);

	public override double ReadDouble() => ReadDouble(StreamEndianness);

	public override decimal ReadDecimal() => ReadDecimal(StreamEndianness);

	public override ushort ReadUInt16() => ReadUInt16(StreamEndianness);

	public override uint ReadUInt32() => ReadUInt32(StreamEndianness);

	public override ulong ReadUInt64() => ReadUInt64(StreamEndianness);

	public char ReadChar(Endianness endianness)
	{
		if (endianness == SystemEndianness)
			return base.ReadChar();

		throw new NotImplementedException("To read alternate-endianness characters, instead set the stream encoding to the desired endianness encoding");
	}

	public short ReadInt16(Endianness endianness) => BitConverter.ToInt16(ReadForEndianness(sizeof(short), endianness), 0);

	public int ReadInt32(Endianness endianness) => BitConverter.ToInt32(ReadForEndianness(sizeof(int), endianness), 0);

	public float ReadSingle(Endianness endianness) => BitConverter.ToSingle(ReadForEndianness(sizeof(float), endianness), 0);

	public long ReadInt64(Endianness endianness) => BitConverter.ToInt64(ReadForEndianness(sizeof(long), endianness), 0);

	public double ReadDouble(Endianness endianness) => BitConverter.ToDouble(ReadForEndianness(sizeof(double), endianness), 0);

	public decimal ReadDecimal(Endianness endianness)
	{
		// TODO: this is cancer
		var b = ReadForEndianness(sizeof(int) * 4, endianness);
		return new decimal(new[]
		{
			BitConverter.ToInt32(b, 0),
			BitConverter.ToInt32(b, 4),
			BitConverter.ToInt32(b, 8),
			BitConverter.ToInt32(b, 12)
		});
	}

	public ushort ReadUInt16(Endianness endianness) => BitConverter.ToUInt16(ReadForEndianness(sizeof(ushort), endianness), 0);

	public uint ReadUInt32(Endianness endianness) => BitConverter.ToUInt32(ReadForEndianness(sizeof(uint), endianness), 0);

	public ulong ReadUInt64(Endianness endianness) => BitConverter.ToUInt64(ReadForEndianness(sizeof(ulong), endianness), 0);

	private byte[] ReadForEndianness(int bytesToRead, Endianness endianness)
	{
		if (endianness != SystemEndianness)
		{
			ReadReverse(_buffer, 0, bytesToRead);
			return _buffer;
		}

		var r = Read(_buffer, 0, bytesToRead);

		if (r < bytesToRead)
			throw new EndOfStreamException();

		return _buffer;
	}

	private void ReadReverse(byte[] buffer, int index, int count)
	{
		if (BaseStream.Position + count > BaseStream.Length)
			throw new EndOfStreamException();

		for (var i = 0; i < count; i++) buffer[index + count - i - 1] = ReadByte();
	}
}