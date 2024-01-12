using System;
using System.Linq;

namespace FBT.TypeData.Base.Attributes;

internal class TypeStringAttribute
	: TypeAttribute
{
	public string Value;

	public TypeStringAttribute(string p_Key, string p_Value)
		: base(p_Key)
	{
		Value = p_Value;
	}


	public byte[]? TryToByteArray()
	{
		byte[]? s_Array = null;

		try
		{
			s_Array = Enumerable.Range(0, Value.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(Value.Substring(x, 2), 0x10))
				.ToArray();
		}
		catch (Exception _)
		{
			return null;
		}

		return s_Array;
	}
}