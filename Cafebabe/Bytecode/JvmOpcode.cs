using System.Diagnostics.CodeAnalysis;

namespace Cafebabe.Bytecode;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Opcode names are (mostly) as they appear in the JVM spec")]
public enum JvmOpcode : byte
{
	// <summary>
	//	load onto the stack a reference from an array
	// </summary>
	aaload = 0x32,

	// <summary>
	//	store a reference in an array
	// </summary>
	aastore = 0x53,

	// <summary>
	//	push a null reference onto the stack
	// </summary>
	aconst_null = 0x01,

	// <summary>
	//	load a reference onto the stack from a local variable #index
	// </summary>
	aload = 0x19,

	// <summary>
	//	load a reference onto the stack from local variable 0
	// </summary>
	aload_0 = 0x2a,

	// <summary>
	//	load a reference onto the stack from local variable 1
	// </summary>
	aload_1 = 0x2b,

	// <summary>
	//	load a reference onto the stack from local variable 2
	// </summary>
	aload_2 = 0x2c,

	// <summary>
	//	load a reference onto the stack from local variable 3
	// </summary>
	aload_3 = 0x2d,

	// <summary>
	//	create a new array of references of length count and component type identified by the class reference index (indexbyte1 << 8 | indexbyte2) in the constant pool
	// </summary>
	anewarray = 0xbd,

	// <summary>
	//	return a reference from a method
	// </summary>
	areturn = 0xb0,

	// <summary>
	//	get the length of an array
	// </summary>
	arraylength = 0xbe,

	// <summary>
	//	store a reference into a local variable #index
	// </summary>
	astore = 0x3a,

	// <summary>
	//	store a reference into local variable 0
	// </summary>
	astore_0 = 0x4b,

	// <summary>
	//	store a reference into local variable 1
	// </summary>
	astore_1 = 0x4c,

	// <summary>
	//	store a reference into local variable 2
	// </summary>
	astore_2 = 0x4d,

	// <summary>
	//	store a reference into local variable 3
	// </summary>
	astore_3 = 0x4e,

	// <summary>
	//	throws an error or exception (notice that the rest of the stack is cleared, leaving only a reference to the Throwable)
	// </summary>
	athrow = 0xbf,

	// <summary>
	//	load a byte or Boolean value from an array
	// </summary>
	baload = 0x33,

	// <summary>
	//	store a byte or Boolean value into an array
	// </summary>
	bastore = 0x54,

	// <summary>
	//	push a byte onto the stack as an integer value
	// </summary>
	bipush = 0x10,

	// <summary>
	//	reserved for breakpoints in Java debuggers; should not appear in any class file
	// </summary>
	breakpoint = 0xca,

	// <summary>
	//	load a char from an array
	// </summary>
	caload = 0x34,

	// <summary>
	//	store a char into an array
	// </summary>
	castore = 0x55,

	// <summary>
	//	checks whether an objectref is of a certain type, the class reference of which is in the constant pool at index (indexbyte1 << 8 | indexbyte2)
	// </summary>
	checkcast = 0xc0,

	// <summary>
	//	convert a double to a float
	// </summary>
	d2f = 0x90,

	// <summary>
	//	convert a double to an int
	// </summary>
	d2i = 0x8e,

	// <summary>
	//	convert a double to a long
	// </summary>
	d2l = 0x8f,

	// <summary>
	//	add two doubles
	// </summary>
	dadd = 0x63,

	// <summary>
	//	load a double from an array
	// </summary>
	daload = 0x31,

	// <summary>
	//	store a double into an array
	// </summary>
	dastore = 0x52,

	// <summary>
	//	compare two doubles, 1 on NaN
	// </summary>
	dcmpg = 0x98,

	// <summary>
	//	compare two doubles, -1 on NaN
	// </summary>
	dcmpl = 0x97,

	// <summary>
	//	push the constant 0.0 (a double) onto the stack
	// </summary>
	dconst_0 = 0x0e,

