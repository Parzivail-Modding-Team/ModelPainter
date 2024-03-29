﻿using Cafebabe.Bytecode;
using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaCodeAttributeInfo
(string Name, short MaxStack, short MaxLocals, SortedDictionary<short, JvmInstruction> Instructions, SortedDictionary<short, short> OffsetToInstructionCounterTable,
	JavaExceptionTableEntry[] ExceptionTable,
	JavaAttributeInfo[] Attributes) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = Utils.CreateReader(data);

		var maxStack = br.ReadInt16();
		var maxLocals = br.ReadInt16();

		var codeLength = br.ReadInt32();
		var code = br.ReadBytes(codeLength);

		var (instructions, offsetToInstructionCounterTable) = JvmBytecodeParser.ParseInstructions(constantPool, code);

		var exceptionTableLength = br.ReadInt16();
		var exceptionTable = new JavaExceptionTableEntry[exceptionTableLength];

		for (var i = 0; i < exceptionTable.Length; i++)
		{
			var startPc = br.ReadInt16();
			var endPc = br.ReadInt16();
			var handlerPc = br.ReadInt16();
			var catchTypeIdx = br.ReadInt16();
			exceptionTable[i] = new JavaExceptionTableEntry(startPc, endPc, handlerPc, constantPool.GetClassInfo(catchTypeIdx));
		}

		var numAttributes = br.ReadInt16();
		var attributes = new JavaAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaAttributeInfo.Read(constantPool, br);

		return new JavaCodeAttributeInfo(name, maxStack, maxLocals, instructions, offsetToInstructionCounterTable, exceptionTable, attributes);
	}
}