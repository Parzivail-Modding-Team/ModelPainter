using Gdk;
using Gtk;
using ModelPainterCore.View;
using OpenTK.Mathematics;

namespace Sabine.Views;

internal class GtkGlControlBackend : IControlBackend
{
    private readonly GLArea _viewport;

    public event EventHandler<Vector2> MouseMove;
    public event EventHandler<MouseButtons> MouseDown;
    public event EventHandler<MouseButtons> MouseUp;
    public event EventHandler<MouseWheelDelta> MouseWheel;

    public int Width => _viewport.AllocatedWidth;
    public int Height => _viewport.AllocatedHeight;

    public GtkGlControlBackend(GLArea viewport)
    {
        _viewport = viewport;
        _viewport.AddEvents((int)(EventMask.PointerMotionMask | EventMask.ScrollMask | EventMask.ButtonPressMask | EventMask.ButtonReleaseMask));
        _viewport.MotionNotifyEvent += (o, args) =>
        {
            MouseMove?.Invoke(this, new Vector2((float)args.Event.X, (float)args.Event.Y));
        };
        _viewport.ButtonPressEvent += (o, args) =>
        {
            switch (args.Event.Button)
            {
                case 1:
                    MouseDown?.Invoke(this, MouseButtons.Left);
                    break;
                case 2:
                    MouseDown?.Invoke(this, MouseButtons.Middle);
                    break;
                case 3:
                    MouseDown?.Invoke(this, MouseButtons.Right);
                    break;
            }
        };
        _viewport.ButtonReleaseEvent += (o, args) =>
        {
            switch (args.Event.Button)
            {
                case 1:
                    MouseUp?.Invoke(this, MouseButtons.Left);
                    break;
                case 2:
                    MouseUp?.Invoke(this, MouseButtons.Middle);
                    break;
                case 3:
                    MouseUp?.Invoke(this, MouseButtons.Right);
                    break;
            }
        };
        _viewport.ScrollEvent += (o, args) =>
        {
            MouseWheel?.Invoke(this, new MouseWheelDelta((float)args.Event.DeltaY));
        };
    }

    public void SwapBuffers()
    {
        _viewport.QueueRender();
    }

    public void MakeCurrent()
    {
        _viewport.MakeCurrent();
    }
}