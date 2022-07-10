using System.Text.Json.Serialization;

namespace ModelPainterCore.Model.TBL;

public class OldTabulaModel
{
	public class Cube
	{
		[JsonPropertyName("name")] public string Name { get; set; }

		[JsonPropertyName("dimensions")] public List<int> Dimensions { get; set; }

		[JsonPropertyName("position")] public List<double> Position { get; set; }

		[JsonPropertyName("offset")] public List<double> Offset { get; set; }

		[JsonPropertyName("rotation")] public List<double> Rotation { get; set; }

		[JsonPropertyName("scale")] public List<double> Scale { get; set; }

		[JsonPropertyName("txOffset")] public List<int> TxOffset { get; set; }

		[JsonPropertyName("txMirror")] public bool TxMirror { get; set; }

		[JsonPropertyName("mcScale")] public double McScale { get; set; }

		[JsonPropertyName("opacity")] public double Opacity { get; set; }

		[JsonPropertyName("hidden")] public bool Hidden { get; set; }

		[JsonPropertyName("children")] public List<Cube> Children { get; set; }

		[JsonPropertyName("identifier")] public string Identifier { get; set; }

		public TabulaModel.Part ToPart(int texWidth, int texHeight)
		{
			return new TabulaModel.Part
			{
				RotationPointX = Position[0],
				RotationPointY = Position[1],
				RotationPointZ = Position[2],
				Pitch = Rotation[0],
				Yaw = Rotation[1],
				Roll = Rotation[2],
				Mirror = TxMirror,
				Boxes = new List<TabulaModel.Box>
				{
					ToBox()
				},
				TexWidth = texWidth,
				TexHeight = texHeight,
				ShowModel = !Hidden,
				Children = Children.Select(cube => cube.ToPart(texWidth, texHeight)).ToList(),
				Identifier = Identifier,
				Name = Name
			};
		}

		private TabulaModel.Box ToBox()
		{
			return new TabulaModel.Box
			{
				U = TxOffset[0],
				V = TxOffset[1],
				X = Offset[0],
				Y = Offset[1],
				Z = Offset[2],
				SizeX = Dimensions[0],
				SizeY = Dimensions[1],
				SizeZ = Dimensions[2],
				Identifier = Identifier + "_Box",
				Name = Name + "_Box"
			};
		}
	}

	[JsonPropertyName("modelName")] public string ModelName { get; set; }

	[JsonPropertyName("authorName")] public string AuthorName { get; set; }

	[JsonPropertyName("projVersion")] public int ProjVersion { get; set; }

	[JsonPropertyName("textureWidth")] public int TextureWidth { get; set; }

	[JsonPropertyName("textureHeight")] public int TextureHeight { get; set; }

	[JsonPropertyName("scale")] public List<double> Scale { get; set; }

	[JsonPropertyName("cubeGroups")] public List<object> CubeGroups { get; set; }

	[JsonPropertyName("cubes")] public List<Cube> Cubes { get; set; }

	[JsonPropertyName("cubeCount")] public int CubeCount { get; set; }
}