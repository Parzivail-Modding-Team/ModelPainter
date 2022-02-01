using System.ComponentModel;
using ModelPainter.Render;
using OpenTK;

namespace ModelPainter.Model
{
	public class ModelPart
	{
		private float _textureWidth = 32;
		private float _textureHeight = 32;

		public readonly Guid Id;

		public event EventHandler<EventArgs> ModelChanged;

		public List<Cuboid> Cuboids { get; } = new();
		public List<ModelPart> Parts { get; } = new();

		public string Name { get; set; } = "Part";

		public float TextureWidth
		{
			get => _textureWidth;
			set
			{
				_textureWidth = value;
				UpdateCuboidTextureSizes();
			}
		}

		public float TextureHeight
		{
			get => _textureHeight;
			set
			{
				_textureHeight = value;
				UpdateCuboidTextureSizes();
			}
		}

		public int TextureOffsetU { get; set; }
		public int TextureOffsetV { get; set; }
		public float PivotX { get; set; }
		public float PivotY { get; set; }
		public float PivotZ { get; set; }
		public float Pitch { get; set; }
		public float Yaw { get; set; }
		public float Roll { get; set; }
		public bool Mirror { get; set; }
		public bool Visible { get; set; } = true;

		public ModelPart(int textureOffsetU, int textureOffsetV)
		{
			Id = Guid.NewGuid();

			TextureOffsetU = textureOffsetU;
			TextureOffsetV = textureOffsetV;
		}

		private void OnChildPartChanged(object sender, EventArgs e)
		{
			if (sender is not ModelPart)
				return;

			ModelChanged?.Invoke(sender, EventArgs.Empty);
		}

		private void OnCuboidChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is not Cuboid || e.PropertyName != nameof(Cuboid.Sides))
				return;

			ModelChanged?.Invoke(sender, EventArgs.Empty);
		}

		private void UpdateCuboidTextureSizes()
		{
			foreach (var cuboid in Cuboids) cuboid.SetTextureSize(TextureWidth, TextureHeight);
		}

		public void SetAnglesRad(float pitch, float yaw, float roll)
		{
			Pitch = MathHelper.RadiansToDegrees(pitch);
			Yaw = MathHelper.RadiansToDegrees(yaw);
			Roll = MathHelper.RadiansToDegrees(roll);
		}

		public void SetPivot(float x, float y, float z)
		{
			PivotX = x;
			PivotY = y;
			PivotZ = z;
		}

		public void AddCuboid(float x, float y, float z, float sizeX, float sizeY, float sizeZ, float extra, float dialation = 0)
		{
			AddCuboid(TextureOffsetU, TextureOffsetV, x, y, z, sizeX, sizeY, sizeZ, extra, extra, extra, Mirror, dialation);
		}

		public void AddCuboid(float x, float y, float z, float sizeX, float sizeY, float sizeZ, float extraX, float extraY, float extraZ, float dialation = 0)
		{
			AddCuboid(TextureOffsetU, TextureOffsetV, x, y, z, sizeX, sizeY, sizeZ, extraX, extraY, extraZ, Mirror, dialation);
		}

		public void AddCuboid(float x, float y, float z, float sizeX, float sizeY, float sizeZ, float extra, bool mirror, float dialation = 0)
		{
			AddCuboid(TextureOffsetU, TextureOffsetV, x, y, z, sizeX, sizeY, sizeZ, extra, extra, extra, mirror, dialation);
		}

		private void AddCuboid(int u, int v, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float extraX, float extraY, float extraZ, bool mirror, float dialation = 0)
		{
			Cuboids.Add(new Cuboid(u, v, x, y, z, sizeX, sizeY, sizeZ, extraX, extraY, extraZ, mirror, TextureWidth, TextureHeight, dialation));
		}

		public void AddChild(ModelPart modelPart)
		{
			Parts.Add(modelPart);
		}

		public void Render(MatrixStack matrices, List<VboVertex> vertices, float dialation, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			if (!Visible || (Cuboids.Count == 0 && Parts.Count == 0)) return;

			matrices.Push();

			Rotate(matrices);
			RenderCuboids(matrices.Peek(), vertices, dialation, objectIdMap, ref objectId);

			foreach (var modelPart in Parts)
				modelPart.Render(matrices, vertices, dialation, objectIdMap, ref objectId);

			matrices.Pop();
		}

		private void RenderCuboids(MatrixStack.Entry matrices, List<VboVertex> vertices, float dialation, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			var modelMat = matrices.Model;
			var normalMat = matrices.Normal;

			foreach (var cuboid in Cuboids)
			{
				cuboid.SetDialation(dialation);

				var sides = cuboid.Sides;

				foreach (var quad in sides)
				{
					var direction = new Vector3(quad.Direction.X, quad.Direction.Y, quad.Direction.Z);
					direction *= normalMat;

					foreach (var vertex in quad.Vertices)
					{
						var x = vertex.Pos.X / 16.0F;
						var y = vertex.Pos.Y / 16.0F;
						var z = vertex.Pos.Z / 16.0F;
						var pos4 = new Vector4(x, y, z, 1.0F);
						pos4 *= modelMat;
						vertices.Add(new VboVertex(pos4.Xyz, new Vector2(vertex.U, vertex.V), direction, objectId));
					}
				}

				objectIdMap[objectId] = cuboid.Id;
				objectId++;
			}
		}

		private void Rotate(MatrixStack matrix)
		{
			matrix.Translate(PivotX / 16.0F, PivotY / 16.0F, PivotZ / 16.0F);
			if (Roll != 0.0F)
				matrix.RotateZ(MathHelper.DegreesToRadians(Roll));

			if (Yaw != 0.0F)
				matrix.RotateY(MathHelper.DegreesToRadians(Yaw));

			if (Pitch != 0.0F)
				matrix.RotateX(MathHelper.DegreesToRadians(Pitch));
		}
	}
}