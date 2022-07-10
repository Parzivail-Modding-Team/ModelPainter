using OpenTK.Mathematics;

namespace ModelPainterCore.Model;

public class Cuboid
{
	public readonly Guid Id;

	public string Name { get; set; } = "Part";

	public float Dialation { get; private set; }
	public int U { get; private set; }
	public int V { get; private set; }
	public float X { get; private set; }
	public float Y { get; private set; }
	public float Z { get; private set; }
	public float SizeX { get; private set; }
	public float SizeY { get; private set; }
	public float SizeZ { get; private set; }
	public float ExtraX { get; private set; }
	public float ExtraY { get; private set; }
	public float ExtraZ { get; private set; }
	public bool Mirror { get; private set; }
	public float TextureWidth { get; private set; }
	public float TextureHeight { get; private set; }

	public readonly Quad[] Sides;

	public Cuboid(int u, int v, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float extraX, float extraY, float extraZ, bool mirror, float textureWidth, float textureHeight,
		float dialation)
	{
		Dialation = dialation;
		Id = Guid.NewGuid();
		Sides = new Quad[6];

		BuildQuads(u, v, x, y, z, sizeX, sizeY, sizeZ, extraX, extraY, extraZ, mirror, textureWidth, textureHeight, dialation);
	}

	public void SetDialation(float dialation)
	{
		BuildQuads(U, V, X, Y, Z, SizeX, SizeY, SizeZ, ExtraX, ExtraY, ExtraZ, Mirror, TextureWidth, TextureHeight, dialation);
	}

	public void SetMirrored(bool mirror)
	{
		BuildQuads(U, V, X, Y, Z, SizeX, SizeY, SizeZ, ExtraX, ExtraY, ExtraZ, mirror, TextureWidth, TextureHeight, Dialation);
	}

	public void SetTextureSize(float textureWidth, float textureHeight)
	{
		BuildQuads(U, V, X, Y, Z, SizeX, SizeY, SizeZ, ExtraX, ExtraY, ExtraZ, Mirror, textureWidth, textureHeight, Dialation);
	}

	public void SetTexCoords(int u, int v)
	{
		BuildQuads(u, v, X, Y, Z, SizeX, SizeY, SizeZ, ExtraX, ExtraY, ExtraZ, Mirror, TextureWidth, TextureHeight, Dialation);
	}

	public void SetSize(float sizeX, float sizeY, float sizeZ)
	{
		BuildQuads(U, V, X, Y, Z, sizeX, sizeY, sizeZ, ExtraX, ExtraY, ExtraZ, Mirror, TextureWidth, TextureHeight, Dialation);
	}

	public void SetPosition(float x, float y, float z)
	{
		BuildQuads(U, V, x, y, z, SizeX, SizeY, SizeZ, ExtraX, ExtraY, ExtraZ, Mirror, TextureWidth, TextureHeight, Dialation);
	}

	public void SetExtra(float extraX, float extraY, float extraZ)
	{
		BuildQuads(U, V, X, Y, Z, SizeX, SizeY, SizeZ, extraX, extraY, extraZ, Mirror, TextureWidth, TextureHeight, Dialation);
	}

	public void BuildQuads(int u, int v, float x, float y, float z, float sizeX, float sizeY, float sizeZ, float extraX, float extraY, float extraZ, bool mirror, float textureWidth,
		float textureHeight, float dialation)
	{
		U = u;
		V = v;
		X = x;
		Y = y;
		Z = z;
		SizeX = sizeX;
		SizeY = sizeY;
		SizeZ = sizeZ;
		ExtraX = extraX;
		ExtraY = extraY;
		ExtraZ = extraZ;
		Mirror = mirror;
		TextureWidth = textureWidth;
		TextureHeight = textureHeight;

		var f = x + sizeX;
		var g = y + sizeY;
		var h = z + sizeZ;
		x -= extraX;
		y -= extraY;
		z -= extraZ;
		f += extraX;
		g += extraY;
		h += extraZ;
		if (mirror)
		{
			(f, x) = (x, f);
		}

		var vertex = new Vertex(x, y, z, 0.0F, 0.0F);
		var vertex2 = new Vertex(f, y, z, 0.0F, 8.0F);
		var vertex3 = new Vertex(f, g, z, 8.0F, 8.0F);
		var vertex4 = new Vertex(x, g, z, 8.0F, 0.0F);
		var vertex5 = new Vertex(x, y, h, 0.0F, 0.0F);
		var vertex6 = new Vertex(f, y, h, 0.0F, 8.0F);
		var vertex7 = new Vertex(f, g, h, 8.0F, 8.0F);
		var vertex8 = new Vertex(x, g, h, 8.0F, 0.0F);
		var j = u;
		var k = u + sizeZ;
		var l = u + sizeZ + sizeX;
		var m = u + sizeZ + sizeX + sizeX;
		var n = u + sizeZ + sizeX + sizeZ;
		var o = u + sizeZ + sizeX + sizeZ + sizeX;
		var p = v;
		var q = v + sizeZ;
		var r = v + sizeZ + sizeY;
		Sides[2] = new Quad(new[] { vertex6, vertex5, vertex, vertex2 }, k, p, l, q, textureWidth, textureHeight, mirror, new Vector3(0, -1, 0), dialation);
		Sides[3] = new Quad(new[] { vertex3, vertex4, vertex8, vertex7 }, l, p, m, q, textureWidth, textureHeight, mirror, new Vector3(0, 1, 0), dialation);
		Sides[1] = new Quad(new[] { vertex, vertex5, vertex8, vertex4 }, j, q, k, r, textureWidth, textureHeight, mirror, new Vector3(-1, 0, 0), dialation);
		Sides[4] = new Quad(new[] { vertex2, vertex, vertex4, vertex3 }, k, q, l, r, textureWidth, textureHeight, mirror, new Vector3(0, 0, -1), dialation);
		Sides[0] = new Quad(new[] { vertex6, vertex2, vertex3, vertex7 }, l, q, n, r, textureWidth, textureHeight, mirror, new Vector3(1, 0, 0), dialation);
		Sides[5] = new Quad(new[] { vertex5, vertex6, vertex7, vertex8 }, n, q, o, r, textureWidth, textureHeight, mirror, new Vector3(0, 0, 1), dialation);
	}
}