namespace FBT.TypeData.Base.Attributes;

public class TypeNumeralAttribute
	: TypeAttribute
{
	public long Value;

	public TypeNumeralAttribute(string p_Key, long p_Value)
		: base(p_Key)
	{
		Value = p_Value;
	}
}