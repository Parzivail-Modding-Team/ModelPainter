using System.ComponentModel;

namespace ModelPainterCore.Util;

public class UvMapGenOptions
{
	[
		Category("UV Map"),
		DisplayName("Resolution"),
		Description("Horizontal and vertical resolution, in pixels."),
		DefaultValue(128)
	]
	public int Resolution { get; set; } = 128;

	[
		Category("UV Snapping"),
		DisplayName("Enable Snapping"),
		Description("Whether UV snapping should be enabled."),
		DefaultValue(false)
	]
	public bool UvSnap { get; set; } = false;

	[
		Category("UV Snapping"),
		DisplayName("Max Error"),
		Description("The maximim distance, in pixels, a UV vertex can be from a pixel edge and still be snapped."),
		DefaultValue(0.1f)
	]
	public float UvSnapEpsilon { get; set; } = 0.1f;
}