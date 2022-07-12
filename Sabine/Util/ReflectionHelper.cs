using System.Reflection;

namespace Sabine.Util;

public class ReflectionHelper
{
    public static T LoadGtkFunction<T>(string functionName)
    {
        var assy = typeof(Gtk.Window).Assembly;

        var gLibLoadMethod = GetMethod(assy, "GLibrary", "Load", BindingFlags.Static | BindingFlags.Public);
        var libraryGtk = GetEnumMember(assy, "Library", "Gtk");
        var lib = (IntPtr)gLibLoadMethod.Invoke(null, new[] { libraryGtk });

        var funcLoaderGetProcAddr =
            GetMethod(assy, "FuncLoader", "GetProcAddress", BindingFlags.Static | BindingFlags.Public);
        var procAddr = funcLoaderGetProcAddr.Invoke(null, new object[] { lib, functionName });

        var funcLoaderLoadFunction =
            GetMethod(assy, "FuncLoader", "LoadFunction", BindingFlags.Static | BindingFlags.Public);
        var func = funcLoaderLoadFunction
            .MakeGenericMethod(typeof(T))
            .Invoke(null, new[] { procAddr });

        return (T)func;
    }

    private static MethodInfo GetMethod(Assembly assembly, string className, string methodName, BindingFlags bf)
    {
        var mc = assembly.CreateInstance(className);
        if (mc == null)
            throw new TypeLoadException(nameof(className));

        var t = mc.GetType();
        var mi = t.GetMethod(methodName, bf);
        if (mi == null)
            throw new MethodAccessException(nameof(methodName));

        return mi;
    }

    private static object GetEnumMember(Assembly assembly, string className, string memberName)
    {
        var mc = assembly.CreateInstance(className);
        if (mc == null)
            throw new TypeLoadException(nameof(className));

        var t = mc.GetType();
        var mi = t.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
        if (mi == null)
            throw new FieldAccessException(nameof(memberName));
        var val = mi.GetValue(null);
        if (val == null)
            throw new FieldAccessException(nameof(memberName));

        return val;
    }
}