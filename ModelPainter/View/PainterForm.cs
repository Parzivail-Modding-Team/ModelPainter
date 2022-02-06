using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ModelPainter.Model;
using ModelPainter.Model.DCM;
using ModelPainter.Model.OBJ;
using ModelPainter.Model.P3D;
using ModelPainter.Model.TBL;
using ModelPainter.Model.TCN;
using ModelPainter.Render;
using ModelPainter.Util;
using SkiaSharp.Views.Desktop;

namespace ModelPainter.View;

public partial class PainterForm
{
	private GlControlContext _render3dContext;
	private ModelRenderer _renderer3d;
	private GlControlContext _render2dContext;
	private SurfaceRenderer _renderer2d;

	private UvMapRenderer _uvMapRenderer;

	private ManualResetEventSlim _renderHandle = new();
	private CancellationTokenSource _exitTokenSource = new();

	private void SetupRenderer()
	{
		_render3dContext = new GlControlContext(_modelControl, _renderHandle);
		_renderer3d = new ModelRenderer(_render3dContext);

		_render2dContext = new GlControlContext(_imageControl, _renderHandle);
		_renderer2d = new SurfaceRenderer(_render2dContext);

		_uvMapRenderer = new UvMapRenderer();

		LoadModelParts(new List<ModelPart>());

		_modelControl.MouseMove += (sender, args) =>
		{
			var pointedUv = _renderer3d.GetObjectUvAt(args.X, args.Y);
			_renderer2d.SetPointedUv(pointedUv);
			_renderer3d.SetPointedUv(pointedUv);
		};

		_imageControl.MouseMove += (sender, args) =>
		{
			var pointedPixel = _renderer2d.GetSurfaceUvAt(args.X, args.Y);
			_renderer2d.SetPointedUv(pointedPixel);
			_renderer3d.SetPointedUv(pointedPixel);
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
		_uvMapRenderer.SetVboData(modelData);
	}

	private void LoadTabulaModel(TabulaModel tbl)
	{
		var (modelData, idMap) = ModelBakery.BakeTabula(tbl, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeTabula(tbl);
		_renderer2d.SetVboData(modelData);
		_uvMapRenderer.SetVboData(modelData);
	}

	private void LoadTechneModel(TechneModel tcn)
	{
		var (modelData, idMap) = ModelBakery.BakeTechne(tcn, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeTechne(tcn);
		_renderer2d.SetVboData(modelData);
		_uvMapRenderer.SetVboData(modelData);
	}

	private void LoadStudioModel(StudioModel dcm)
	{
		var (modelData, idMap) = ModelBakery.BakeStudioModel(dcm, 0.04f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelBakery.BakeStudioModel(dcm);
		_renderer2d.SetVboData(modelData);
		_uvMapRenderer.SetVboData(modelData);
	}

	private void LoadP3dModel(P3dModel p3d)
	{
		var (modelData, idMap) = ModelBakery.BakeP3dModel(p3d);
		_renderer3d.UploadModelQuads(modelData, idMap);
		_renderer2d.SetVboData(modelData);
		_uvMapRenderer.SetVboData(modelData);
	}

	private void LoadObjModel(ObjModel obj)
	{
		var (modelData, idMap) = ModelBakery.BakeObjModel(obj);
		_renderer3d.UploadModelQuads(modelData, idMap);
		_renderer2d.SetVboData(modelData);
		_uvMapRenderer.SetVboData(modelData);
	}

	private void OnSettingsChanged()
	{
		_renderer2d.SetInvertColors(_settings.LightMode2d);
		_renderer3d.SetBackgroundColor(ColorTranslator.FromHtml(_settings.BackgroundColor));
		_renderer3d.SetModelColor(ColorTranslator.FromHtml(_settings.ModelColor));
		_renderer3d.SetSelectedPixelColor(ColorTranslator.FromHtml(_settings.SelectedPixelColor));
	}

	private void GenerateUvMap()
	{
		using var sfd = new SaveFileDialog()
		{
			Filter = "PNG Image|*.png"
		};

		if (sfd.ShowDialog() != DialogResult.OK)
			return;

		var options = new UvMapGenOptions();
		using var optionsForm = new UvMapGenOptionsForm(options);
		if (optionsForm.ShowDialog() != DialogResult.OK)
			return;

		_uvMapRenderer.Generate(options.Resolution, options.UvSnap, options.UvSnapEpsilon, sfd.FileName);
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

internal class UvMapRenderer
{
	private VboData _vboData;

	public void SetVboData(VboData vboData)
	{
		_vboData = vboData;
	}

	public void Generate(int resolution, bool snap, float snapEpsilon, string outputFilename)
	{
		var bmp = new Bitmap(resolution, resolution);
		using (var g = Graphics.FromImage(bmp))
		{
			g.SmoothingMode = SmoothingMode.None;
			g.Clear(Color.Transparent);
			g.TranslateTransform(-0.5f, -0.5f);

			for (var i = 0; i < _vboData.Elements.Length; i += 4)
			{
				var p1 = _vboData.TexCoords[i];
				var p2 = _vboData.TexCoords[i + 1];
				var p3 = _vboData.TexCoords[i + 2];
				var p4 = _vboData.TexCoords[i + 3];

				var normal = _vboData.Normals[i];

				if (normal.X < 0)
					normal.X = -normal.X * 0.7f;
				if (normal.Y < 0)
					normal.Y = -normal.Y * 0.7f;
				if (normal.Z < 0)
					normal.Z = -normal.Z * 0.7f;

				using var p = new Pen(Color.FromArgb(255, (byte)(normal.X * 255), (byte)(normal.Y * 255), (byte)(normal.Z * 255)), 0.5f);
				using var b = new SolidBrush(Color.FromArgb(255, (byte)(normal.X * 255), (byte)(normal.Y * 255), (byte)(normal.Z * 255)));

				g.FillPolygon(b, new[]
				{
					new Point(
						(int)SnapTexCoord(p1.X, snap, resolution, snapEpsilon),
						(int)SnapTexCoord(p1.Y, snap, resolution, snapEpsilon)
					),
					new Point(
						(int)SnapTexCoord(p2.X, snap, resolution, snapEpsilon),
						(int)SnapTexCoord(p2.Y, snap, resolution, snapEpsilon)
					),
					new Point(
						(int)SnapTexCoord(p3.X, snap, resolution, snapEpsilon),
						(int)SnapTexCoord(p3.Y, snap, resolution, snapEpsilon)
					),
					new Point(
						(int)SnapTexCoord(p4.X, snap, resolution, snapEpsilon),
						(int)SnapTexCoord(p4.Y, snap, resolution, snapEpsilon)
					)
				});
			}
		}

		bmp.Save(outputFilename, ImageFormat.Png);
	}

	private static float SnapTexCoord(float f, bool snap, int snapR, float snapE)
	{
		if (!snap)
			return snapR * f;

		var rounded = (float)Math.Round(f * snapR);
		return snapR * (Math.Abs(rounded - f * snapR) < snapE ? rounded / snapR : f);
	}
}