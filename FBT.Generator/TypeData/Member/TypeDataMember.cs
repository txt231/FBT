using FBT.TypeData.Base;
using FBT.TypeData.DefaultValue;

namespace FBT.TypeData.Member;

public class TypeDataMember
	: TypeDataBase
{
	//public bool IsArray;
	public int ArrayCount = -1;

	public RefTypeData BaseType = new();

	public TypeDefault? DefaultValue = null;

	public long Offset;
	public string Protection = null;

	public override bool HasMember(string p_MemberName)
	{
		return Name == p_MemberName;
	}


	public bool IsArray()
	{
		return BaseType.Data is TypeDataArray;
	}

	public TypeDataBase GetFieldType()
	{
		if (IsArray())
			return (BaseType.Data as TypeDataArray)!.ArrayType;

		return BaseType;
	}
}