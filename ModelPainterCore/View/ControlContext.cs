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

public interface IControlBackend
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
	protected readonly IControlBackend ControlBackend;
	private Vector2 _lastMousePos = Vector2.Zero;
	private MouseButtons _mouseState = MouseButtons.None;

	/// <inheritdoc />
	public int Width => ControlBackend.Width;

	/// <inheritdoc />
	public int Height => ControlBackend.Height;

	/// <inheritdoc />
	public Vector2 MousePosition { get; private set; } = Vector2.Zero;

	/// <inheritdoc />
	public event EventHandler<Vector2> MouseMove;

	/// <inheritdoc />
	public event EventHandler<float> MouseWheel;

	public ControlContext(IControlBackend controlBackend)
	{
		ControlBackend = controlBackend;

		controlBackend.MouseMove += OnMousePositionChange;
		controlBackend.MouseDown += (sender, button) => _mouseState |= button;
		controlBackend.MouseUp += (sender, button) => _mouseState &= ~button;

		controlBackend.MouseWheel += (sender, args) => MouseWheel?.Invoke(sender, args.Delta);
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
		ControlBackend.Invalidate();
	}

	public void MakeCurrent()
	{
		ControlBackend.MakeCurrent();
	}
}