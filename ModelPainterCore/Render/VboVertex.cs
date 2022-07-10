using OpenTK.Mathematics;

namespace ModelPainterCore.Render;

public class VboVertex
{
	public Vector3 Position { get; }
	public Vector2 Tex { get; }
	public Vector3 Normal { get; }
	public uint ObjectId { get; }

	public VboVertex(Vector3 position, Vector2 tex, Vector3 normal, uint objectId)
	{
		Position = position;
		Tex = tex;
		Normal = normal;
		ObjectId = objectId;
	}
}