using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;

namespace FBT.TypeData.Member;

public class TypeDataValueType
	: TypeDataBase
{
	public string Protection = null;

	public uint? GetMessageType()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("MessageType") as TypeNumeralAttribute;

		if (s_NumeralAttribute == null)
			return null;

		return (uint)s_NumeralAttribute.Value;
	}

	public uint? GetMessageCategory()
	{
		var s_NumeralAttribute = FindAttributeIgnoreCase("MessageCategory") as TypeNumeralAttribute;

		if (s_NumeralAttribute == null)
			return null;

		return (uint)s_NumeralAttribute.Value;
	}
}