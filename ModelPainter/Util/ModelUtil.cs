using ModelPainter.Render;
using OpenTK;

namespace ModelPainter.Util;

static internal class ModelUtil
{
	public static VboVertex Bake(MatrixStack.Entry mat, Vector3 vertex, Vector2 texCoord, Vector3 normal, uint objectId)
	{
		var pos4 = new Vector4(vertex, 1.0F);
		pos4 *= mat.Model;
		return (new VboVertex(pos4.Xyz, new Vector2(texCoord.X, 1 - texCoord.Y), normal * mat.Normal, objectId));
	}
}