	// <summary>
	//	push the constant 1.0 (a double) onto the stack
	// </summary>
	dconst_1 = 0x0f,

	// <summary>
	//	divide two doubles
	// </summary>
	ddiv = 0x6f,

	// <summary>
	//	load a double value from a local variable #index
	// </summary>
	dload = 0x18,

	// <summary>
	//	load a double from local variable 0
	// </summary>
	dload_0 = 0x26,

	// <summary>
	//	load a double from local variable 1
	// </summary>
	dload_1 = 0x27,

	// <summary>
	//	load a double from local variable 2
	// </summary>
	dload_2 = 0x28,

	// <summary>
	//	load a double from local variable 3
	// </summary>
	dload_3 = 0x29,

	// <summary>
	//	multiply two doubles
	// </summary>
	dmul = 0x6b,

	// <summary>
	//	negate a double
	// </summary>
	dneg = 0x77,

	// <summary>
	//	get the remainder from a division between two doubles
	// </summary>
	drem = 0x73,

	// <summary>
	//	return a double from a method
	// </summary>
	dreturn = 0xaf,

	// <summary>
	//	store a double value into a local variable #index
	// </summary>
	dstore = 0x39,

	// <summary>
	//	store a double into local variable 0
	// </summary>
	dstore_0 = 0x47,

	// <summary>
	//	store a double into local variable 1
	// </summary>
	dstore_1 = 0x48,

	// <summary>
	//	store a double into local variable 2
	// </summary>
	dstore_2 = 0x49,

	// <summary>
	//	store a double into local variable 3
	// </summary>
	dstore_3 = 0x4a,

	// <summary>
	//	subtract a double from another
	// </summary>
	dsub = 0x67,

	// <summary>
	//	duplicate the value on top of the stack
	// </summary>
	dup = 0x59,

	// <summary>
	//	insert a copy of the top value into the stack two values from the top. value1 and value2 must not be of the type double or long.
	// </summary>
	dup_x1 = 0x5a,

	// <summary>
	//	insert a copy of the top value into the stack two (if value2 is double or long it takes up the entry of value3, too) or three values (if value2 is neither double nor long) from the top
	// </summary>
	dup_x2 = 0x5b,

	// <summary>
	//	duplicate top two stack words (two values, if value1 is not double nor long; a single value, if value1 is double or long)
	// </summary>
	dup2 = 0x5c,

	// <summary>
	//	duplicate two words and insert beneath third word (see explanation above)
	// </summary>
	dup2_x1 = 0x5d,

	// <summary>
	//	duplicate two words and insert beneath fourth word
	// </summary>
	dup2_x2 = 0x5e,

	// <summary>
	//	convert a float to a double
	// </summary>
	f2d = 0x8d,

	// <summary>
	//	convert a float to an int
	// </summary>
	f2i = 0x8b,

	// <summary>
	//	convert a float to a long
	// </summary>
	f2l = 0x8c,

	// <summary>
	//	add two floats
	// </summary>
	fadd = 0x62,

	// <summary>
	//	load a float from an array
	// </summary>
	faload = 0x30,

	// <summary>
	//	store a float in an array
	// </summary>
	fastore = 0x51,

	// <summary>
	//	compare two floats, 1 on NaN
	// </summary>
	fcmpg = 0x96,

	// <summary>
	//	compare two floats, -1 on NaN
	// </summary>
	fcmpl = 0x95,

	// <summary>
	//	push 0.0f on the stack
	// </summary>
	fconst_0 = 0x0b,

	// <summary>
	//	push 1.0f on the stack
	// </summary>
	fconst_1 = 0x0c,

	// <summary>
	//	push 2.0f on the stack
	// </summary>
	fconst_2 = 0x0d,

	// <summary>
	//	divide two floats
	// </summary>
	fdiv = 0x6e,

	// <summary>
	//	load a float value from a local variable #index
	// </summary>
	fload = 0x17,

