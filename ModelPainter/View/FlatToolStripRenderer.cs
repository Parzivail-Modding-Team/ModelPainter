namespace ModelPainter.View;

public class FlatToolStripRenderer : ToolStripProfessionalRenderer
{
	/// <inheritdoc />
	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip.GetType() != typeof(MenuStrip))
			base.OnRenderToolStripBackground(e);
	}
}