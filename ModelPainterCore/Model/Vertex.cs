using OpenTK.Mathematics;

namespace ModelPainterCore.Model;

public class Vertex
{
	public Vector3 Pos { get; }
	public float U { get; }
	public float V { get; }

	public Vertex(Vector3 pos, float u, float v)
	{
		Pos = pos;
		U = u;
		V = v;
	}

	public Vertex(float x, float y, float z, float u, float v) : this(new Vector3(x, y, z), u, v)
	{
	}

	public Vertex Remap(float u, float v)
	{
		return new Vertex(Pos, u, v);
	}
}