	// <summary>
	//	load a float value from local variable 0
	// </summary>
	fload_0 = 0x22,

	// <summary>
	//	load a float value from local variable 1
	// </summary>
	fload_1 = 0x23,

	// <summary>
	//	load a float value from local variable 2
	// </summary>
	fload_2 = 0x24,

	// <summary>
	//	load a float value from local variable 3
	// </summary>
	fload_3 = 0x25,

	// <summary>
	//	multiply two floats
	// </summary>
	fmul = 0x6a,

	// <summary>
	//	negate a float
	// </summary>
	fneg = 0x76,

	// <summary>
	//	get the remainder from a division between two floats
	// </summary>
	frem = 0x72,

	// <summary>
	//	return a float
	// </summary>
	freturn = 0xae,

	// <summary>
	//	store a float value into a local variable #index
	// </summary>
	fstore = 0x38,

	// <summary>
	//	store a float value into local variable 0
	// </summary>
	fstore_0 = 0x43,

	// <summary>
	//	store a float value into local variable 1
	// </summary>
	fstore_1 = 0x44,

	// <summary>
	//	store a float value into local variable 2
	// </summary>
	fstore_2 = 0x45,

	// <summary>
	//	store a float value into local variable 3
	// </summary>
	fstore_3 = 0x46,

	// <summary>
	//	subtract two floats
	// </summary>
	fsub = 0x66,

	// <summary>
	//	get a field value of an object objectref, where the field is identified by field reference in the constant pool index (indexbyte1 << 8 | indexbyte2)
	// </summary>
	getfield = 0xb4,

	// <summary>
	//	get a static field value of a class, where the field is identified by field reference in the constant pool index (indexbyte1 << 8 | indexbyte2)
	// </summary>
	getstatic = 0xb2,

	// <summary>
	//	goes to another instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	goto_s = 0xa7,

	// <summary>
	//	goes to another instruction at branchoffset (signed int constructed from unsigned bytes branchbyte1 << 24 | branchbyte2 << 16 | branchbyte3 << 8 | branchbyte4)
	// </summary>
	goto_w = 0xc8,

	// <summary>
	//	convert an int into a byte
	// </summary>
	i2b = 0x91,

	// <summary>
	//	convert an int into a character
	// </summary>
	i2c = 0x92,

	// <summary>
	//	convert an int into a double
	// </summary>
	i2d = 0x87,

	// <summary>
	//	convert an int into a float
	// </summary>
	i2f = 0x86,

	// <summary>
	//	convert an int into a long
	// </summary>
	i2l = 0x85,

	// <summary>
	//	convert an int into a short
	// </summary>
	i2s = 0x93,

	// <summary>
	//	add two ints
	// </summary>
	iadd = 0x60,

	// <summary>
	//	load an int from an array
	// </summary>
	iaload = 0x2e,

	// <summary>
	//	perform a bitwise AND on two integers
	// </summary>
	iand = 0x7e,

	// <summary>
	//	store an int into an array
	// </summary>
	iastore = 0x4f,

	// <summary>
	//	load the int value −1 onto the stack
	// </summary>
	iconst_m1 = 0x02,

	// <summary>
	//	load the int value 0 onto the stack
	// </summary>
	iconst_0 = 0x03,

	// <summary>
	//	load the int value 1 onto the stack
	// </summary>
	iconst_1 = 0x04,

	// <summary>
	//	load the int value 2 onto the stack
	// </summary>
	iconst_2 = 0x05,

	// <summary>
	//	load the int value 3 onto the stack
	// </summary>
	iconst_3 = 0x06,

	// <summary>
	//	load the int value 4 onto the stack
	// </summary>
	iconst_4 = 0x07,

	// <summary>
	//	load the int value 5 onto the stack
	// </summary>
	iconst_5 = 0x08,

	// <summary>
	//	divide two integers
	// </summary>
	idiv = 0x6c,

