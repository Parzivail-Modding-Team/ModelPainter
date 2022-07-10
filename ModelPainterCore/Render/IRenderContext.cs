using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ModelPainterCore.Render;

public interface IRenderContext
{
	int Width { get; }
	int Height { get; }
	Vector2 MousePosition { get; }

	event EventHandler<Vector2> MouseMove;
	event EventHandler<float> MouseWheel;

	bool IsMouseDown(MouseButton button);

	void MarkDirty();
}