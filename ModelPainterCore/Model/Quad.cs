using OpenTK.Mathematics;

namespace ModelPainterCore.Model;

public class Quad
{
	public readonly Vertex[] Vertices;
	public readonly Vector3 Direction;

	public Quad(Vertex[] vertices, float u1, float v1, float u2, float v2, float texWidth, float texHeight, bool flip, Vector3 direction, float dialation)
	{
		Vertices = vertices;

		// UV dialation
		var f = dialation / texWidth;
		var g = dialation / texHeight;

		vertices[0] = vertices[0].Remap(u2 / texWidth - f, v1 / texHeight + g);
		vertices[1] = vertices[1].Remap(u1 / texWidth + f, v1 / texHeight + g);
		vertices[2] = vertices[2].Remap(u1 / texWidth + f, v2 / texHeight - g);
		vertices[3] = vertices[3].Remap(u2 / texWidth - f, v2 / texHeight - g);

		if (flip)
		{
			var i = vertices.Length;

			for (var j = 0; j < i / 2; ++j)
			{
				// swap
				(vertices[j], vertices[i - 1 - j]) = (vertices[i - 1 - j], vertices[j]);
			}
		}

		Direction = direction;
		if (flip)
			Direction = new Vector3(-direction.X, direction.Y, direction.Z);
	}
}