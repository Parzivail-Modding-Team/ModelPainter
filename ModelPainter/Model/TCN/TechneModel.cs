using System.IO.Compression;
using System.Xml.Serialization;

namespace ModelPainter.Model.TCN;

[XmlRoot(ElementName = "Techne")]
public class TechneModel
{
	[XmlRoot(ElementName = "Animation")]
	public class Animation
	{
		[XmlElement(ElementName = "AnimationAngles")]
		public string AnimationAngles { get; set; }

		[XmlElement(ElementName = "AnimationDuration")]
		public string AnimationDuration { get; set; }

		[XmlElement(ElementName = "AnimationType")]
		public string AnimationType { get; set; }
	}

	[XmlRoot(ElementName = "Shape")]
	public class Shape
	{
		[XmlElement(ElementName = "Animation")]
		public Animation Animation { get; set; }

		[XmlElement(ElementName = "IsDecorative")]
		public string IsDecorative { get; set; }

		[XmlElement(ElementName = "IsFixed")] public string IsFixed { get; set; }

		[XmlElement(ElementName = "IsMirrored")]
		public string IsMirrored { get; set; }

		[XmlElement(ElementName = "Offset")] public string Offset { get; set; }
		[XmlElement(ElementName = "Position")] public string Position { get; set; }
		[XmlElement(ElementName = "Rotation")] public string Rotation { get; set; }
		[XmlElement(ElementName = "Size")] public string Size { get; set; }

		[XmlElement(ElementName = "TextureOffset")]
		public string TextureOffset { get; set; }

		[XmlAttribute(AttributeName = "type")] public string Type { get; set; }
		[XmlAttribute(AttributeName = "name")] public string Name { get; set; }

		public ModelPart ToModelPart(int texWidth, int texHeight, float dialation)
		{
			var offset = Offset.Split(',').Select(float.Parse).ToArray();
			var position = Position.Split(',').Select(float.Parse).ToArray();
			var rotation = Rotation.Split(',').Select(float.Parse).ToArray();
			var size = Size.Split(',').Select(int.Parse).ToArray();
			var texCoords = TextureOffset.Split(',').Select(int.Parse).ToArray();

			return new ModelPart
			{
				Cuboids =
				{
					ToCuboid(texWidth, texHeight, offset, size, texCoords, dialation)
				},
				Mirror = IsMirrored == "True",
				Pitch = rotation[0],
				Yaw = rotation[1],
				Roll = rotation[2],
				PivotX = position[0],
				PivotY = position[1],
				PivotZ = position[2]
			};
		}

		private Cuboid ToCuboid(int texWidth, int texHeight, float[] offset, int[] size, int[] texCoords, float dialation)
		{
			return new Cuboid(texCoords[0], texCoords[1], offset[0], offset[1], offset[2], size[0], size[1], size[2], 0, 0, 0, IsMirrored == "True", texWidth, texHeight, dialation);
		}
	}

	[XmlRoot(ElementName = "Geometry")]
	public class Geometry
	{
		[XmlElement(ElementName = "Shape")] public List<Shape> Shape { get; set; }
	}

	[XmlRoot(ElementName = "Model")]
	public class Model
	{
		[XmlElement(ElementName = "BaseClass")]
		public string BaseClass { get; set; }

		[XmlElement(ElementName = "Geometry")] public Geometry Geometry { get; set; }
		[XmlElement(ElementName = "GlScale")] public string GlScale { get; set; }
		[XmlElement(ElementName = "Name")] public string Name { get; set; }

		[XmlElement(ElementName = "TextureSize")]
		public string TextureSize { get; set; }

		[XmlAttribute(AttributeName = "texture")]
		public string Texture { get; set; }
	}

	[XmlRoot(ElementName = "Models")]
	public class ModelContainer
	{
		[XmlElement(ElementName = "Model")] public Model Model { get; set; }
	}

	[XmlElement(ElementName = "Author")] public string Author { get; set; }

	[XmlElement(ElementName = "DateCreated")]
	public string DateCreated { get; set; }

	[XmlElement(ElementName = "Description")]
	public string Description { get; set; }

	[XmlElement(ElementName = "Models")] public ModelContainer Models { get; set; }
	[XmlElement(ElementName = "Name")] public string Name { get; set; }

	[XmlElement(ElementName = "PreviewImage")]
	public string PreviewImage { get; set; }

	[XmlElement(ElementName = "ProjectName")]
	public string ProjectName { get; set; }

	[XmlElement(ElementName = "ProjectType")]
	public string ProjectType { get; set; }

	[XmlAttribute(AttributeName = "Version")]
	public string Version { get; set; }

	public static TechneModel Load(string filename)
	{
		using var fs = ZipFile.Open(filename, ZipArchiveMode.Read);

		var modelEntry = fs.Entries.FirstOrDefault(entry => entry.FullName == "model.xml");

		if (modelEntry == null)
			throw new InvalidDataException();

		using var modelStream = new StreamReader(modelEntry.Open());
		var modelXml = modelStream.ReadToEnd();

		var x = new XmlSerializer(typeof(TechneModel));
		var techne = (TechneModel)x.Deserialize(new StringReader(modelXml));

		return techne;
	}

	public List<ModelPart> GetModelParts(float dialation)
	{
		var texSize = Models.Model.TextureSize.Split(',').Select(int.Parse).ToArray();
		return Models.Model.Geometry.Shape.Select(shape => shape.ToModelPart(texSize[0], texSize[1], dialation)).ToList();
	}
}