using System.Runtime.InteropServices;
using OpenTK;

namespace Sabine.GraphicsBindings;

public class OSXBindingsContext : IBindingsContext
{
	[DllImport("libdl.dylib", EntryPoint = "NSIsSymbolNameDefined", CharSet = CharSet.Ansi)]
	private static extern bool NSIsSymbolNameDefined(string s);

	[DllImport("libdl.dylib", EntryPoint = "NSLookupAndBindSymbol", CharSet = CharSet.Ansi)]
	private static extern IntPtr NSLookupAndBindSymbol(string s);

	[DllImport("libdl.dylib", EntryPoint = "NSAddressOfSymbol", CharSet = CharSet.Ansi)]
	private static extern IntPtr NSAddressOfSymbol(IntPtr symbol);

	public IntPtr GetProcAddress(string procName)
	{
		var fname = "_" + procName;
		if (!NSIsSymbolNameDefined(fname))
			return IntPtr.Zero;

		var symbol = NSLookupAndBindSymbol(fname);
		if (symbol != IntPtr.Zero)
			symbol = NSAddressOfSymbol(symbol);

		return symbol;
	}
}