using ModelPainter.View;
using OpenTK;
using OpenTK.Input;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace ModelPainter.Render;

public class SurfaceRenderer
{
	private readonly SkControlContext _renderContext;

	private static readonly float _transparentCheckerboardScale = 8f;
	private static readonly SKPaint _transparentCheckerboardPaint;
	private static readonly SKPaint _textPaint;

	static SurfaceRenderer()
	{
		var path = new SKPath();
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

	private readonly object _textureLock = new object();
	private SKBitmap _texture;
	private int _textureWidth;
	private int _textureHeight;
	private VboData _vboData;
	private KeyValuePair<string, string>[] _hudData;

	public SurfaceRenderer(SkControlContext renderContext)
	{
		_renderContext = renderContext;

		_renderContext.MouseMove += OnMouseMove;
		_renderContext.MouseWheel += OnMouseWheel;
		_renderContext.RenderFrame += Render;
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

		_textureWidth = bitmap.Width;
		_textureHeight = bitmap.Height;

		ContentTransformation = SKMatrix.Identity
			.PreConcat(SKMatrix.CreateTranslation((_renderContext.Width - _textureWidth) / 2f, (_renderContext.Height - _textureHeight) / 2f));

		_renderContext.MarkDirty();
	}

	private void Render(object sender, SKPaintSurfaceEventArgs args)
	{
		var checkerboardColor1 = new SKColor(0xFF_EFEFEF);
		var checkerboardColor2 = new SKColor(0xFF_CFCFCF);

		var canvas = args.Surface.Canvas;
		canvas.Clear(checkerboardColor1);

		_transparentCheckerboardPaint.Color = checkerboardColor2;

		var rect = new SKRect(0, 0, args.Info.Width, args.Info.Height);
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

		if (_vboData != null)
		{
			using var paint = new SKPaint()
			{
				Color = 0xFF_000000,
				IsStroke = false
			};

			var size = new Vector2(_textureWidth, _textureHeight);
			for (var i = 0; i < _vboData.Elements.Length; i += 4)
			{
				var p1 = size * _vboData.TexCoords[i];
				var p2 = size * _vboData.TexCoords[i + 1];
				var p3 = size * _vboData.TexCoords[i + 2];
				var p4 = size * _vboData.TexCoords[i + 3];

				canvas.DrawLine(p1.X, p1.Y, p2.X, p2.Y, paint);
				canvas.DrawLine(p2.X, p2.Y, p3.X, p3.Y, paint);
				canvas.DrawLine(p3.X, p3.Y, p4.X, p4.Y, paint);
				canvas.DrawLine(p4.X, p4.Y, p1.X, p1.Y, paint);
			}

			paint.IsStroke = true;
			canvas.DrawRect(0, 0, _textureWidth, _textureHeight, paint);
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
		_vboData = vboData;
	}
}