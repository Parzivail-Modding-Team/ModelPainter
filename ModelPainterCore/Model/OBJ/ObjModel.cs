using JeremyAnsel.Media.WavefrontObj;

namespace ModelPainterCore.Model.OBJ;

public record ObjModel(ObjFile ObjFile)
{
	public static ObjModel Load(string filename)
	{
		var obj = ObjFile.FromFile(filename);

		return new ObjModel(obj);
	}
}