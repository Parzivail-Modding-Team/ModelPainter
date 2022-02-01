using OpenTK;

namespace ModelPainter.View;

public class GlControlContext : ControlContext<GLControl>
{
	public event EventHandler<EventArgs> RenderFrame;

	/// <inheritdoc />
	public GlControlContext(GLControl control) : base(control)
	{
		control.Paint += (sender, args) =>
		{
			if (sender is not GLControl gl)
				return;

			gl.MakeCurrent();
			RenderFrame?.Invoke(sender, EventArgs.Empty);
			gl.SwapBuffers();
		};
	}
}