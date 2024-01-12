using System.Collections.Generic;
using System.Linq;
using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;
using FBT.TypeData.DefaultValue;

namespace FBT.TypeData.Member;

public class TypeDataClass
	: TypeDataBase
{
	public List<BaseMemberValue> BaseValues = new();

	public List<RefTypeData> InherritedTypes = new();

	public string Protection = null;

	public long GetClassId()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("classId") as TypeNumeralAttribute;

		if (s_NumeralAttribute == null)
			return -1;

		return s_NumeralAttribute.Value;
	}

	public long GetLastSubClassId()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("lastSubClassId") as TypeNumeralAttribute;

		if (s_NumeralAttribute == null)
			return -1;

		return s_NumeralAttribute.Value;
	}

	public override bool HasMember(string p_MemberName)
	{
		return InherritedTypes.Any(x => x.Data?.HasMember(p_MemberName) ?? false) || base.HasMember(p_MemberName);
	}

	public class BaseMemberValue
	{
		public RefTypeData BaseClass { get; set; }
		public string BaseField { get; set; }
		public TypeDefault Value { get; set; }
	}
}