using ModelPainter.View;

namespace ModelPainter;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		Application.SetHighDpiMode(HighDpiMode.SystemAware);
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new PainterForm());
	}
}