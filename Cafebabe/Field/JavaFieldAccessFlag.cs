namespace Cafebabe.Field;

[Flags]
public enum JavaFieldAccessFlag : ushort
{
	Public = 0x1,
	Private = 0x2,
	Protected = 0x4,
	Static = 0x8,
	Final = 0x10,
	Volatile = 0x40,
	Transient = 0x80,
	Synthetic = 0x1000,
	Enum = 0x4000
}