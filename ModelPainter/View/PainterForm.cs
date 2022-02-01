using ModelPainter.Model;
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

	private void SetupRenderer()
	{
		_render3dContext = new GlControlContext(_modelControl);
		_renderer3d = new ModelRenderer(_render3dContext);

		_render2dContext = new GlControlContext(_imageControl);
		_renderer2d = new SurfaceRenderer(_render2dContext);

		_modelControl.MouseMove += (sender, args) =>
		{
			if (args.Button != MouseButtons.None)
				return;

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

		var parts = new List<ModelPart>();

		var part1 = new ModelPart(19, 8);
		part1.SetPivot(1.5F, 16.940000534057617F, -2.759999990463257F);
		part1.AddCuboid(-0.5F, 0.0F, -1.5F, 1, 5, 3, 0.0F);
		part1.SetAnglesRad(-0.6980999708175659F, -3.1415927410125732F, -0.08730000257492065F);
		var part2 = new ModelPart(22, 1);
		part2.SetPivot(0.0F, 21.06999969482422F, 1.159999966621399F);
		part2.AddCuboid(-1.5F, -1.0F, -1.0F, 3, 4, 1, 0.0F);
		part2.SetAnglesRad(1.0149999856948853F, 0.0F, 0.0F);
		var part7sub1 = new ModelPart(10, 0);
		part7sub1.SetPivot(0.0F, -2.0F, -1.0F);
		part7sub1.AddCuboid(-1.0F, -0.5F, -2.0F, 2, 1, 4, 0.0F);
		var part3 = new ModelPart(14, 18);
		part3.SetPivot(-1.0F, 22.0F, -1.0499999523162842F);
		part3.AddCuboid(-0.5F, 0.0F, -0.5F, 1, 2, 1, 0.0F);
		part3.SetAnglesRad(0.6682316660881042F, 0.0F, 0.0F);
		var part4 = new ModelPart(19, 8);
		part4.SetPivot(-1.5F, 16.940000534057617F, -2.759999990463257F);
		part4.AddCuboid(-0.5F, 0.0F, -1.5F, 1, 5, 3, 0.0F);
		part4.SetAnglesRad(-0.6980999708175659F, -3.1415927410125732F, 0.08730000257492065F);
		var part5 = new ModelPart(2, 8);
		part5.SetPivot(0.0F, 16.5F, -3.0F);
		part5.AddCuboid(-1.5F, 0.0F, -1.5F, 3, 6, 3, 0.0F);
		part5.SetAnglesRad(0.493699997663498F, 0.0F, 0.0F);
		var part6 = new ModelPart(14, 18);
		part6.SetPivot(1.0F, 22.0F, -1.0499999523162842F);
		part6.AddCuboid(-0.5F, 0.0F, -0.5F, 1, 2, 1, 0.0F);
		part6.SetAnglesRad(0.6682316660881042F, 0.0F, 0.0F);
		var part7sub2 = new ModelPart(2, 18);
		part7sub2.SetPivot(0.0F, -2.1500000953674316F, 0.15000000596046448F);
		part7sub2.AddCuboid(0.0F, -4.0F, -2.0F, 0, 5, 4, 0.0F);
		part7sub2.SetAnglesRad(-0.22139999270439148F, 0.0F, 0.0F);
		var part7 = new ModelPart(2, 2);
		part7.SetPivot(0.0F, 15.6899995803833F, -2.759999990463257F);
		part7.AddCuboid(-1.0F, -1.5F, -1.0F, 2, 3, 2, 0.0F);
		var part7sub3 = new ModelPart(11, 7);
		part7sub3.SetPivot(0.0F, -0.5F, -1.5F);
		part7sub3.AddCuboid(-0.5F, -1.0F, -0.5F, 1, 2, 1, 0.0F);
		var part7sub4 = new ModelPart(16, 7);
		part7sub4.SetPivot(0.0F, -1.75F, -2.450000047683716F);
		part7sub4.AddCuboid(-0.5F, 0.0F, -0.5F, 1, 2, 1, 0.0F);
		part7.AddChild(part7sub1);
		part7.AddChild(part7sub2);
		part7.AddChild(part7sub3);
		part7.AddChild(part7sub4);

		parts.Add(part1);
		parts.Add(part2);
		parts.Add(part3);
		parts.Add(part4);
		parts.Add(part5);
		parts.Add(part6);
		parts.Add(part7);

		var (modelData, idMap) = ModelRenderer.BuildModelQuads(parts, 0.02f);
		_renderer3d.UploadModelQuads(modelData, idMap);

		(modelData, _) = ModelRenderer.BuildModelQuads(parts);
		_renderer2d.SetVboData(modelData);
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		_renderer2d.Dispose();
	}
}