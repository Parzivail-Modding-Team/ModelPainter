using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModelPainterCore.Model.TBL;

public class TabulaModel
{
	private class VersionTestFields
	{
		[JsonPropertyName("parts")] public List<object> Parts { get; set; }

		[JsonPropertyName("cubes")] public List<object> Cubes { get; set; }
	}

	public class Part
	{
		[JsonPropertyName("notes")] public List<string> Notes { get; set; }

		[JsonPropertyName("texWidth")] public int TexWidth { get; set; }

		[JsonPropertyName("texHeight")] public int TexHeight { get; set; }

		[JsonPropertyName("matchProject")] public bool MatchProject { get; set; }

		[JsonPropertyName("texOffX")] public int U { get; set; }

		[JsonPropertyName("texOffY")] public int V { get; set; }

		[JsonPropertyName("rotPX")] public double RotationPointX { get; set; }

		[JsonPropertyName("rotPY")] public double RotationPointY { get; set; }

		[JsonPropertyName("rotPZ")] public double RotationPointZ { get; set; }

		[JsonPropertyName("rotAX")] public double Pitch { get; set; }

		[JsonPropertyName("rotAY")] public double Yaw { get; set; }

		[JsonPropertyName("rotAZ")] public double Roll { get; set; }

		[JsonPropertyName("mirror")] public bool Mirror { get; set; }

		[JsonPropertyName("showModel")] public bool ShowModel { get; set; }

		[JsonPropertyName("boxes")] public List<Box> Boxes { get; set; }

		[JsonPropertyName("children")] public List<Part> Children { get; set; }

		[JsonPropertyName("identifier")] public string Identifier { get; set; }

		[JsonPropertyName("name")] public string Name { get; set; }
	}

	public class Box
	{
		[JsonPropertyName("posX")] public double X { get; set; }

		[JsonPropertyName("posY")] public double Y { get; set; }

		[JsonPropertyName("posZ")] public double Z { get; set; }

		[JsonPropertyName("dimX")] public double SizeX { get; set; }

		[JsonPropertyName("dimY")] public double SizeY { get; set; }

		[JsonPropertyName("dimZ")] public double SizeZ { get; set; }

		[JsonPropertyName("expandX")] public double ExpandX { get; set; }

		[JsonPropertyName("expandY")] public double ExpandY { get; set; }

		[JsonPropertyName("expandZ")] public double ExpandZ { get; set; }

		[JsonPropertyName("texOffX")] public int U { get; set; }

		[JsonPropertyName("texOffY")] public int V { get; set; }

		[JsonPropertyName("identifier")] public string Identifier { get; set; }

		[JsonPropertyName("name")] public string Name { get; set; }

		public Cuboid ToCuboid(Part part, int texWidth, int texHeight, float dialation)
		{
			return new Cuboid(part.U + U, part.V + V, (float)X, (float)Y, (float)Z, (float)SizeX, (float)SizeY, (float)SizeZ, (float)ExpandX, (float)ExpandY, (float)ExpandZ, part.Mirror,
				texWidth, texHeight, dialation);
		}
	}

	[JsonPropertyName("author")] public string Author { get; set; }

	[JsonPropertyName("projVersion")] public int ProjVersion { get; set; }

	[JsonPropertyName("notes")] public List<string> Notes { get; set; }

	[JsonPropertyName("scaleX")] public double ScaleX { get; set; }

	[JsonPropertyName("scaleY")] public double ScaleY { get; set; }

	[JsonPropertyName("scaleZ")] public double ScaleZ { get; set; }

	[JsonPropertyName("texWidth")] public int TexWidth { get; set; }

	[JsonPropertyName("texHeight")] public int TexHeight { get; set; }

	[JsonPropertyName("textureFile")] public string TextureFile { get; set; }

	[JsonPropertyName("textureFileMd5")] public string TextureFileMd5 { get; set; }

	[JsonPropertyName("parts")] public List<Part> Parts { get; set; }

	[JsonPropertyName("partCountProjectLife")]
	public int PartCountProjectLife { get; set; }

	[JsonPropertyName("identifier")] public string Identifier { get; set; }

	[JsonPropertyName("name")] public string Name { get; set; }

	public static TabulaModel Load(string filename)
	{
		using var fs = ZipFile.Open(filename, ZipArchiveMode.Read);

		var modelEntry = fs.Entries.FirstOrDefault(entry => entry.FullName == "model.json");

		if (modelEntry == null)
			throw new InvalidDataException();

		using var modelStream = new StreamReader(modelEntry.Open());
		var modelJson = modelStream.ReadToEnd();

		// Determine the model version -- if it has "parts", it's a modern one.
		// If it has "cubes", it's an old one.
		var versionTest = JsonSerializer.Deserialize<VersionTestFields>(modelJson);

		if (versionTest.Parts != null)
			return JsonSerializer.Deserialize<TabulaModel>(modelJson);

		return Modernize(JsonSerializer.Deserialize<OldTabulaModel>(modelJson));
	}

	private static TabulaModel Modernize(OldTabulaModel oldModel)
	{
		var model = new TabulaModel
		{
			Parts = new List<Part>(),
			TexWidth = oldModel.TextureWidth,
			TexHeight = oldModel.TextureHeight
		};

		foreach (var cube in oldModel.Cubes)
			model.Parts.Add(cube.ToPart(oldModel.TextureWidth, oldModel.TextureHeight));

		return model;
	}

	public List<ModelPart> GetModelParts(float dialation)
	{
		return Parts.Select(part => ToModelPart(part, dialation)).ToList();
	}

	private ModelPart ToModelPart(Part part, float dialation)
	{
		return new ModelPart
		{
			Mirror = part.Mirror,
			Name = part.Name,
			Pitch = (float)part.Pitch,
			Yaw = (float)part.Yaw,
			Roll = (float)part.Roll,
			Visible = part.ShowModel,
			PivotX = (float)part.RotationPointX,
			PivotY = (float)part.RotationPointY,
			PivotZ = (float)part.RotationPointZ,
			TextureWidth = TexWidth,
			TextureHeight = TexHeight,
			Cuboids = part.Boxes.Select(box => box.ToCuboid(part, TexWidth, TexHeight, dialation)).ToList(),
			Parts = part.Children.Select(part1 => ToModelPart(part1, dialation)).ToList()
		};
	}
}