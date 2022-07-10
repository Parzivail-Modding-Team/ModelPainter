using Gtk;
using Sabine.Views;

namespace Sabine;

/*
 * S.A.B.I.N.E. - Synchronized Asset/Bitmap Interface for Natural Editing
 */
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Init();

        var app = new Application("com.parzivail.Sabine", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var win = new MainWindow();
        app.AddWindow(win);

        win.Show();
        Application.Run();
    }
}