using OpenTK.Mathematics;
using Substrate.Nbt;

namespace ModelPainterCore.Model.NEM;

public record NbtEntityModel(string Author, int Version, Vector3 Scale, NbtEntityModel.ModelTextureInfo Texture, List<ModelPart> Parts)
{
	public record ModelTextureInfo(string Path, int Width, int Height);

	public record PartTextureInfo(float Width, float Height, int U, int V, bool Mirrored);

	public record EulerAngles(float Pitch, float Yaw, float Roll);

	public static NbtEntityModel Load(string filename)
	{
		using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		var nbt = new NbtTree(fs);

		var authorTag = nbt.Root["author"].ToTagString();
		var versionTag = nbt.Root["version"].ToTagInt();
		var scaleTag = nbt.Root["scale"].ToTagCompound();
		var textureTag = nbt.Root["tex"].ToTagCompound();
		var partsTag = nbt.Root["parts"].ToTagCompound();

		var author = authorTag.Data;
		var version = versionTag.Data;
		var scale = GetVec3(scaleTag);
		var texture = GetTextureInfo(textureTag);
		var parts = GetParts(partsTag, texture.Width, texture.Height);

		return new NbtEntityModel(author, version, scale, texture, parts);
	}

	private static List<ModelPart> GetParts(TagNodeCompound partsTag, int textureWidth, int textureHeight)
	{
		var parts = new List<ModelPart>();

		foreach (var partKey in partsTag.Keys)
		{
			var partTag = partsTag[partKey].ToTagCompound();
			parts.Add(GetPart(partTag, textureWidth, textureHeight));
		}

		return parts;
	}

	private static ModelPart GetPart(TagNodeCompound partTag, int textureWidth, int textureHeight)
	{
		var rotTag = partTag["rot"].ToTagCompound();
		var posTag = partTag["pos"].ToTagCompound();
		var texTag = partTag["tex"].ToTagCompound();
		var childrenTag = partTag.ContainsKey("children") ? partTag["children"].ToTagCompound() : null;
		var cuboidsTag = partTag["cuboids"].ToTagList();

		var rot = GetEulerAngles(rotTag);
		var pos = GetVec3(posTag);
		var tex = GetPartTextureInfo(texTag);
		var children = childrenTag != null ? GetParts(childrenTag, textureWidth, textureHeight) : new List<ModelPart>();
		var cuboids = GetCuboids(cuboidsTag, tex.Mirrored, tex.U, tex.V, textureWidth, textureHeight);

		return new ModelPart
		{
			Pitch = MathHelper.RadiansToDegrees(rot.Pitch),
			Yaw = MathHelper.RadiansToDegrees(rot.Yaw),
			Roll = MathHelper.RadiansToDegrees(rot.Roll),
			TextureWidth = (int)tex.Width,
			TextureHeight = (int)tex.Height,
			PivotX = pos.X,
			PivotY = pos.Y,
			PivotZ = pos.Z,
			Parts = children,
			Cuboids = cuboids
		};
	}

	private static List<Cuboid> GetCuboids(TagNodeList tag, bool mirror, int u, int v, int textureWidth, int textureHeight)
	{
		return tag.Select(node => GetCuboid(node.ToTagCompound(), mirror, u, v, textureWidth, textureHeight)).ToList();
	}

	private static Cuboid GetCuboid(TagNodeCompound tag, bool mirror, int u, int v, int textureWidth, int textureHeight)
	{
		var sizeTag = tag["size"].ToTagCompound();
		var posTag = tag["pos"].ToTagCompound();
		var expandTag = tag["expand"].ToTagCompound();
		var texTag = tag["tex"].ToTagCompound();

		var size = GetVec3I(sizeTag);
		var pos = GetVec3(posTag);
		var expand = GetVec3(expandTag);
		var tex = GetTex(texTag);

		return new Cuboid(u + (int)tex.X, v + (int)tex.Y, pos.X, pos.Y, pos.Z, size.X, size.Y, size.Z, expand.X, expand.Y, expand.Z, mirror, textureWidth, textureHeight, 0);
	}

	private static PartTextureInfo GetPartTextureInfo(TagNodeCompound tag)
	{
		return new PartTextureInfo(
			(float)tag["w"].ToTagDouble().Data,
			(float)tag["h"].ToTagDouble().Data,
			tag["u"].ToTagInt().Data,
			tag["v"].ToTagInt().Data,
			tag["mirrored"].ToTagByte().Data == 1
		);
	}

	private static EulerAngles GetEulerAngles(TagNodeCompound tag)
	{
		return new EulerAngles(
			(float)tag["pitch"].ToTagDouble().Data,
			(float)tag["yaw"].ToTagDouble().Data,
			(float)tag["roll"].ToTagDouble().Data
		);
	}

	private static Vector3 GetVec3(TagNodeCompound tag)
	{
		return new Vector3(
			(float)tag["x"].ToTagDouble().Data,
			(float)tag["y"].ToTagDouble().Data,
			(float)tag["z"].ToTagDouble().Data
		);
	}

	private static Vector3 GetVec3I(TagNodeCompound tag)
	{
		return new Vector3(
			tag["x"].ToTagInt().Data,
			tag["y"].ToTagInt().Data,
			tag["z"].ToTagInt().Data
		);
	}

	private static Vector2 GetTex(TagNodeCompound tag)
	{
		return new Vector2(
			tag["u"].ToTagInt().Data,
			tag["v"].ToTagInt().Data
		);
	}

	private static ModelTextureInfo GetTextureInfo(TagNodeCompound tag)
	{
		return new ModelTextureInfo(
			tag["path"].ToTagString().Data,
			(int)tag["w"].ToTagDouble().Data,
			(int)tag["h"].ToTagDouble().Data
		);
	}
}