	// <summary>
	//	if references are equal, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_acmpeq = 0xa5,

	// <summary>
	//	if references are not equal, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_acmpne = 0xa6,

	// <summary>
	//	if ints are equal, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_icmpeq = 0x9f,

	// <summary>
	//	if value1 is greater than or equal to value2, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_icmpge = 0xa2,

	// <summary>
	//	if value1 is greater than value2, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_icmpgt = 0xa3,

	// <summary>
	//	if value1 is less than or equal to value2, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_icmple = 0xa4,

	// <summary>
	//	if value1 is less than value2, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_icmplt = 0xa1,

	// <summary>
	//	if ints are not equal, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	if_icmpne = 0xa0,

	// <summary>
	//	if value is 0, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifeq = 0x99,

	// <summary>
	//	if value is greater than or equal to 0, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifge = 0x9c,

	// <summary>
	//	if value is greater than 0, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifgt = 0x9d,

	// <summary>
	//	if value is less than or equal to 0, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifle = 0x9e,

	// <summary>
	//	if value is less than 0, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	iflt = 0x9b,

	// <summary>
	//	if value is not 0, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifne = 0x9a,

	// <summary>
	//	if value is not null, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifnonnull = 0xc7,

	// <summary>
	//	if value is null, branch to instruction at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2)
	// </summary>
	ifnull = 0xc6,

	// <summary>
	//	increment local variable #index by signed byte const
	// </summary>
	iinc = 0x84,

	// <summary>
	//	load an int value from a local variable #index
	// </summary>
	iload = 0x15,

	// <summary>
	//	load an int value from local variable 0
	// </summary>
	iload_0 = 0x1a,

	// <summary>
	//	load an int value from local variable 1
	// </summary>
	iload_1 = 0x1b,

	// <summary>
	//	load an int value from local variable 2
	// </summary>
	iload_2 = 0x1c,

	// <summary>
	//	load an int value from local variable 3
	// </summary>
	iload_3 = 0x1d,

	// <summary>
	//	reserved for implementation-dependent operations within debuggers; should not appear in any class file
	// </summary>
	impdep1 = 0xfe,

	// <summary>
	//	reserved for implementation-dependent operations within debuggers; should not appear in any class file
	// </summary>
	impdep2 = 0xff,

	// <summary>
	//	multiply two integers
	// </summary>
	imul = 0x68,

	// <summary>
	//	negate int
	// </summary>
	ineg = 0x74,

	// <summary>
	//	determines if an object objectref is of a given type, identified by class reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	instanceof = 0xc1,

	// <summary>
	//	invokes a dynamic method and puts the result on the stack (might be void); the method is identified by method reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	invokedynamic = 0xba,

	// <summary>
	//	invokes an interface method on object objectref and puts the result on the stack (might be void); the interface method is identified by method reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	invokeinterface = 0xb9,

	// <summary>
	//	invoke instance method on object objectref and puts the result on the stack (might be void); the method is identified by method reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	invokespecial = 0xb7,

	// <summary>
	//	invoke a static method and puts the result on the stack (might be void); the method is identified by method reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	invokestatic = 0xb8,

	// <summary>
	//	invoke virtual method on object objectref and puts the result on the stack (might be void); the method is identified by method reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	invokevirtual = 0xb6,

	// <summary>
	//	bitwise int OR
	// </summary>
	ior = 0x80,

	// <summary>
	//	logical int remainder
	// </summary>
	irem = 0x70,

	// <summary>
	//	return an integer from a method
	// </summary>
	ireturn = 0xac,

	// <summary>
	//	int shift left
	// </summary>
	ishl = 0x78,

	// <summary>
	//	int arithmetic shift right
	// </summary>
	ishr = 0x7a,

	// <summary>
	//	store int value into variable #index
	// </summary>
	istore = 0x36,

