using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ModelPainterCore.Render.Shader;
using ModelPainterCore.Resources;
using ModelPainterCore.View;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;

namespace ModelPainterCore.Render;

public class ModelRenderer
{
	private static readonly DebugProc DebugCallback = OnGlMessage;

	private const int ObjectIdModeDisabled = 0;
	private const int ObjectIdModeCubeTest = 1;
	private const int ObjectIdModeAlphaTest = 2;
	private const int ObjectIdModeTexCoord = 3;

	private readonly ControlContext _renderContext;
	private readonly Dictionary<uint, Guid> _objectIdMap;
	private readonly VertexBuffer _vbo = new();

	private int _screenVao = -1;
	private int _gizmoVao = -1;
	private Framebuffer _viewFbo;
	private Framebuffer _selectionFbo;
	private Framebuffer _uvFbo;
	private ShaderProgram _shaderScreen;
	private ShaderProgram _shaderModel;
	private ShaderProgram _shaderGizmo;

	private PixelBuffer<uint> _selectionBuffer;
	private PixelBuffer<float> _uvBuffer;

	private uint _selectedCuboidId;
	private int _modelTexture = -1;
	private int _modelOverlayTexture = -1;
	private int _defaultTexture = -1;

	private Camera _camera;

	private Color _loadModelColor = Color.Empty;
	private Color _loadSelectColor = Color.Empty;
	private Color _backgroundColor;

	private int _fovY = 4;
	private float _zoom = 1;
	private Vector2 _rotation = new(-35.264f, 45);
	private Vector3 _translation = Vector3.Zero;

	private Vector2? _loadPointedUv;
	private int _loadBitmapWidth;
	private int _loadBitmapHeight;
	private byte[]? _loadBitmap;

