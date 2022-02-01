using SkiaSharp.Views.Desktop;

namespace ModelPainter.View;

public class SkControlContext : ControlContext<SKControl>
{
	public event EventHandler<SKPaintSurfaceEventArgs> RenderFrame;

	/// <inheritdoc />
	public SkControlContext(SKControl control) : base(control)
	{
		control.PaintSurface += (sender, args) => RenderFrame?.Invoke(sender, args);
	}
}