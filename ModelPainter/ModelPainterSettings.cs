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

	public void Save(string filename)
	{
		File.WriteAllText(filename, JsonSerializer.Serialize(this));
	}

	public static ModelPainterSettings Load(string filename)
	{
		return !File.Exists(filename) ? new ModelPainterSettings() : JsonSerializer.Deserialize<ModelPainterSettings>(File.ReadAllText(filename));
	}
}