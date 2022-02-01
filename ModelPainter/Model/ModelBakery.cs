using ModelPainter.Render;
using OpenTK;

namespace ModelPainter.Model;

public class ModelBakery
{
	public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeModelParts(List<ModelPart> modelParts, float dialation = 0)
	{
		var objectIdMap = new Dictionary<uint, Guid>();

		var startingId = 1u;

		var vertices = new List<VboVertex>();
		var matrices = new MatrixStack();

		foreach (var part in modelParts)
			part.Render(matrices, vertices, dialation, objectIdMap, ref startingId);

		var v = new Vector3[vertices.Count];
		var n = new Vector3[vertices.Count];
		var t = new Vector2[vertices.Count];
		var id = new uint[vertices.Count];
		var e = new uint[vertices.Count];

		for (var i = 0; i < vertices.Count; i++)
		{
			v[i] = vertices[i].Position;
			n[i] = vertices[i].Normal;
			t[i] = vertices[i].Tex;
			id[i] = vertices[i].ObjectId;
			e[i] = (uint)i;
		}

		return (new VboData(v, n, t, id, e), objectIdMap);
	}
}