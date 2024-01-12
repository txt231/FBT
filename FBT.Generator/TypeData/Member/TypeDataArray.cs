using FBT.TypeData.Base;

namespace FBT.TypeData.Member;

public class TypeDataArray
	: TypeDataBase
{
	public RefTypeData ArrayType { get; set; } = new();
}