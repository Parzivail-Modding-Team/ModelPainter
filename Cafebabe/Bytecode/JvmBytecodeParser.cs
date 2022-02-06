﻿using Cafebabe.Class;

namespace Cafebabe.Bytecode;

public static class JvmBytecodeParser
{
	private delegate JvmInstruction JavaAttributeInfoBaker(JavaConstantPool constantPool, JvmOpcode opcode, BinaryReader r);

	private static readonly Dictionary<JvmOpcode, JavaAttributeInfoBaker> Bakery = new()
	{
		[JvmOpcode.aaload] = DefaultBakery,
		[JvmOpcode.aastore] = DefaultBakery,
		[JvmOpcode.aconst_null] = DefaultBakery,
		[JvmOpcode.aload_0] = DefaultBakery,
		[JvmOpcode.aload_1] = DefaultBakery,
		[JvmOpcode.aload_2] = DefaultBakery,
		[JvmOpcode.aload_3] = DefaultBakery,
		[JvmOpcode.areturn] = DefaultBakery,
		[JvmOpcode.arraylength] = DefaultBakery,
		[JvmOpcode.astore_0] = DefaultBakery,
		[JvmOpcode.astore_1] = DefaultBakery,
		[JvmOpcode.astore_2] = DefaultBakery,
		[JvmOpcode.astore_3] = DefaultBakery,
		[JvmOpcode.athrow] = DefaultBakery,
		[JvmOpcode.baload] = DefaultBakery,
		[JvmOpcode.bastore] = DefaultBakery,
		[JvmOpcode.breakpoint] = DefaultBakery,
		[JvmOpcode.caload] = DefaultBakery,
		[JvmOpcode.castore] = DefaultBakery,
		[JvmOpcode.d2f] = DefaultBakery,
		[JvmOpcode.d2i] = DefaultBakery,
		[JvmOpcode.d2l] = DefaultBakery,
		[JvmOpcode.dadd] = DefaultBakery,
		[JvmOpcode.daload] = DefaultBakery,
		[JvmOpcode.dastore] = DefaultBakery,
		[JvmOpcode.dcmpg] = DefaultBakery,
		[JvmOpcode.dcmpl] = DefaultBakery,
		[JvmOpcode.dconst_0] = DefaultBakery,
		[JvmOpcode.dconst_1] = DefaultBakery,
		[JvmOpcode.ddiv] = DefaultBakery,
		[JvmOpcode.dload_0] = DefaultBakery,
		[JvmOpcode.dload_1] = DefaultBakery,
		[JvmOpcode.dload_2] = DefaultBakery,
		[JvmOpcode.dload_3] = DefaultBakery,
		[JvmOpcode.dmul] = DefaultBakery,
		[JvmOpcode.dneg] = DefaultBakery,
		[JvmOpcode.drem] = DefaultBakery,
		[JvmOpcode.dreturn] = DefaultBakery,
		[JvmOpcode.dstore_0] = DefaultBakery,
		[JvmOpcode.dstore_1] = DefaultBakery,
		[JvmOpcode.dstore_2] = DefaultBakery,
		[JvmOpcode.dstore_3] = DefaultBakery,
		[JvmOpcode.dsub] = DefaultBakery,
		[JvmOpcode.dup] = DefaultBakery,
		[JvmOpcode.dup_x1] = DefaultBakery,
		[JvmOpcode.dup_x2] = DefaultBakery,
		[JvmOpcode.dup2] = DefaultBakery,
		[JvmOpcode.dup2_x1] = DefaultBakery,
		[JvmOpcode.dup2_x2] = DefaultBakery,
		[JvmOpcode.f2d] = DefaultBakery,
		[JvmOpcode.f2i] = DefaultBakery,
		[JvmOpcode.f2l] = DefaultBakery,
		[JvmOpcode.fadd] = DefaultBakery,
		[JvmOpcode.faload] = DefaultBakery,
		[JvmOpcode.fastore] = DefaultBakery,
		[JvmOpcode.fcmpg] = DefaultBakery,
		[JvmOpcode.fcmpl] = DefaultBakery,
		[JvmOpcode.fconst_0] = DefaultBakery,
		[JvmOpcode.fconst_1] = DefaultBakery,
		[JvmOpcode.fconst_2] = DefaultBakery,
		[JvmOpcode.fdiv] = DefaultBakery,
		[JvmOpcode.fload_0] = DefaultBakery,
		[JvmOpcode.fload_1] = DefaultBakery,
		[JvmOpcode.fload_2] = DefaultBakery,
		[JvmOpcode.fload_3] = DefaultBakery,
		[JvmOpcode.fmul] = DefaultBakery,
		[JvmOpcode.fneg] = DefaultBakery,
		[JvmOpcode.frem] = DefaultBakery,
		[JvmOpcode.freturn] = DefaultBakery,
		[JvmOpcode.fstore_0] = DefaultBakery,
		[JvmOpcode.fstore_1] = DefaultBakery,
		[JvmOpcode.fstore_2] = DefaultBakery,
		[JvmOpcode.fstore_3] = DefaultBakery,
		[JvmOpcode.fsub] = DefaultBakery,
		[JvmOpcode.i2b] = DefaultBakery,
		[JvmOpcode.i2c] = DefaultBakery,
		[JvmOpcode.i2d] = DefaultBakery,
		[JvmOpcode.i2f] = DefaultBakery,
		[JvmOpcode.i2l] = DefaultBakery,
		[JvmOpcode.i2s] = DefaultBakery,
		[JvmOpcode.iadd] = DefaultBakery,
		[JvmOpcode.iaload] = DefaultBakery,
		[JvmOpcode.iand] = DefaultBakery,
		[JvmOpcode.iastore] = DefaultBakery,
		[JvmOpcode.iconst_m1] = DefaultBakery,
		[JvmOpcode.iconst_0] = DefaultBakery,
		[JvmOpcode.iconst_1] = DefaultBakery,
		[JvmOpcode.iconst_2] = DefaultBakery,
		[JvmOpcode.iconst_3] = DefaultBakery,
		[JvmOpcode.iconst_4] = DefaultBakery,
		[JvmOpcode.iconst_5] = DefaultBakery,
		[JvmOpcode.idiv] = DefaultBakery,
		[JvmOpcode.iload_0] = DefaultBakery,
		[JvmOpcode.iload_1] = DefaultBakery,
		[JvmOpcode.iload_2] = DefaultBakery,
		[JvmOpcode.iload_3] = DefaultBakery,
		[JvmOpcode.impdep1] = DefaultBakery,
		[JvmOpcode.impdep2] = DefaultBakery,
		[JvmOpcode.imul] = DefaultBakery,
		[JvmOpcode.ineg] = DefaultBakery,
		[JvmOpcode.ior] = DefaultBakery,
		[JvmOpcode.irem] = DefaultBakery,
		[JvmOpcode.ireturn] = DefaultBakery,
		[JvmOpcode.ishl] = DefaultBakery,
		[JvmOpcode.ishr] = DefaultBakery,
		[JvmOpcode.istore_0] = DefaultBakery,
		[JvmOpcode.istore_1] = DefaultBakery,
		[JvmOpcode.istore_2] = DefaultBakery,
		[JvmOpcode.istore_3] = DefaultBakery,
		[JvmOpcode.isub] = DefaultBakery,
		[JvmOpcode.iushr] = DefaultBakery,
		[JvmOpcode.ixor] = DefaultBakery,
		[JvmOpcode.l2d] = DefaultBakery,
		[JvmOpcode.l2f] = DefaultBakery,
		[JvmOpcode.l2i] = DefaultBakery,
		[JvmOpcode.ladd] = DefaultBakery,
		[JvmOpcode.laload] = DefaultBakery,
		[JvmOpcode.land] = DefaultBakery,
		[JvmOpcode.lastore] = DefaultBakery,
		[JvmOpcode.lcmp] = DefaultBakery,
		[JvmOpcode.lconst_0] = DefaultBakery,
		[JvmOpcode.lconst_1] = DefaultBakery,
		[JvmOpcode.ldiv] = DefaultBakery,
		[JvmOpcode.lload_0] = DefaultBakery,
		[JvmOpcode.lload_1] = DefaultBakery,
		[JvmOpcode.lload_2] = DefaultBakery,
		[JvmOpcode.lload_3] = DefaultBakery,
		[JvmOpcode.lmul] = DefaultBakery,
		[JvmOpcode.lneg] = DefaultBakery,
		[JvmOpcode.lor] = DefaultBakery,
		[JvmOpcode.lrem] = DefaultBakery,
		[JvmOpcode.lreturn] = DefaultBakery,
		[JvmOpcode.lshl] = DefaultBakery,
		[JvmOpcode.lshr] = DefaultBakery,
		[JvmOpcode.lstore_0] = DefaultBakery,
		[JvmOpcode.lstore_1] = DefaultBakery,
		[JvmOpcode.lstore_2] = DefaultBakery,
		[JvmOpcode.lstore_3] = DefaultBakery,
		[JvmOpcode.lsub] = DefaultBakery,
		[JvmOpcode.lushr] = DefaultBakery,
		[JvmOpcode.lxor] = DefaultBakery,
		[JvmOpcode.monitorenter] = DefaultBakery,
		[JvmOpcode.monitorexit] = DefaultBakery,
		[JvmOpcode.nop] = DefaultBakery,
		[JvmOpcode.pop] = DefaultBakery,
		[JvmOpcode.pop2] = DefaultBakery,
		[JvmOpcode.return_void] = DefaultBakery,
		[JvmOpcode.saload] = DefaultBakery,
		[JvmOpcode.sastore] = DefaultBakery,
		[JvmOpcode.swap] = DefaultBakery,
		[JvmOpcode.bipush] = JvmBipushInstruction.Read,
		[JvmOpcode.fload] = JvmIndexedInstruction.ReadNarrow,
		[JvmOpcode.getfield] = JvmClassChildReferenceInstruction.Read,
		[JvmOpcode.invokespecial] = JvmClassChildReferenceInstruction.Read,
		[JvmOpcode.invokevirtual] = JvmClassChildReferenceInstruction.Read,
		[JvmOpcode.ldc] = JvmLoadConstantInstruction.ReadNarrow,
		[JvmOpcode.ldc_w] = JvmLoadConstantInstruction.ReadWide,
		[JvmOpcode.ldc2_w] = JvmLoadConstantInstruction.ReadWide,
		[JvmOpcode.new_obj] = JvmClassReferenceInstruction.Read,
		[JvmOpcode.putfield] = JvmClassChildReferenceInstruction.Read
	};

	private static JvmInstruction DefaultBakery(JavaConstantPool constantpool, JvmOpcode opcode, BinaryReader r)
	{
		return new JvmInstruction(opcode);
	}

	public static SortedDictionary<short, JvmInstruction> ParseInstructions(JavaConstantPool constantPool, byte[] code)
	{
		using var br = new EndiannessAwareBinaryReader(new MemoryStream(code), EndiannessAwareBinaryReader.Endianness.Big);

		var instructions = new SortedDictionary<short, JvmInstruction>();

		while (br.BaseStream.Position < code.Length)
		{
			if (br.BaseStream.Position > short.MaxValue)
				throw new InvalidOperationException($"No opcodes can exist past {short.MaxValue}");

			var opcode = (JvmOpcode)br.ReadByte();
			if (Bakery.TryGetValue(opcode, out var baker))
				instructions[(short)br.BaseStream.Position] = baker.Invoke(constantPool, opcode, br);
			else
			{
				throw new NotSupportedException($"No baker for opcode \"{opcode}\"");
			}
		}

		return instructions;
	}
}