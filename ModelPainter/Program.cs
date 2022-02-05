using ModelPainter.Model.Java;
using ModelPainter.View;

namespace ModelPainter;

class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		CompiledJavaModel.Load(@"E:\Forge\Mods\PSWG\PSWG15\resources\models\armor & clothing\armor\models\PSWG_Armour_Base.class");
		Application.SetHighDpiMode(HighDpiMode.SystemAware);
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new PainterForm());
	}
}