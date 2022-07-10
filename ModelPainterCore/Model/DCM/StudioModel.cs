using ModelPainterCore.Render;
using ModelPainterCore.Util;
using OpenTK.Mathematics;

namespace ModelPainterCore.Model.DCM;

public record StudioModel(string Author, int TextureWidth, int TextureHeight, List<StudioModel.Cube> Children)
{
	public record TexturedVertex(Vector3 Position, float U, float V);

	public record Quad(Vector3 Normal, List<TexturedVertex> Vertices);

	public record Cube(string Name, Vector3 Size, Vector3 RotationPoint, Vector3 Offset, Vector3 Rotation, int U, int V, bool Mirrored, Vector3 CubeGrow, int TextureWidth, int TextureHeight,
		List<Cube> Children)
	{
		public void Render(MatrixStack matrices, List<VboVertex> vertices, float dialation, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			matrices.Push();

			Rotate(matrices);
			RenderCuboid(matrices, vertices, dialation, objectIdMap, ref objectId);

			foreach (var child in Children)
				child.Render(matrices, vertices, dialation, objectIdMap, ref objectId);

			matrices.Pop();
		}

		private void RenderCuboid(MatrixStack stack, List<VboVertex> vertices, float dialation, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			var sides = new Quad[6];

			var w = Size.X;
			var h = Size.Y;
			var d = Size.Z;
			sides[Mirrored ? 1 : 0] = GenerateFaceData(Vector3.UnitX, 2, 1, 0, -1, -1, d, h, w, Mirrored, U, V, TextureWidth, TextureHeight, d, d + h, -d, -h, dialation);
			sides[Mirrored ? 0 : 1] = GenerateFaceData(-Vector3.UnitX, 2, 1, 0, 1, -1, d, h, -w, Mirrored, U, V, TextureWidth, TextureHeight, d + w + d, d + h, -d, -h, dialation);
			sides[2] = GenerateFaceData(Vector3.UnitY, 0, 2, 1, 1, 1, w, d, h, Mirrored, U, V, TextureWidth, TextureHeight, d, 0, w, d, dialation);
			sides[3] = GenerateFaceData(-Vector3.UnitY, 0, 2, 1, 1, -1, w, d, -h, Mirrored, U, V, TextureWidth, TextureHeight, d + w, d, w, -d, dialation);
			sides[4] = GenerateFaceData(Vector3.UnitZ, 0, 1, 2, 1, -1, w, h, d, Mirrored, U, V, TextureWidth, TextureHeight, d + w + d + w, d + h, -w, -h, dialation);
			sides[5] = GenerateFaceData(-Vector3.UnitZ, 0, 1, 2, -1, -1, w, h, -d, Mirrored, U, V, TextureWidth, TextureHeight, d + w, d + h, -w, -h, dialation);

			stack.Push();

			// cubes are rendered around the center instead of a corner
			stack.Translate(w / 32, h / 32, d / 32);

			stack.Translate(Offset.X / 16, Offset.Y / 16, Offset.Z / 16);

			var matrices = stack.Peek();
			var modelMat = matrices.Model;
			var normalMat = matrices.Normal;

			foreach (var (norm, verts) in sides)
			{
				var direction = new Vector3(norm.X, norm.Y, norm.Z);
				direction *= normalMat;

				foreach (var (pos, u, v) in verts)
				{
					var x = pos.X / 16.0F;
					var y = pos.Y / 16.0F;
					var z = pos.Z / 16.0F;
					var pos4 = new Vector4(x, y, z, 1.0F);
					pos4 *= modelMat;
					vertices.Add(new VboVertex(pos4.Xyz, new Vector2(u, v), direction, objectId));
				}
			}

			stack.Pop();

			objectIdMap[objectId] = Guid.NewGuid();
			objectId++;
		}

		private static Quad GenerateFaceData(Vector3 normal, int iU, int iV, int iW, int uD, int vD, float w, float h, float d, bool mirrored, int u, int v, int textureWidth, int textureHeight,
			float offU, float offV, float heightU, float heightV, float dialation)
		{
			var widthHalf = w / 2;
			var heightHalf = h / 2;
			var depthHalf = d / 2;

			if (mirrored)
			{
				offU += heightU;
				heightU *= -1;
			}

			var uMin = (u + offU) / textureWidth;
			var vMin = (v + offV) / textureHeight;
			var uMax = (u + offU + heightU) / textureWidth;
			var vMax = (v + offV + heightV) / textureHeight;

			if (uMax > uMin)
			{
				uMin += dialation / textureWidth;
				uMax -= dialation / textureWidth;
			}
			else
			{
				uMin -= dialation / textureWidth;
				uMax += dialation / textureWidth;
			}

			if (vMax > vMin)
			{
				vMin += dialation / textureHeight;
				vMax -= dialation / textureHeight;
			}
			else
			{
				vMin -= dialation / textureHeight;
				vMax += dialation / textureHeight;
			}

			var tv1 = new TexturedVertex(new Vector3
			{
				[iU] = -widthHalf * uD,
				[iV] = -heightHalf * vD,
				[iW] = depthHalf
			}, uMax, vMax); // 0, 1

			var tv2 = new TexturedVertex(new Vector3
			{
				[iU] = widthHalf * uD,
				[iV] = -heightHalf * vD,
				[iW] = depthHalf
			}, uMin, vMax); // 1, 1

			var tv3 = new TexturedVertex(new Vector3
			{
				[iU] = -widthHalf * uD,
				[iV] = heightHalf * vD,
				[iW] = depthHalf
			}, uMax, vMin); // 0, 0

			var tv4 = new TexturedVertex(new Vector3
			{
				[iU] = widthHalf * uD,
				[iV] = heightHalf * vD,
				[iW] = depthHalf
			}, uMin, vMin); // 1, 0

			return new Quad(normal, new List<TexturedVertex> { tv1, tv2, tv4, tv3 });
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
			throw new NotSupportedException("Only Studio version 2 models are supported");

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