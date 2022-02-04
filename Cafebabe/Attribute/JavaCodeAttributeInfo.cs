using Cafebabe.Bytecode;
using Cafebabe.Class;

namespace Cafebabe.Attribute;

public record JavaCodeAttributeInfo(string Name, short MaxStack, short MaxLocals, byte[] Code, JavaExceptionTableEntry[] ExceptionTable, JavaAttributeInfo[] Attributes) : JavaAttributeInfo(Name)
{
	public static JavaAttributeInfo Read(JavaConstantPool constantPool, string name, byte[] data)
	{
		using var br = new EndiannessAwareBinaryReader(new MemoryStream(data), EndiannessAwareBinaryReader.Endianness.Big);

		var maxStack = br.ReadInt16();
		var maxLocals = br.ReadInt16();

		var codeLength = br.ReadInt32();
		var code = br.ReadBytes(codeLength);

		// TODO:
		var instructions = JvmBytecodeParser.ParseInstructions(constantPool, code);

		var exceptionTableLength = br.ReadInt16();
		var exceptionTable = new JavaExceptionTableEntry[exceptionTableLength];

		for (var i = 0; i < exceptionTable.Length; i++)
		{
			var startPc = br.ReadInt16();
			var endPc = br.ReadInt16();
			var handlerPc = br.ReadInt16();
			var catchType = br.ReadInt16();
			exceptionTable[i] = new JavaExceptionTableEntry(startPc, endPc, handlerPc, (string)constantPool.Constants[catchType]);
		}

		var numAttributes = br.ReadInt16();
		var attributes = new JavaAttributeInfo[numAttributes];
		for (var i = 0; i < attributes.Length; i++)
			attributes[i] = JavaPooledAttributeInfo.Read(br).Bake(constantPool);

		return new JavaCodeAttributeInfo(name, maxStack, maxLocals, code, exceptionTable, attributes);
	}
}