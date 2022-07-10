using ModelPainterCore.View;
using OpenTK;

namespace ModelPainter.View;

public class GlControlContext : ControlContext
{
	private readonly ManualResetEventSlim _dirtyHandle;

	public Action RenderFrame;

	public bool IsDirty { get; private set; } = true;

	/// <inheritdoc />
	public GlControlContext(GLControl control, ManualResetEventSlim dirtyHandle) : base(control)
	{
		_dirtyHandle = dirtyHandle;
		control.Paint += (sender, args) =>
		{
			if (sender is not GLControl gl)
				return;

			gl.MakeCurrent();
			RenderFrame?.Invoke();
			gl.SwapBuffers();
		};
	}

	/// <inheritdoc />
	public override void MarkDirty()
	{
		_dirtyHandle.Set();
	}

	public void MakeCurrent()
	{
		_control.MakeCurrent();
	}
}