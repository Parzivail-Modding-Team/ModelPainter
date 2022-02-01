using OpenTK;

namespace ModelPainter.View;

public class GlControlContext : ControlContext<GLControl>
{
	public Action RenderFrame;

	/// <inheritdoc />
	public GlControlContext(GLControl control) : base(control)
	{
		control.Paint += (sender, args) =>
		{
			if (sender is not GLControl gl)
				return;

			gl.MakeCurrent();
			RenderFrame?.Invoke();
			gl.SwapBuffers();
		};
	}

	public void MakeCurrent()
	{
		_control.MakeCurrent();
	}
}