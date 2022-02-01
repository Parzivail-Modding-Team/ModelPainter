using ModelPainter.View;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SkiaSharp;

namespace ModelPainter.Render;

public class SurfaceRenderer : IDisposable
{
	private const SKColorType ColorType = SKColorType.Rgba8888;
	private const GRSurfaceOrigin SurfaceOrigin = GRSurfaceOrigin.BottomLeft;

	private readonly GlControlContext _renderContext;

	private static readonly float _transparentCheckerboardScale = 8f;
	private static readonly SKPaint _transparentCheckerboardPaint;
	private static readonly SKPaint _textPaint;

	static SurfaceRenderer()
	{
		using var path = new SKPath();
		path.AddRect(new SKRect(0, 0, _transparentCheckerboardScale, _transparentCheckerboardScale));
		var matrix = SKMatrix.CreateScale(2 * _transparentCheckerboardScale, _transparentCheckerboardScale)
			.PreConcat(SKMatrix.CreateSkew(0.5f, 0));
		_transparentCheckerboardPaint = new SKPaint
		{
			PathEffect = SKPathEffect.Create2DPath(matrix, path)
		};

		_textPaint = new SKPaint(new SKFont(SKTypeface.Default, 16))
		{
			Color = SKColors.White,
			Style = SKPaintStyle.Fill
		};
	}

	private readonly object _textureLock = new();
	private readonly SKPath _uvMapPath = new();
	private SKBitmap _texture;
	private int _textureWidth;
	private int _textureHeight;
	private KeyValuePair<string, string>[] _hudData;
	private Vector2? _previewedUv;

	private SKCanvas _canvas;
	private SKSurface _surface;
	private GRGlFramebufferInfo _glInfo;
	private GRContext _grContext;
	private SKSizeI _size;
	private SKSizeI _lastSize;
	private GRBackendRenderTarget _renderTarget;
	private bool _invertColors;

	public SurfaceRenderer(GlControlContext renderContext)
	{
		_renderContext = renderContext;

		_renderContext.MouseMove += OnMouseMove;
		_renderContext.MouseWheel += OnMouseWheel;
		_renderContext.RenderFrame = Render;
	}

	public SKMatrix ContentTransformation { get; set; } = SKMatrix.Identity;

	public void OnMouseMove(object sender, Vector2 delta)
	{
		if (_renderContext.IsMouseDown(MouseButton.Left))
		{
			ContentTransformation = ContentTransformation.PostConcat(SKMatrix.CreateTranslation(delta.X, delta.Y));
			_renderContext.MarkDirty();
		}
	}

	public void OnMouseWheel(object sender, float f)
	{
		var pos = _renderContext.MousePosition;
		var localPos = ContentTransformation.Invert().MapPoint(new SKPoint(pos.X, pos.Y));
		if (f > 0)
			ContentTransformation =
				ContentTransformation.PreConcat(SKMatrix.CreateScale(2, 2, localPos.X, localPos.Y));
		else
			ContentTransformation =
				ContentTransformation.PreConcat(SKMatrix.CreateScale(0.5f, 0.5f, localPos.X, localPos.Y));

		_renderContext.MarkDirty();
	}

	public void SetTexture(SKBitmap bitmap, KeyValuePair<string, string>[] hudData)
	{
		_hudData = hudData;

		lock (_textureLock)
		{
			_texture?.Dispose();
			_texture = bitmap;
		}

		if (bitmap.Width != _textureWidth || bitmap.Height != _textureHeight)
		{
			_textureWidth = bitmap.Width;
			_textureHeight = bitmap.Height;

			ContentTransformation = SKMatrix.Identity
				.PreConcat(SKMatrix.CreateTranslation((_renderContext.Width - _textureWidth) / 2f, (_renderContext.Height - _textureHeight) / 2f));
		}

		_renderContext.MarkDirty();
	}

	public void SetPreviewedUv(Vector2? uv)
	{
		_previewedUv = uv;

		_renderContext.MarkDirty();
	}

