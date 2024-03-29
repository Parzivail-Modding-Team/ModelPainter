﻿namespace Cafebabe.Class;

[Flags]
public enum JavaClassAccessFlag : ushort
{
	Public = 0x1,
	Final = 0x10,
	Super = 0x20,
	Interface = 0x200,
	Abstract = 0x400,
	Synthetic = 0x1000,
	Annotation = 0x2000,
	Enum = 0x4000
}