using ModelPainter.Extensions;
using OpenTK;

namespace ModelPainter.Render
{
	public class MatrixStack
	{
		public class Entry
		{
			public Matrix4 Model { get; set; }
			public Matrix3 Normal { get; set; }

			public Entry(Matrix4 model, Matrix3 normal)
			{
				Model = model;
				Normal = normal;
			}
		}

		private readonly Stack<Entry> _stack;

		public MatrixStack()
		{
			_stack = new Stack<Entry>();
			_stack.Push(new Entry(Matrix4.Identity, Matrix3.Identity));
		}

		public void Translate(double x, double y, double z)
		{
			var entry = Peek();
			entry.Model = Matrix4.CreateTranslation((float)x, (float)y, (float)z) * entry.Model;
		}

		public void RotateX(float amount)
		{
			var entry = Peek();

			entry.Model = Matrix4.CreateRotationX(amount) * entry.Model;
			entry.Normal = Matrix3.CreateRotationX(amount) * entry.Normal;
		}

		public void RotateY(float amount)
		{
			var entry = Peek();

			entry.Model = Matrix4.CreateRotationY(amount) * entry.Model;
			entry.Normal = Matrix3.CreateRotationY(amount) * entry.Normal;
		}

		public void RotateZ(float amount)
		{
			var entry = Peek();

			entry.Model = Matrix4.CreateRotationZ(amount) * entry.Model;
			entry.Normal = Matrix3.CreateRotationZ(amount) * entry.Normal;
		}

		public void Scale(float x, float y, float z)
		{
			var entry = Peek();
			entry.Model = Matrix4.CreateScale(x, y, z) * entry.Model;
			if (x == y && y == z)
			{
				if (x > 0.0F)
				{
					return;
				}

				entry.Normal.MultScalar(-1);
			}

			var f = 1.0F / x;
			var g = 1.0F / y;
			var h = 1.0F / z;
			var i = 1 / Math.Cbrt(f * g * h);
			entry.Normal = Matrix3.CreateScale((float)(i * f), (float)(i * g), (float)(i * h)) * entry.Normal;
		}

		public void Push()
		{
			var entry = Peek();
			_stack.Push(new Entry(entry.Model, entry.Normal));
		}

		public void Pop()
		{
			_stack.Pop();
		}


		public Entry Peek()
		{
			return _stack.Peek();
		}
	}
}