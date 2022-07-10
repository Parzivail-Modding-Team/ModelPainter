using System.Drawing;
using Gtk;
using ModelPainterCore.Render;
using ModelPainterCore.View;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Sabine.GraphicsBindings;
using UI = Gtk.Builder.ObjectAttribute;

namespace Sabine.Views;

class MainWindow : Window
{
    [UI] private GLArea _viewport3d;
    [UI] private GLArea _viewport2d;

    private ModelRenderer _modelRenderer;
    private SurfaceRenderer _surfaceRenderer;

    public MainWindow() : this(new Builder("MainWindow.glade"))
    {
    }

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        builder.Autoconnect(this);

        DeleteEvent += Window_DeleteEvent;

        _viewport3d.Realized += Viewport3dCreated;
        _viewport3d.Render += Viewport3dOnRender;
        _viewport2d.Realized += Viewport2dCreated;
        _viewport2d.Render += Viewport2dOnRender;
    }

    private void Viewport3dCreated(object? o, EventArgs args)
    {
        _viewport3d.MakeCurrent();
        GL.LoadBindings(new NativeBindingsContext());

        _modelRenderer = new ModelRenderer(new ControlContext(new GtkGlControl(_viewport3d)));
    }

    private void Viewport3dOnRender(object sender, RenderArgs e)
    {
        _modelRenderer.Render(false);
    }

    private void Viewport2dCreated(object? o, EventArgs args)
    {
        _viewport2d.MakeCurrent();
        GL.LoadBindings(new NativeBindingsContext());
    }

    private void Viewport2dOnRender(object sender, RenderArgs e)
    {
        _viewport2d.MakeCurrent();
        var fbo = GL.GetInteger(GetPName.FramebufferBinding);

        GL.ClearColor(Color.Lime);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _viewport2d.QueueDraw();
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }
}

internal class GtkGlControl : IControl
{
    private readonly GLArea _viewport;

    public event EventHandler<Vector2> MouseMove;
    public event EventHandler<MouseButtons> MouseDown;
    public event EventHandler<MouseButtons> MouseUp;
    public event EventHandler<MouseWheelDelta> MouseWheel;

    public int Width => _viewport.AllocatedWidth;
    public int Height => _viewport.AllocatedHeight;

    public GtkGlControl(GLArea viewport)
    {
        _viewport = viewport;
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

    public void Invalidate()
    {
        _viewport.QueueRender();
    }

    public void MakeCurrent()
    {
        _viewport.MakeCurrent();
    }
}