	public ModelRenderer(ControlContext renderContext)
	{
		_renderContext = renderContext;

		_camera = new Camera
		{
			Position = new Vector3(10, 10, 10),
			Rotation = new Vector2(35.264f, -45)
		};

		_objectIdMap = new Dictionary<uint, Guid>();

		_renderContext.MouseMove += OnMouseMove;
		_renderContext.MouseWheel += OnMouseWheel;

		renderContext.MakeCurrent();
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			GL.Enable(EnableCap.DebugOutput);
			GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
		}
	}

	private void CreateScreenVao()
	{
		float[] quadVertices =
		{
			// positions   // texCoords
			-1.0f, 1.0f, 0.0f, 1.0f,
			-1.0f, -1.0f, 0.0f, 0.0f,
			1.0f, -1.0f, 1.0f, 0.0f,

			-1.0f, 1.0f, 0.0f, 1.0f,
			1.0f, -1.0f, 1.0f, 0.0f,
			1.0f, 1.0f, 1.0f, 1.0f
		};

		_screenVao = GL.GenVertexArray();
		var vbo = GL.GenBuffer();
		GL.BindVertexArray(_screenVao);
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices,
			BufferUsageHint.StaticDraw);
		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
		GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices,
			BufferUsageHint.StaticDraw);
		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
		GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		GL.BindVertexArray(0);
	}

	private void CreateGizmoVao()
	{
		float[] gizmo =
		{
			// XYZ,RGB
			0, 0, 0, 1, 0, 0,
			1, 0, 0, 1, 0, 0,
			0, 0, 0, 0, 1, 0,
			0, 1, 0, 0, 1, 0,
			0, 0, 0, 0, 0, 1,
			0, 0, 1, 0, 0, 1,
		};

		_gizmoVao = GL.GenVertexArray();
		var vbo = GL.GenBuffer();
		GL.BindVertexArray(_gizmoVao);
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, gizmo.Length * sizeof(float), gizmo,
			BufferUsageHint.StaticDraw);
		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
		GL.BufferData(BufferTarget.ArrayBuffer, gizmo.Length * sizeof(float), gizmo,
			BufferUsageHint.StaticDraw);
		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
		GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		GL.BindVertexArray(0);
	}

	private void DrawFullscreenQuad()
	{
		GL.Disable(EnableCap.DepthTest);

		GL.ActiveTexture(TextureUnit.Texture0);
		GL.BindTexture(_viewFbo.Samples == 1 ? TextureTarget.Texture2D : TextureTarget.Texture2DMultisample,
			_viewFbo.Texture);

		GL.BindVertexArray(_screenVao);
		GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
		GL.BindVertexArray(0);

		GL.Enable(EnableCap.DepthTest);
	}

	private void DrawGizmo(Matrix4 model, Matrix4 view, Matrix4 perspective)
	{
		GL.Disable(EnableCap.DepthTest);

		_shaderGizmo.Uniforms.SetValue("m", model);
		_shaderGizmo.Uniforms.SetValue("v", view);
		_shaderGizmo.Uniforms.SetValue("p", perspective);
		_shaderGizmo.Use();

		GL.BindVertexArray(_gizmoVao);
		GL.DrawArrays(PrimitiveType.Lines, 0, 6);
		GL.BindVertexArray(0);

		_shaderGizmo.Release();

		GL.Enable(EnableCap.DepthTest);
	}

	public Guid GetObjectIdAt(int x, int y)
	{
		if (_selectionFbo == null || x < 0 || x >= _selectionFbo.Width || y < 0 || y >= _selectionFbo.Height)
			return Guid.Empty;

		y = _selectionFbo.Height - 1 - y;

		var data = _selectionBuffer.GetData();

		var id = data[y * _selectionFbo.Width + x];

		id = (id >> 8) & 0xFFFFFF;

		return _objectIdMap.TryGetValue(id, out var guid) ? guid : Guid.Empty;
	}

	public Vector2? GetObjectUvAt(int x, int y)
	{
		if (_uvFbo == null || x < 0 || x >= _uvFbo.Width || y < 0 || y >= _uvFbo.Height)
			return null;

		var data = _uvBuffer.GetData();

		y = _uvFbo.Height - 1 - y;
		var i = y * _uvFbo.Width + x;
		var r = data[i * 2];
		var g = data[i * 2 + 1];

		return new Vector2(r, g);
	}

	private void Render(object? sender, EventArgs args)
	{
		if (_renderContext.Width != 0 && _renderContext.Height != 0)
			Render(false);
	}

	public void ResetView()
	{
		_zoom = 1;
		_fovY = 4;
		_rotation = new Vector2(-35.264f, 45);
		_translation = Vector3.Zero;

		_renderContext.SwapBuffers();
	}

	public void Render(bool useAlphaTestSelectMode)
	{
		_renderContext.MakeCurrent();

		var width = _renderContext.Width;
		var height = _renderContext.Height;

		if (_screenVao == -1)
		{
			CreateScreenVao();
			CreateGizmoVao();

			GL.GetInteger(GetPName.FramebufferBinding, out var fbo);
			_viewFbo = new Framebuffer(8, unboundFbo: fbo);
			_selectionFbo = new Framebuffer(1, unboundFbo: fbo, forceSingleSampleTexture: true);
			_uvFbo = new Framebuffer(1, PixelInternalFormat.Rg32f, PixelFormat.Rg, PixelType.Float, unboundFbo: fbo,
				forceSingleSampleTexture: true);

			_selectionBuffer = new PixelBuffer<uint>(1, PixelFormat.Bgra, PixelType.UnsignedInt8888);
			_uvBuffer = new PixelBuffer<float>(2, PixelFormat.Rg, PixelType.Float);

			_shaderScreen = new ShaderProgram(ResourceHelper.GetLocalStringResource("screen.frag"),
				ResourceHelper.GetLocalStringResource("screen.vert"));
			_shaderScreen.Uniforms.SetValue("texScene", 0);
			_shaderScreen.Uniforms.SetValue("samplesScene", _viewFbo.Samples);

			_shaderModel = new ShaderProgram(ResourceHelper.GetLocalStringResource("model.frag"),
				ResourceHelper.GetLocalStringResource("model.vert"));
			_shaderModel.Uniforms.SetValue("texModel", 1);
			_shaderModel.Uniforms.SetValue("texOverlay", 2);
			_shaderModel.Uniforms.SetValue("lightPos", new Vector3(0.6f, -1, 0.8f));

			_shaderGizmo = new ShaderProgram(ResourceHelper.GetLocalStringResource("gizmo.frag"),
				ResourceHelper.GetLocalStringResource("gizmo.vert"));

			_defaultTexture = GL.GenTexture();

			GL.ActiveTexture(TextureUnit.Texture1);

			GL.BindTexture(TextureTarget.Texture2D, _defaultTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0, PixelFormat.Bgra,
				PixelType.UnsignedByte, new byte[] { 0, 0, 0, 255 });

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
				(int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
				(int)TextureWrapMode.ClampToEdge);

			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		if (width != _viewFbo.Width || height != _viewFbo.Height)
		{
			_viewFbo.Init(width, height);
			_selectionFbo.Init(width, height);
			_uvFbo.Init(width, height);
			_shaderScreen.Uniforms.SetValue("width", width);
			_shaderScreen.Uniforms.SetValue("height", height);

			_selectionBuffer.Init(width, height);
			_uvBuffer.Init(width, height);
		}

		if (_loadBitmap != null)
		{
			var needsInit = _modelTexture == -1;

			if (needsInit)
				_modelTexture = GL.GenTexture();

			GL.ActiveTexture(TextureUnit.Texture1);

			GL.BindTexture(TextureTarget.Texture2D, _modelTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _loadBitmapWidth, _loadBitmapHeight, 0,
				PixelFormat.Bgra, PixelType.UnsignedByte, _loadBitmap);

			if (needsInit)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
					(int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
					(int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
					(int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
					(int)TextureWrapMode.ClampToEdge);
			}

			GL.BindTexture(TextureTarget.Texture2D, 0);

			_loadBitmap = null;
		}

		if (_loadPointedUv != null)
		{
			var needsInit = _modelOverlayTexture == -1;

			if (needsInit)
				_modelOverlayTexture = GL.GenTexture();

			GL.ActiveTexture(TextureUnit.Texture2);

			GL.BindTexture(TextureTarget.Texture2D, _modelOverlayTexture);
			var pixels = new byte[_loadBitmapWidth * _loadBitmapHeight];

			var x = (int)(_loadPointedUv.Value.X * _loadBitmapWidth);
			var y = (int)(_loadPointedUv.Value.Y * _loadBitmapHeight);

			pixels[y * _loadBitmapWidth + x] = 255;

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, _loadBitmapWidth, _loadBitmapHeight, 0,
				PixelFormat.Red, PixelType.UnsignedByte, pixels);

			if (needsInit)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
					(int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
					(int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
					(int)TextureWrapMode.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
					(int)TextureWrapMode.ClampToEdge);
			}

			GL.BindTexture(TextureTarget.Texture2D, 0);

			_loadBitmap = null;
		}

		if (_loadModelColor != Color.Empty)
		{
			GL.ActiveTexture(TextureUnit.Texture1);

			GL.BindTexture(TextureTarget.Texture2D, _defaultTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0, PixelFormat.Bgra,
				PixelType.UnsignedByte,
				new[] { _loadModelColor.B, _loadModelColor.G, _loadModelColor.R, _loadModelColor.A });

			GL.BindTexture(TextureTarget.Texture2D, 0);

			_loadModelColor = Color.Empty;
		}

		if (_loadSelectColor != Color.Empty)
		{
			_shaderModel.Uniforms.SetValue("selectColor",
				new Vector4(_loadSelectColor.R / 255f, _loadSelectColor.G / 255f, _loadSelectColor.B / 255f,
					_loadSelectColor.A / 255f));
			_loadSelectColor = Color.Empty;
		}

		var aspectRatio = width / (float)height;
		var perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.Pi * 1 / _fovY, aspectRatio, 1f, 65536);

		GL.Viewport(0, 0, width, height);

		GL.ClearColor(Color4.Black);
		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		var view = Matrix4.CreateRotationY((float)(-_rotation.Y / 180 * Math.PI))
		           * Matrix4.CreateRotationX((float)(-_rotation.X / 180 * Math.PI))
		           * Matrix4.CreateScale((float)Math.Pow(10, _zoom / 10f) * Vector3.One)
		           * Matrix4.CreateTranslation(_translation)
		           * Matrix4.CreateTranslation(0, 0, -10);

		var model = Matrix4.CreateTranslation(0, -24 / 16f, 0)
		            * Matrix4.CreateScale(1, -1, 1);

		_shaderModel.Uniforms.SetValue("m", model);
		_shaderModel.Uniforms.SetValue("v", view);
		_shaderModel.Uniforms.SetValue("p", perspective);
		_shaderModel.Uniforms.SetValue("selectedCuboidId", _selectedCuboidId);

		GL.Enable(EnableCap.DepthTest);

		// 3D Viewport
		{
			_viewFbo.Use();
			GL.ClearColor(_backgroundColor);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			DrawGizmo(Matrix4.Identity, view, perspective);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, _modelTexture == -1 ? _defaultTexture : _modelTexture);
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, _modelOverlayTexture == -1 ? 0 : _modelOverlayTexture);

			_shaderModel.Uniforms.SetValue("objectIdMode", ObjectIdModeDisabled);
			_shaderModel.Use();
			_vbo.Render();
			_shaderModel.Release();

			_viewFbo.Release();

			_selectionFbo.Use();
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_shaderModel.Uniforms.SetValue("objectIdMode",
				useAlphaTestSelectMode ? ObjectIdModeAlphaTest : ObjectIdModeCubeTest);
			_shaderModel.Use();
			_vbo.Render();
			_shaderModel.Release();

			_selectionBuffer.Read();
			_selectionFbo.Release();

			_uvFbo.Use();
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_shaderModel.Uniforms.SetValue("objectIdMode", ObjectIdModeTexCoord);
			_shaderModel.Use();
			_vbo.Render();
			_shaderModel.Release();

			_uvBuffer.Read();
			_uvFbo.Release();

			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		_viewFbo.Use();
		// 2D Viewport
		{
			var ortho = Matrix4.CreateOrthographic(width, height, -100, 100);

			var view2d = Matrix4.CreateRotationY((float)(-_rotation.Y / 180 * Math.PI))
			             * Matrix4.CreateRotationX((float)(-_rotation.X / 180 * Math.PI))
			             * Matrix4.CreateScale(25)
			             * Matrix4.CreateTranslation(-width / 2f + 30, -height / 2f + 30, 0);

			DrawGizmo(Matrix4.Identity, view2d, ortho);
		}
		_viewFbo.Release();

		_shaderScreen.Use();
		DrawFullscreenQuad();
		_shaderScreen.Release();

		_selectionBuffer.Copy();
		_uvBuffer.Copy();
	}

	private static void OnGlMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length,
		IntPtr message, IntPtr userparam)
	{
		if (severity == DebugSeverity.DebugSeverityNotification || id is 131184 or 131154)
			return;

		var msg = Marshal.PtrToStringAnsi(message, length);
		if (severity == DebugSeverity.DebugSeverityHigh)
			throw new IOException(msg);

		Console.WriteLine($"[OpenGL] {msg}");
	}

	private static string GetTrimmedStackTrace([CallerMemberName] string? callerName = null)
	{
		return string.Join('\n',
			Environment.StackTrace
				.Split('\n')
				.Where(s => !s.Contains("System.Environment.get_StackTrace") &&
				            !s.Contains(nameof(GetTrimmedStackTrace)) &&
				            (callerName == null || !s.Contains(callerName)))
		);
	}

	public void OnMouseMove(object sender, Vector2 delta)
	{
		if (_renderContext.IsMouseDown(MouseButton.Right))
		{
			_translation.X += delta.X / 50f;
			_translation.Y -= delta.Y / 50f;

			_renderContext.SwapBuffers();
		}
		else if (_renderContext.IsMouseDown(MouseButton.Left))
		{
			_rotation.X -= delta.Y / 2f;
			_rotation.Y -= delta.X / 2f;

			_renderContext.SwapBuffers();
		}
	}

	public void OnMouseWheel(object sender, float delta)
	{
		const int d = 1;
		var m1 = 1; //keyboard[Key.LShift] ? 10 : 1;
		var m2 = 1; //_keyboard[Key.LControl] ? 100 : 1;

		var deltaZoom = delta * m1 * m2;

		_zoom += deltaZoom;

		if (_zoom > 350)
			_zoom = 350;

		if (_zoom < -350)
			_zoom = -350;

		_renderContext.SwapBuffers();
	}

	public void SetSelectedCuboidId(Guid id)
	{
		_selectedCuboidId = id == Guid.Empty ? 0 : _objectIdMap.First(pair => pair.Value == id).Key;
	}

	public void SetTexture(SKBitmap bitmap)
	{
		_renderContext.MakeCurrent();

		_loadBitmap = new byte[bitmap.Width * bitmap.Height * 4];
		Marshal.Copy(bitmap.GetPixels(), _loadBitmap, 0, _loadBitmap.Length);

		_loadBitmapWidth = bitmap.Width;
		_loadBitmapHeight = bitmap.Height;

		_renderContext.SwapBuffers();
	}

	public void UploadModelQuads(VboData data, Dictionary<uint, Guid> objectIdMap)
	{
		_renderContext.MakeCurrent();

		_objectIdMap.Clear();

		foreach (var (oId, oGuid) in objectIdMap) _objectIdMap[oId] = oGuid;

		var (v, n, t, id, e) = data;
		_vbo.InitializeVbo(v, n, t, id, e);

		_renderContext.SwapBuffers();
	}

	public void SetBackgroundColor(Color color)
	{
		_backgroundColor = color;
	}

	public void SetModelColor(Color color)
	{
		_loadModelColor = color;
	}

	public void SetSelectedPixelColor(Color color)
	{
		_loadSelectColor = color;
	}

	public void SetPointedUv(Vector2? pixel)
	{
		if (_loadBitmapWidth == 0 || _loadBitmapHeight == 0 || pixel == null)
			return;

		_loadPointedUv = pixel;
		_renderContext.SwapBuffers();
	}
}