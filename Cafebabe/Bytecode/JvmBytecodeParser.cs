using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public static class JvmBytecodeParser
{
	private delegate JvmInstruction JavaAttributeInfoBaker(JavaConstantPool constantPool, JvmOpcode opcode, BinaryReader r);

	private static readonly Dictionary<JvmOpcode, JavaAttributeInfoBaker> Bakery = new()
	{
		[JvmOpcode.invokespecial] = JvmInvokespecialInstruction.Read,
		[JvmOpcode.bipush] = JvmBipushInstruction.Read
	};

	public static byte[] ParseInstructions(JavaConstantPool constantPool, byte[] code)
	{
		using var br = new EndiannessAwareBinaryReader(new MemoryStream(code), EndiannessAwareBinaryReader.Endianness.Big);

		var instructions = new List<JvmInstruction>();

		while (br.BaseStream.Position < code.Length)
		{
			var opcode = (JvmOpcode)br.ReadByte();
			if (Bakery.TryGetValue(opcode, out var baker))
				instructions.Add(baker.Invoke(constantPool, opcode, br));
			else
			{
				Console.WriteLine($"No baker for opcode {opcode}");
				instructions.Add(new JvmInstruction(opcode));
			}
		}

		return null;
	}
}