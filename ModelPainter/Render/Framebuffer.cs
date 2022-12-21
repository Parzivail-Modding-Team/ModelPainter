using OpenTK.Graphics.OpenGL;

namespace ModelPainter.Render
{
	public class Framebuffer : IDisposable
	{
		private readonly int _unboundFbo;

		private int _samples;
		private readonly PixelInternalFormat _internalFormat;
		private readonly PixelFormat _format;
		private readonly PixelType _pixelType;
		public int FboId { get; set; }
		public int Texture { get; set; }
		public int DepthId { get; set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public int Samples
		{
			get => _samples;
			set
			{
				_samples = value;
				Init(Width, Height);
			}
		}

		public Framebuffer(int samples, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Rgba,
			PixelType pixelType = PixelType.UnsignedByte,
			int unboundFbo = 0)
		{
			_unboundFbo = unboundFbo;
			_samples = samples;
			_internalFormat = internalFormat;
			_format = format;
			_pixelType = pixelType;
			FboId = GL.GenFramebuffer();
			Texture = GL.GenTexture();
			DepthId = GL.GenRenderbuffer();
		}

		public void Init(int width, int height)
		{
			if (width == 0 || height == 0)
				return;

			Width = width;
			Height = height;

			Use();

			GL.BindTexture(TextureTarget.Texture2D, Texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, _internalFormat, width, height, 0, _format, _pixelType, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture, 0);

			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthId);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, DepthId);

			var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != FramebufferErrorCode.FramebufferComplete)
				throw new ApplicationException($"Framebuffer status expected to be FramebufferComplete, instead was {status}");

			GL.BindTexture(TextureTarget.Texture2D, 0);
			Release();
		}

		public void Use()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, FboId);
		}

		public void Release()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, _unboundFbo);
		}

		private void ReleaseUnmanagedResources()
		{
			GL.DeleteFramebuffer(FboId);
			GL.DeleteTexture(Texture);
			GL.DeleteRenderbuffer(DepthId);
		}

		private void Dispose(bool disposing)
		{
			ReleaseUnmanagedResources();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}