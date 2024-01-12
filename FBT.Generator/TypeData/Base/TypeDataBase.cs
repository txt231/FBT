using System.Collections.Generic;
using System.Linq;
using FBT.TypeData.Base.Attributes;
using FBT.TypeData.Member;

namespace FBT.TypeData.Base;

public class TypeDataBase
{
	public List<TypeAttribute> Attributes = new();


	public List<TypeDataBase> Children = new();
	public string Name;


	public TypeAttribute FindAttribute(string p_Name)
	{
		return Attributes.FirstOrDefault(x => x.Key == p_Name);
	}

	public TypeAttribute FindAttributeIgnoreCase(string p_Name)
	{
		return Attributes.FirstOrDefault(x => x.Key.ToLower() == p_Name.ToLower());
	}


	public uint GetNameHash()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("NameHash") as TypeNumeralAttribute;

		if (s_NumeralAttribute is null)
			return 0;

		return (uint)s_NumeralAttribute.Value;
	}

	public byte[]? GetTypeGuid()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("Guid") as TypeStringAttribute;

		if (s_NumeralAttribute is null)
			return null;

		return s_NumeralAttribute.TryToByteArray();
	}

	public uint? GetTypeSignature()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("Signature") as TypeNumeralAttribute;

		if (s_NumeralAttribute is null)
			return 0;

		return (uint)s_NumeralAttribute.Value;
	}

	public long? GetSize()
	{
		return (FindAttributeIgnoreCase("size") as TypeNumeralAttribute).Value;
	}


	public virtual bool HasMember(string p_MemberName)
	{
		return Children.Where(x => x is TypeDataMember).Any(x => x.HasMember(p_MemberName));
	}
}