	// <summary>
	//	store int value into variable 0
	// </summary>
	istore_0 = 0x3b,

	// <summary>
	//	store int value into variable 1
	// </summary>
	istore_1 = 0x3c,

	// <summary>
	//	store int value into variable 2
	// </summary>
	istore_2 = 0x3d,

	// <summary>
	//	store int value into variable 3
	// </summary>
	istore_3 = 0x3e,

	// <summary>
	//	int subtract
	// </summary>
	isub = 0x64,

	// <summary>
	//	int logical shift right
	// </summary>
	iushr = 0x7c,

	// <summary>
	//	int xor
	// </summary>
	ixor = 0x82,

	// <summary>
	//	jump to subroutine at branchoffset (signed short constructed from unsigned bytes branchbyte1 << 8 | branchbyte2) and place the return address on the stack
	// </summary>
	jsr = 0xa8,

	// <summary>
	//	jump to subroutine at branchoffset (signed int constructed from unsigned bytes branchbyte1 << 24 | branchbyte2 << 16 | branchbyte3 << 8 | branchbyte4) and place the return address on the stack
	// </summary>
	jsr_w = 0xc9,

	// <summary>
	//	convert a long to a double
	// </summary>
	l2d = 0x8a,

	// <summary>
	//	convert a long to a float
	// </summary>
	l2f = 0x89,

	// <summary>
	//	convert a long to a int
	// </summary>
	l2i = 0x88,

	// <summary>
	//	add two longs
	// </summary>
	ladd = 0x61,

	// <summary>
	//	load a long from an array
	// </summary>
	laload = 0x2f,

	// <summary>
	//	bitwise AND of two longs
	// </summary>
	land = 0x7f,

	// <summary>
	//	store a long to an array
	// </summary>
	lastore = 0x50,

	// <summary>
	//	push 0 if the two longs are the same, 1 if value1 is greater than value2, -1 otherwise
	// </summary>
	lcmp = 0x94,

	// <summary>
	//	push 0L (the number zero with type long) onto the stack
	// </summary>
	lconst_0 = 0x09,

	// <summary>
	//	push 1L (the number one with type long) onto the stack
	// </summary>
	lconst_1 = 0x0a,

	// <summary>
	//	push a constant #index from a constant pool (String, int, float, Class, java.lang.invoke.MethodType, java.lang.invoke.MethodHandle, or a dynamically-computed constant) onto the stack
	// </summary>
	ldc = 0x12,

	// <summary>
	//	push a constant #index from a constant pool (String, int, float, Class, java.lang.invoke.MethodType, java.lang.invoke.MethodHandle, or a dynamically-computed constant) onto the stack (wide index is constructed as indexbyte1 << 8 | indexbyte2)
	// </summary>
	ldc_w = 0x13,

	// <summary>
	//	push a constant #index from a constant pool (double, long, or a dynamically-computed constant) onto the stack (wide index is constructed as indexbyte1 << 8 | indexbyte2)
	// </summary>
	ldc2_w = 0x14,

	// <summary>
	//	divide two longs
	// </summary>
	ldiv = 0x6d,

	// <summary>
	//	load a long value from a local variable #index
	// </summary>
	lload = 0x16,

	// <summary>
	//	load a long value from a local variable 0
	// </summary>
	lload_0 = 0x1e,

	// <summary>
	//	load a long value from a local variable 1
	// </summary>
	lload_1 = 0x1f,

	// <summary>
	//	load a long value from a local variable 2
	// </summary>
	lload_2 = 0x20,

	// <summary>
	//	load a long value from a local variable 3
	// </summary>
	lload_3 = 0x21,

	// <summary>
	//	multiply two longs
	// </summary>
	lmul = 0x69,

	// <summary>
	//	negate a long
	// </summary>
	lneg = 0x75,

	// <summary>
	//	a target address is looked up from a table using a key and execution continues from the instruction at that address
	// </summary>
	lookupswitch = 0xab,

