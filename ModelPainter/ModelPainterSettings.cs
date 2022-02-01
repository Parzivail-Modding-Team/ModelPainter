using System.ComponentModel;
using System.Text.Json;

namespace ModelPainter;

public class ModelPainterSettings
{
	[
		Category("Temporary"),
		DisplayName("Dummy Setting"),
		Description("Added to make sure the settings menu works."),
		DefaultValue(false)
	]
	public bool DummySetting { get; set; } = false;

	public void Save(string filename)
	{
		File.WriteAllText(filename, JsonSerializer.Serialize(this));
	}

	public static ModelPainterSettings Load(string filename)
	{
		return !File.Exists(filename) ? new ModelPainterSettings() : JsonSerializer.Deserialize<ModelPainterSettings>(File.ReadAllText(filename));
	}
}