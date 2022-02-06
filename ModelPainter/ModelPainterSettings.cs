using System.ComponentModel;
using System.Text.Json;

namespace ModelPainter;

public class ModelPainterSettings
{
	[
		Category("2D Viewport"),
		DisplayName("Invert Color Scheme"),
		Description("Whether to use a white UV outline and dark background."),
		DefaultValue(false)
	]
	public bool LightMode2d { get; set; } = false;

	[
		Category("3D Viewport"),
		DisplayName("Background Color"),
		Description("Viewport background color. #RRGGBB"),
		DefaultValue("#505050")
	]
	public string BackgroundColor { get; set; } = "#505050";

	[
		Category("3D Viewport"),
		DisplayName("Untextured Model Color"),
		Description("Viewport background color. #RRGGBB"),
		DefaultValue("#D3D3D3")
	]
	public string ModelColor { get; set; } = "#D3D3D3";

	[
		Category("3D Viewport"),
		DisplayName("Selected Pixel Color"),
		Description("Pixel overlay for selected pixels. #RRGGBB or #AARRGGBB"),
		DefaultValue("#80FFFFFF")
	]
	public string SelectedPixelColor { get; set; } = "#80FFFFFF";

	public void Save(string filename)
	{
		File.WriteAllText(filename, JsonSerializer.Serialize(this));
	}

	public static ModelPainterSettings Load(string filename)
	{
		return !File.Exists(filename) ? new ModelPainterSettings() : JsonSerializer.Deserialize<ModelPainterSettings>(File.ReadAllText(filename));
	}
}