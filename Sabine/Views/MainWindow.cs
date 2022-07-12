using Gdk;
using Gtk;
using ModelPainterCore.Model;
using ModelPainterCore.Model.NEM;
using ModelPainterCore.Render;
using ModelPainterCore.Resources;
using ModelPainterCore.View;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sabine.GraphicsBindings;
using Sabine.Util;
using SkiaSharp;
using UI = Gtk.Builder.ObjectAttribute;
using Window = Gtk.Window;

namespace Sabine.Views;

class MainWindow : Window
{
    [UI] private MenuItem _bOpen;
    [UI] private MenuItem _bPreferences;
    [UI] private MenuItem _bQuit;

    [UI] private MenuItem _bGenUvMap;

    [UI] private MenuItem _bReset3d;
    [UI] private MenuItem _bReset2d;

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

        this.SetIcon(new Pixbuf(ResourceHelper.GetLocalResource("icon.png")));

        DeleteEvent += Window_DeleteEvent;

        _bOpen.Activated += (sender, args) =>
        {
            var fc = new FileChooserNative(Title, this, FileChooserAction.Open, "Open", "Cancel");

            fc.AddFilter("Models and Textures", "*.p3d", "*.dcm", "*.nem", "*.tbl", "*.tcn", "*.obj", "*.png");
            fc.AddFilter("All Files", "*");

            if (fc.TrySelectFilename(out var filename)) 
                LoadModelFile(filename);
        };
        _bQuit.Activated += (sender, args) => Close();

        _bReset2d.Activated += (sender, args) => _surfaceRenderer.ResetView();
        _bReset3d.Activated += (sender, args) => _modelRenderer.ResetView();

        _viewport3d.Realized += Viewport3dCreated;
        _viewport3d.Render += Viewport3dOnRender;
        _viewport3d.MotionNotifyEvent += (sender, args) =>
        {
            var uv = _modelRenderer.GetObjectUvAt((int)args.Event.X, (int)args.Event.Y);
            _modelRenderer.SetPointedUv(uv);
            _surfaceRenderer.SetPointedUv(uv);
        };
        
        _viewport2d.Realized += Viewport2dCreated;
        _viewport2d.Render += Viewport2dOnRender;
        _viewport2d.MotionNotifyEvent += (sender, args) =>
        {
            var uv = _surfaceRenderer.GetSurfaceUvAt((int)args.Event.X, (int)args.Event.Y);
            // _modelRenderer.SetPointedUv(uv);
            _surfaceRenderer.SetPointedUv(uv);
        };
    }

    private void LoadModelFile(string filename)
    {
        if (System.IO.Path.GetExtension(filename) == ".png")
        {
            var bmp = SKBitmap.Decode(File.ReadAllBytes(filename));
            _surfaceRenderer.SetTexture(bmp, Array.Empty<KeyValuePair<string, string>>());
            _modelRenderer.SetTexture(bmp);
        }
        else
        {
            var nem = NbtEntityModel.Load(filename);
            LoadModelParts(nem.Parts);
        }
    }
    
    private void LoadModelParts(List<ModelPart> parts)
    {
        var (modelData, idMap) = ModelBakery.BakeModelParts(parts, 0.04f);
        _modelRenderer.UploadModelQuads(modelData, idMap);

        (modelData, _) = ModelBakery.BakeModelParts(parts);
        _surfaceRenderer.SetVboData(modelData);
        // _uvMapRenderer.SetVboData(modelData);
    }

    private void Viewport3dCreated(object? o, EventArgs args)
    {
        _viewport3d.MakeCurrent();
        GL.LoadBindings(_renderBindingsContext);

        var context = new ControlContext(new GtkGlControlBackend(_viewport3d));
        
        _modelRenderer = new ModelRenderer(context);
    }

    private void Viewport3dOnRender(object sender, RenderArgs e)
    {
        _modelRenderer.Render(false);
    }

    private void Viewport2dCreated(object? o, EventArgs args)
    {
        _viewport2d.MakeCurrent();
        var context = new ControlContext(new GtkGlControlBackend(_viewport2d));
        _surfaceRenderer = new SurfaceRenderer(context, _renderBindingsContext);
        
        // LoadModelFile("/home/cnewman/IdeaProjects/GalaxiesParzisStarWarsMod/core/src/main/resources/assets/pswg/textures/armor/sandtrooper_default.png");
        LoadModelFile("/home/cnewman/IdeaProjects/GalaxiesParzisStarWarsMod/core/src/main/resources/assets/pswg/models/armor/sandtrooper_default.nem");
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