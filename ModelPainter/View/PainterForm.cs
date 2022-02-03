using ModelPainter.Model;
using ModelPainter.Model.DCM;
using ModelPainter.Model.OBJ;
using ModelPainter.Model.P3D;
using ModelPainter.Model.TBL;
using ModelPainter.Model.TCN;
using ModelPainter.Render;
using OpenTK;
using SkiaSharp.Views.Desktop;

namespace ModelPainter.View;

public partial class PainterForm
{
	private GlControlContext _render3dContext;
	private ModelRenderer _renderer3d;
	private GlControlContext _render2dContext;
	private SurfaceRenderer _renderer2d;

	private ManualResetEventSlim _renderHandle = new();
	private CancellationTokenSource _exitTokenSource = new();

	private void SetupRenderer()
	{
		_render3dContext = new GlControlContext(_modelControl, _renderHandle);
		_renderer3d = new ModelRenderer(_render3dContext);

		_render2dContext = new GlControlContext(_imageControl, _renderHandle);
		_renderer2d = new SurfaceRenderer(_render2dContext);

		LoadModelParts(new List<ModelPart>());

		_modelControl.MouseMove += (sender, args) =>
		{
			var pointedUv = _renderer3d.GetObjectUvAt(args.X, args.Y);
			if (pointedUv == Vector2.Zero)
				_renderer2d.SetPreviewedUv(null);
			else
				_renderer2d.SetPreviewedUv(pointedUv);
		};

		_imageWatcher.FileChanged += (sender, stream) =>
		{
			if (stream == null)
				return;

			using (stream)
			{
				try
				{
					using var bmp = new Bitmap(stream);
					bmp.SetResolution(96, 96);

					_renderer3d.SetTexture(bmp);
					_renderer2d.SetTexture(bmp.ToSKBitmap(), null);
				}
				catch (Exception e)
				{
					Console.WriteLine("Error loading image");
					Console.WriteLine(e);
				}
			}
		};
	}

	private void LoadModelParts(List<ModelPart> parts)
	{
		var (modelData, idMap) = ModelBakery.BakeModelParts(parts, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeModelParts(parts);
		_renderer2d.SetVboData(modelData);
	}

	private void LoadTabulaModel(TabulaModel tbl)
	{
		var (modelData, idMap) = ModelBakery.BakeTabula(tbl, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeTabula(tbl);
		_renderer2d.SetVboData(modelData);
	}

	private void LoadTechneModel(TechneModel tcn)
	{
		var (modelData, idMap) = ModelBakery.BakeTechne(tcn, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeTechne(tcn);
		_renderer2d.SetVboData(modelData);
	}

	private void LoadStudioModel(StudioModel dcm)
	{
		var (modelData, idMap) = ModelBakery.BakeStudioModel(dcm, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeStudioModel(dcm);
		_renderer2d.SetVboData(modelData);
	}

	private void LoadP3dModel(P3dModel p3d)
	{
		var (modelData, idMap) = ModelBakery.BakeP3dModel(p3d);
		_renderer3d.UploadModelQuads(modelData, idMap);
		_renderer2d.SetVboData(modelData);
	}

	private void LoadObjModel(ObjModel obj)
	{
		var (modelData, idMap) = ModelBakery.BakeObjModel(obj);
		_renderer3d.UploadModelQuads(modelData, idMap);
		_renderer2d.SetVboData(modelData);
	}

	private void OnSettingsChanged()
	{
		_renderer2d.SetInvertColors(_settings.LightMode2d);
	}

	/// <inheritdoc />
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		new Task(o =>
		{
			while (true)
			{
				// Don't render if there's no work to do
				_renderHandle.Wait(_exitTokenSource.Token);

				_splitContainer.Invoke(() => { _splitContainer.Invalidate(true); });

				_renderHandle.Reset();

				// Bin render requests every 5ms so redrawing doesn't
				// happen too frequently and cause stuttering
				Thread.Sleep(5);
			}
		}, _exitTokenSource.Token).Start();
	}

	/// <inheritdoc />
	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		_exitTokenSource.Cancel();
		base.OnFormClosing(e);
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		_renderer2d.Dispose();
		base.Dispose(disposing);
	}
}