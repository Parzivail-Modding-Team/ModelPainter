namespace Cafebabe;

public enum JavaTagType : byte
{
	Byte = (byte)'B',
	Char = (byte)'C',
	Double = (byte)'D',
	Float = (byte)'F',
	Int = (byte)'I',
	Long = (byte)'J',
	Short = (byte)'S',
	Boolean = (byte)'Z',
	String = (byte)'s',
	EnumClass = (byte)'e',
	Class = (byte)'c',
	AnnotationInterface = (byte)'@',
	ArrayType = (byte)'[',
}