	// <summary>
	//	bitwise OR of two longs
	// </summary>
	lor = 0x81,

	// <summary>
	//	remainder of division of two longs
	// </summary>
	lrem = 0x71,

	// <summary>
	//	return a long value
	// </summary>
	lreturn = 0xad,

	// <summary>
	//	bitwise shift left of a long value1 by int value2 positions
	// </summary>
	lshl = 0x79,

	// <summary>
	//	bitwise shift right of a long value1 by int value2 positions
	// </summary>
	lshr = 0x7b,

	// <summary>
	//	store a long value in a local variable #index
	// </summary>
	lstore = 0x37,

	// <summary>
	//	store a long value in a local variable 0
	// </summary>
	lstore_0 = 0x3f,

	// <summary>
	//	store a long value in a local variable 1
	// </summary>
	lstore_1 = 0x40,

	// <summary>
	//	store a long value in a local variable 2
	// </summary>
	lstore_2 = 0x41,

	// <summary>
	//	store a long value in a local variable 3
	// </summary>
	lstore_3 = 0x42,

	// <summary>
	//	subtract two longs
	// </summary>
	lsub = 0x65,

	// <summary>
	//	bitwise shift right of a long value1 by int value2 positions, unsigned
	// </summary>
	lushr = 0x7d,

	// <summary>
	//	bitwise XOR of two longs
	// </summary>
	lxor = 0x83,

	// <summary>
	//	enter monitor for object ("grab the lock" – start of synchronized() section)
	// </summary>
	monitorenter = 0xc2,

	// <summary>
	//	exit monitor for object ("release the lock" – end of synchronized() section)
	// </summary>
	monitorexit = 0xc3,

	// <summary>
	//	create a new array of dimensions dimensions of type identified by class reference in constant pool index (indexbyte1 << 8 | indexbyte2); the sizes of each dimension is identified by count1, [count2, etc.]
	// </summary>
	multianewarray = 0xc5,

	// <summary>
	//	create new object of type identified by class reference in constant pool index (indexbyte1 << 8 | indexbyte2)
	// </summary>
	new_obj = 0xbb,

	// <summary>
	//	create new array with count elements of primitive type identified by atype
	// </summary>
	newarray = 0xbc,

	// <summary>
	//	perform no operation
	// </summary>
	nop = 0x00,

	// <summary>
	//	discard the top value on the stack
	// </summary>
	pop = 0x57,

	// <summary>
	//	discard the top two values on the stack (or one value, if it is a double or long)
	// </summary>
	pop2 = 0x58,

	// <summary>
	//	set field to value in an object objectref, where the field is identified by a field reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	putfield = 0xb5,

	// <summary>
	//	set static field to value in a class, where the field is identified by a field reference index in constant pool (indexbyte1 << 8 | indexbyte2)
	// </summary>
	putstatic = 0xb3,

	// <summary>
	//	continue execution from address taken from a local variable #index (the asymmetry with jsr is intentional)
	// </summary>
	ret = 0xa9,

	// <summary>
	//	return void from method
	// </summary>
	return_void = 0xb1,

	// <summary>
	//	load short from array
	// </summary>
	saload = 0x35,

	// <summary>
	//	store short to array
	// </summary>
	sastore = 0x56,

	// <summary>
	//	push a short onto the stack as an integer value
	// </summary>
	sipush = 0x11,

	// <summary>
	//	swaps two top words on the stack (note that value1 and value2 must not be double or long)
	// </summary>
	swap = 0x5f,

	// <summary>
	//	continue execution from an address in the table at offset index
	// </summary>
	tableswitch = 0xaa,

	// <summary>
	//	execute opcode, where opcode is either iload, fload, aload, lload, dload, istore, fstore, astore, lstore, dstore, or ret, but assume the index is 16 bit; or execute iinc, where the index is 16 bits and the constant to increment by is a signed 16 bit short
	// </summary>
	wide = 0xc4
}