	private void Render()
	{
		const ClearBufferMask bits = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
		                             ClearBufferMask.StencilBufferBit;
		// Reset the view
		GL.Clear(bits);

		var width = Math.Max(_renderContext.Width, 1);
		var height = Math.Max(_renderContext.Height, 1);

		// get the new surface size
		_size = new SKSizeI(width, height);

		// create the contexts if not done already
		if (_grContext == null)
		{
			var glInterface = GRGlInterface.Create();
			_grContext = GRContext.CreateGl(glInterface);
		}

		// manage the drawing surface
		if (_renderTarget == null || _lastSize != _size || !_renderTarget.IsValid)
		{
			// create or update the dimensions
			_lastSize = _size;

			// GL.GetInteger(GetPName.StencilBits, out var stencil);
			GL.GetInteger(GetPName.Samples, out var samples);
			var maxSamples = _grContext.GetMaxSurfaceSampleCount(ColorType);
			if (samples > maxSamples)
				samples = maxSamples;
			_glInfo = new GRGlFramebufferInfo(0, ColorType.ToGlSizedFormat());

			// destroy the old surface
			_surface?.Dispose();
			_surface = null;
			_canvas = null;

			// re-create the render target
			_renderTarget?.Dispose();
			_renderTarget = new GRBackendRenderTarget(_size.Width, _size.Height, samples, 8, _glInfo);
		}

		// create the surface
		if (_surface == null)
		{
			_surface = SKSurface.Create(_grContext, _renderTarget, SurfaceOrigin, ColorType);
			_canvas = _surface.Canvas;
		}

		if (_surface == null || _canvas == null)
			throw new InvalidOperationException();

		_canvas.Clear(SKColors.White);

		// render the canvas
		using (new SKAutoCanvasRestore(_canvas, true))
			Render(_canvas);

		_canvas.Flush();
	}

	private void Render(SKCanvas canvas)
	{
		var checkerboardColor1 = new SKColor(_invertColors ? 0xFF_2F2F2F : 0xFF_EFEFEF);
		var checkerboardColor2 = new SKColor(_invertColors ? 0xFF_4F4F4F : 0xFF_CFCFCF);

		canvas.Clear(checkerboardColor1);

		_transparentCheckerboardPaint.Color = checkerboardColor2;

		var rect = new SKRect(0, 0, _size.Width, _size.Height);
		rect.Inflate(_transparentCheckerboardScale, _transparentCheckerboardScale);
		canvas.DrawRect(rect, _transparentCheckerboardPaint);

		canvas.Save();

		lock (_textureLock)
		{
			if (_texture != null)
			{
				canvas.SetMatrix(ContentTransformation);
				canvas.DrawBitmap(_texture, 0, 0);
			}
		}

		using var paint = new SKPaint
		{
			Color = _invertColors ? 0xFF_FFFFFF : 0xFF_000000,
			IsStroke = true
		};

		if (_uvMapPath != null)
		{
			canvas.Save();
			canvas.Scale(_textureWidth, _textureHeight);
			canvas.DrawPath(_uvMapPath, paint);
			canvas.Restore();
		}

		if (_previewedUv != null)
		{
			var uv = _previewedUv.Value;
			var u = (int)Math.Floor(uv.X * _textureWidth);
			var v = (int)Math.Floor(uv.Y * _textureHeight);

			var intervals = new[] { 0.2f, 0.2f };

			paint.PathEffect = SKPathEffect.CreateDash(intervals, 0);
			paint.Color = 0xFF_000000;
			canvas.DrawRect(u, v, 1, 1, paint);

			paint.PathEffect = SKPathEffect.CreateDash(intervals, intervals[0]);
			paint.Color = 0xFF_FFFFFF;
			canvas.DrawRect(u, v, 1, 1, paint);
		}

		canvas.Restore();

		if (_hudData != null)
		{
			var lineOffset = _textPaint.FontSpacing + _textPaint.FontMetrics.Leading;
			var y = 10 + lineOffset;
			foreach (var (key, value) in _hudData)
			{
				canvas.DrawText($"{key}: {value}", 10, y, _textPaint);
				y += lineOffset;
			}
		}
	}

	public void ResetView()
	{
		ContentTransformation = SKMatrix.Identity
			.PreConcat(SKMatrix.CreateTranslation((_renderContext.Width - _textureWidth) / 2f, (_renderContext.Height - _textureHeight) / 2f));
		_renderContext.MarkDirty();
	}

	public void SetVboData(VboData vboData)
	{
		_uvMapPath.Reset();

		for (var i = 0; i < vboData.Elements.Length; i += 4)
		{
			var p1 = vboData.TexCoords[i];
			var p2 = vboData.TexCoords[i + 1];
			var p3 = vboData.TexCoords[i + 2];
			var p4 = vboData.TexCoords[i + 3];

			_uvMapPath.MoveTo(p1.X, p1.Y);
			_uvMapPath.LineTo(p2.X, p2.Y);
			_uvMapPath.LineTo(p3.X, p3.Y);
			_uvMapPath.LineTo(p4.X, p4.Y);
			_uvMapPath.Close();
		}

		_uvMapPath.AddRect(new SKRect(0, 0, 1, 1));
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_uvMapPath?.Dispose();
		_texture?.Dispose();
	}

	public void SetInvertColors(bool invert)
	{
		_invertColors = invert;
		_renderContext.MarkDirty();
	}
}