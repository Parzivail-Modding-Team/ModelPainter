using Gtk;
using ModelPainterCore.Render;
using ModelPainterCore.View;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sabine.GraphicsBindings;
using UI = Gtk.Builder.ObjectAttribute;

namespace Sabine.Views;

class MainWindow : Window
{
    [UI] private GLArea _viewport3d;
    [UI] private GLArea _viewport2d;

    private readonly IBindingsContext _renderBindingsContext = new NativeBindingsContext();
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
        GL.LoadBindings(_renderBindingsContext);

        _modelRenderer = new ModelRenderer(new ControlContext(new GtkGlControlBackend(_viewport3d)));
    }

    private void Viewport3dOnRender(object sender, RenderArgs e)
    {
        _modelRenderer.Render(false);
    }

    private void Viewport2dCreated(object? o, EventArgs args)
    {
        _viewport2d.MakeCurrent();
        _surfaceRenderer = new SurfaceRenderer(new ControlContext(new GtkGlControlBackend(_viewport2d)), _renderBindingsContext);
    }

    private void Viewport2dOnRender(object sender, RenderArgs e)
    {
        _surfaceRenderer.Render();
    }

    private void Window_DeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }
}