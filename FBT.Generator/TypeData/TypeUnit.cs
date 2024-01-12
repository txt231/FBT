using System.Collections.Generic;
using FBT.Module;
using FBT.TypeData.Base;

namespace FBT.TypeData;

public class TypeUnit
{
	public List<TypeDataBase> Children = new();

	public List<string> Includes = new();

	public bool IsResolved = false;

	public ModuleInfo Module;
	public string Namespace;
	public string Path = null;

	public TypeUnit(string p_Namespace)
	{
		Namespace = p_Namespace;
	}

	public void AddInclude(string p_Include)
	{
		Includes.Add(p_Include);
	}
}