using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ModelPainterCore.View;

[Flags]
public enum MouseButtons
{
	None = 0,
	Left = 0b1,
	Middle = 0b10,
	Right = 0b100
}

public record MouseWheelDelta(float Delta);

public interface IControl
{
	public event EventHandler<Vector2> MouseMove; 
	public event EventHandler<MouseButtons> MouseDown; 
	public event EventHandler<MouseButtons> MouseUp; 
	public event EventHandler<MouseWheelDelta> MouseWheel; 

	public int Width { get; }
	public int Height { get; }

	void Invalidate();
	void MakeCurrent();
}

public class ControlContext
{
	protected readonly IControl _control;
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

	/// <inheritdoc />
	public event EventHandler<EventArgs> RenderFrame;

	public ControlContext(IControl control)
	{
		_control = control;

		control.MouseMove += OnMousePositionChange;
		control.MouseDown += (sender, button) => _mouseState |= button;
		control.MouseUp += (sender, button) => _mouseState &= ~button;

		control.MouseWheel += (sender, args) => MouseWheel?.Invoke(sender, args.Delta);
	}

	private void OnMousePositionChange(object? sender, Vector2 e)
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

	public void MakeCurrent()
	{
		_control.MakeCurrent();
	}
}