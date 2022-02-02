using ModelPainter.Render;
using ModelPainter.Util;
using OpenTK;

namespace ModelPainter.Model.DCM;

public record StudioModel(string Author, int TextureWidth, int TextureHeight, List<StudioModel.Cube> Children)
{
	public record Cube(string Name, Vector3 Size, Vector3 RotationPoint, Vector3 Offset, Vector3 Rotation, int U, int V, bool Mirrored, Vector3 CubeGrow, int TextureWidth, int TextureHeight,
		List<Cube> Children)
	{
		public void Render(MatrixStack matrices, List<VboVertex> vertices, float dialation, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			matrices.Push();

			Rotate(matrices);
			RenderCuboid(matrices.Peek(), vertices, dialation, objectIdMap, ref objectId);

			foreach (var child in Children)
				child.Render(matrices, vertices, dialation, objectIdMap, ref objectId);

			matrices.Pop();
		}

		private void RenderCuboid(MatrixStack.Entry matrices, List<VboVertex> vertices, float dialation, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			var modelMat = matrices.Model;
			var normalMat = matrices.Normal;

			var cuboid = new Cuboid(U, V, Offset.X, Offset.Y, Offset.Z, Size.X, Size.Y, Size.Z, CubeGrow.X, CubeGrow.Y, CubeGrow.Z, Mirrored, TextureWidth, TextureHeight, dialation);
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

		private void Rotate(MatrixStack matrix)
		{
			matrix.Translate(RotationPoint.X / 16.0F, RotationPoint.Y / 16.0F, RotationPoint.Z / 16.0F);

			if (Rotation.Z != 0.0F)
				matrix.RotateZ(MathHelper.DegreesToRadians(Rotation.Z));

			if (Rotation.Y != 0.0F)
				matrix.RotateY(MathHelper.DegreesToRadians(Rotation.Y));

			if (Rotation.X != 0.0F)
				matrix.RotateX(MathHelper.DegreesToRadians(Rotation.X));
		}
	}

	public static StudioModel Load(string filename)
	{
		using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		var br = new EndiannessAwareBinaryReader(fs, EndiannessAwareBinaryReader.Endianness.Big);

		var version = (int)br.ReadSingle();

		if (version != 2)
			throw new NotSupportedException();

		var author = br.ReadUtf();
		var texWidth = (int)br.ReadSingle();
		var texHeight = (int)br.ReadSingle();

		var cubes = ReadCubes(br, texWidth, texHeight);

		return new StudioModel(author, texWidth, texHeight, cubes);
	}

	private static List<Cube> ReadCubes(EndiannessAwareBinaryReader br, int texWidth, int texHeight)
	{
		var cubes = new List<Cube>();

		var numCubes = (int)br.ReadSingle();

		for (var i = 0; i < numCubes; i++)
		{
			var name = br.ReadUtf();

			var dimensions = ReadVector3(br);

			var rotationPoint = ReadVector3(br);
			var offset = ReadVector3(br);
			var rotation = ReadVector3(br);

			var u = (int)br.ReadSingle();
			var v = (int)br.ReadSingle();

			var mirrored = br.ReadBoolean();

			var cubeGrow = ReadVector3(br);

			var children = ReadCubes(br, texWidth, texHeight);

			cubes.Add(new Cube(name, dimensions, rotationPoint, offset, rotation, u, v, mirrored, cubeGrow, texWidth, texHeight, children));
		}

		return cubes;
	}

	private static Vector3 ReadVector3(BinaryReader r)
	{
		return new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
	}
}