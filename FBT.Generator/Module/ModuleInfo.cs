using System.Collections.Generic;
using FBT.TypeData.Base;

namespace FBT.Module;

public class ModuleInfo
{
	public string Name;

	public List<TypeDataBase> Types;

	public ModuleInfo(string p_Name)
	{
		Name = p_Name;

		Types = new List<TypeDataBase>();
	}


	public void AddType(TypeDataBase p_Type)
	{
		Types.Add(p_Type);
	}
}