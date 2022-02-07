namespace Cafebabe.Class;

[Flags]
public enum JavaInnerClassAccessFlag : ushort
{
	Public = 0x1,
	Private = 0x2,
	Protected = 0x4,
	Static = 0x8,
	Final = 0x10,
	Interface = 0x200,
	Abstract = 0x400,
	Synthetic = 0x1000,
	Annotation = 0x2000,
	Enum = 0x4000
}