namespace ModelPainter.Util;

public static class MathUtil
{
	public static float Lerp(float a, float b, float t)
	{
		return (1 - t) * a + t * b;
	}
}