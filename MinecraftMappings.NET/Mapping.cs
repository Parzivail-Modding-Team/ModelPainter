namespace MinecraftMappings.NET;

public record ClassMapping(string Official, string Intermediary, string Mapped);

public record ClassMemberMapping(string ParentOfficial, string Official, string OfficalSignature, string Intermediary, string Mapped);

public record MappingSet(ClassMapping[] Classes, ClassMemberMapping[] Methods, ClassMemberMapping[] Fields)
{
	public MappingDictionary CreateOfficialMapper()
	{
		return new MappingDictionary(
			Classes.ToDictionary(mapping => mapping.Official, mapping => mapping),
			Methods.ToDictionary(mapping => mapping.ParentOfficial + "/" + mapping.Official + "!" + mapping.OfficalSignature, mapping => mapping),
			Fields.ToDictionary(mapping => mapping.ParentOfficial + "/" + mapping.Official, mapping => mapping)
		);
	}

	public Dictionary<string, ClassMapping> CreateIntermediaryClassMapper()
	{
		return Classes.ToDictionary(mapping => mapping.Intermediary, mapping => mapping);
	}
}

public record MappingDictionary(Dictionary<string, ClassMapping> Classes, Dictionary<string, ClassMemberMapping> Methods, Dictionary<string, ClassMemberMapping> Fields);