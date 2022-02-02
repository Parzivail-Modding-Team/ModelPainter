using System.Text;
using ModelPainter.Render;
using ModelPainter.Util;
using OpenTK;

namespace ModelPainter.Model.P3D;

public record P3dModel(List<P3dModel.Socket> Sockets, List<P3dModel.Mesh> Meshes)
{
	public record Socket(string Name, string Parent, Matrix4 Transform);

	public record Mesh(string Name, Matrix4 Transform, byte Material, List<Face> Faces, List<Mesh> Children)
	{
		public void Render(MatrixStack matrices, List<VboVertex> vertices, Dictionary<uint, Guid> objectIdMap, ref uint objectId)
		{
			matrices.Push();

			matrices.Multiply(Transform);

			var mat = matrices.Peek();
			foreach (var face in Faces)
			{
				vertices.Add(ModelUtil.Bake(mat, face.V1, face.T1, face.Normal, objectId));
				vertices.Add(ModelUtil.Bake(mat, face.V2, face.T2, face.Normal, objectId));
				vertices.Add(ModelUtil.Bake(mat, face.V3, face.T3, face.Normal, objectId));
				vertices.Add(ModelUtil.Bake(mat, face.V4, face.T4, face.Normal, objectId));
			}

			objectId++;

			foreach (var child in Children)
				child.Render(matrices, vertices, objectIdMap, ref objectId);

			matrices.Pop();
		}
	}

	public record Face(Vector3 Normal, Vector3 V1, Vector2 T1, Vector3 V2, Vector2 T2, Vector3 V3, Vector2 T3, Vector3 V4, Vector2 T4);

	public static P3dModel Load(string filename)
	{
		using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		var br = new BinaryReader(fs);

		var magic = br.ReadBytes(3);
		if (Encoding.ASCII.GetString(magic) != "P3D")
			throw new InvalidDataException();

		var version = br.ReadInt32();

		if (version != 2)
			throw new NotSupportedException("Only P3D version 2 models are supported");

		var numSockets = br.ReadInt32();

		var sockets = new List<Socket>();
		for (var i = 0; i < numSockets; i++)
		{
			var name = br.ReadNullTermString();
			var hasParent = br.ReadBoolean();

			string parent = null;
			if (hasParent)
				parent = br.ReadNullTermString();

			var t = br.ReadStruct<Matrix4>();
			var transform = new Matrix4(
				t.M11, t.M21, t.M31, t.M41,
				t.M12, t.M22, t.M32, t.M42,
				t.M13, t.M23, t.M33, t.M43,
				t.M14, t.M24, t.M34, t.M44
			);

			sockets.Add(new Socket(name, parent, transform));
		}

		var numMeshes = br.ReadInt32();

		var meshes = new List<Mesh>();
		for (var i = 0; i < numMeshes; i++)
			meshes.Add(ReadMesh(br));

		return new P3dModel(sockets, meshes);
	}

	private static Mesh ReadMesh(BinaryReader br)
	{
		var name = br.ReadNullTermString();

		var t = br.ReadStruct<Matrix4>();
		var transform = new Matrix4(
			t.M11, t.M21, t.M31, t.M41,
			t.M12, t.M22, t.M32, t.M42,
			t.M13, t.M23, t.M33, t.M43,
			t.M14, t.M24, t.M34, t.M44
		);

		var mat = br.ReadByte();

		var numFaces = br.ReadInt32();

		var faces = new List<Face>();
		for (var j = 0; j < numFaces; j++)
		{
			var normal = br.ReadVector3();

			var v1 = br.ReadVector3();
			var t1 = br.ReadVector2();

			var v2 = br.ReadVector3();
			var t2 = br.ReadVector2();

			var v3 = br.ReadVector3();
			var t3 = br.ReadVector2();

			var v4 = br.ReadVector3();
			var t4 = br.ReadVector2();

			faces.Add(new Face(normal, v1, t1, v2, t2, v3, t3, v4, t4));
		}

		var numChildren = br.ReadInt32();

		var children = new List<Mesh>();
		for (var j = 0; j < numChildren; j++)
			children.Add(ReadMesh(br));

		return new Mesh(name, transform, mat, faces, children);
	}
}