using OpenTK.Graphics.OpenGL;

namespace ModelPainterCore.Util;

public static class GLExt
{
    public static void Label(ObjectLabelIdentifier id, int obj, string name)
    {
        GL.ObjectLabel(id, obj, name.Length, name);
    }
}