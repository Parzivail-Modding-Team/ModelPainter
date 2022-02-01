using ModelPainter.Render;
using OpenTK;
using OpenTK.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ModelPainter.View;

public class ControlContext<T> : IRenderContext where T : Control
{
	protected readonly T _control;
	private Vector2 _lastMousePos = Vector2.Zero;
	private MouseButtons _mouseState = MouseButtons.None;

	/// <inheritdoc />
	public int Width => _control.Width;

	/// <inheritdoc />
	public int Height => _control.Height;

	/// <inheritdoc />
	public Vector2 MousePosition { get; private set; } = Vector2.Zero;

	/// <inheritdoc />
	public event EventHandler<Vector2> MouseMove;

	/// <inheritdoc />
	public event EventHandler<float> MouseWheel;

	public ControlContext(T control)
	{
		_control = control;

		control.MouseMove += OnMouseMove;
		control.MouseDown += (sender, args) => _mouseState |= args.Button;
		control.MouseUp += (sender, args) => _mouseState &= ~args.Button;

		control.MouseWheel += (sender, args) => MouseWheel?.Invoke(sender, args.Delta);
	}

	private void OnMouseMove(object sender, MouseEventArgs e)
	{
		_lastMousePos = MousePosition;
		MousePosition = new Vector2(e.X, e.Y);

		MouseMove?.Invoke(sender, MousePosition - _lastMousePos);
	}

	/// <inheritdoc />
	public bool IsMouseDown(MouseButton button)
	{
		return button switch
		{
			MouseButton.Left => _mouseState.HasFlag(MouseButtons.Left),
			MouseButton.Middle => _mouseState.HasFlag(MouseButtons.Middle),
			MouseButton.Right => _mouseState.HasFlag(MouseButtons.Right),
			_ => false
		};
	}

	/// <inheritdoc />
	public virtual void MarkDirty()
	{
		_control.Invalidate();
	}
}