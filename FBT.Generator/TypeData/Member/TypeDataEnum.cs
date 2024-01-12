using System.Collections.Generic;
using FBT.TypeData.Base;

namespace FBT.TypeData.Member;

public class TypeDataEnum
	: TypeDataBase
{
	public Dictionary<string, int> EnumDict = new();
}