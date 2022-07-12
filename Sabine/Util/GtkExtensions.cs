using System.Runtime.InteropServices;
using Gdk;
using Gtk;
using Window = Gtk.Window;

namespace Sabine.Util;

public static class GtkExtensions
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void d_gtk_window_window_set_icon(IntPtr window, IntPtr icon);
    static d_gtk_window_window_set_icon gtk_window_set_icon = ReflectionHelper.LoadGtkFunction<d_gtk_window_window_set_icon>("gtk_window_set_icon");

    public static void SetIcon(this Window window, Pixbuf pixbuf)
    {
        gtk_window_set_icon(window.Handle, pixbuf.Handle);
    }

    public static void AddFilter(this IFileChooser d, string name, params string[] patterns)
    {
        var f = new FileFilter();
        f.Name = name;
        foreach (var pattern in patterns)
            f.AddPattern(pattern);
        d.AddFilter(f);
    }

    public static bool TrySelectFilename(this FileChooserNative d, out string filename)
    {
        filename = string.Empty;
        if (d.Run() != (int)ResponseType.Accept)
            return false;

        filename = d.Filename;
        return true;
    }
}