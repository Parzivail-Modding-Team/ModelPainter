using OpenTK.Mathematics;

namespace ModelPainterCore.Extensions;

public static class Matrix3Ext
{
	public static void MultScalar(this Matrix3 m, float scalar)
	{
		m.M11 *= scalar;
		m.M12 *= scalar;
		m.M13 *= scalar;
		m.M21 *= scalar;
		m.M22 *= scalar;
		m.M23 *= scalar;
		m.M31 *= scalar;
		m.M32 *= scalar;
		m.M33 *= scalar;
	}
}