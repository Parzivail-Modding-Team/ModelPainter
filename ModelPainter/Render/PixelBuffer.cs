using OpenTK.Graphics.OpenGL;
using Buffer = System.Buffer;

namespace ModelPainter.Render;

public class PixelBuffer<T> : IDisposable where T : unmanaged
{
	private readonly int _channelsPerPixel;
	private readonly PixelFormat _pixelFormat;
	private readonly PixelType _pixelType;
	private readonly int _bufferId;

	private T[] _data;
	private int _width;
	private int _height;

	public PixelBuffer(int channelsPerPixel, PixelFormat pixelFormat, PixelType pixelType)
	{
		_channelsPerPixel = channelsPerPixel;
		_pixelFormat = pixelFormat;
		_pixelType = pixelType;

		_bufferId = GL.GenBuffer();

		_data = Array.Empty<T>();
	}

	public void Init(int width, int height)
	{
		_width = width;
		_height = height;
		_data = new T[width * height * _channelsPerPixel];
	}

	public T[] GetData()
	{
		return _data;
	}

	public void Read()
	{
		unsafe
		{
			var bufferByteSize = _data.Length * sizeof(T);

			GL.BindBuffer(BufferTarget.PixelPackBuffer, _bufferId);
			GL.BufferData(BufferTarget.PixelPackBuffer, bufferByteSize, IntPtr.Zero, BufferUsageHint.StaticRead);
			GL.ReadPixels(0, 0, _width, _height, _pixelFormat, _pixelType, IntPtr.Zero);
			GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
		}
	}

	public void Copy()
	{
		unsafe
		{
			var bufferByteSize = _data.Length * sizeof(T);

			fixed (T* data = _data)
			{
				GL.BindBuffer(BufferTarget.PixelPackBuffer, _bufferId);
				var pixels = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);
				Buffer.MemoryCopy(pixels.ToPointer(), data, bufferByteSize, bufferByteSize);
				GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
				GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
			}
		}
	}

	private void ReleaseUnmanagedResources()
	{
		GL.DeleteBuffer(_bufferId);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}
}