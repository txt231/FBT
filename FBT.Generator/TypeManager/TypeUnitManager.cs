using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FBT.Parser;
using FBT.TypeData;
using FBT.TypeData.Base;
using FBT.TypeData.Member;

namespace FBT.TypeManager;

public class TypeUnitManager
{
	private static TypeUnitManager _Instance;
	public Dictionary<string, TypeUnit> Units = new();
	public static TypeUnitManager Instance => _Instance ?? (_Instance = new TypeUnitManager());


	public TypeUnit? GetUnit(string p_Name)
	{
		if (Units.TryGetValue(p_Name, out var s_Unit))
			return s_Unit;

		return null;
	}

	public void AddUnit(string p_Name, TypeUnit p_Unit)
	{
		Units.Add(p_Name, p_Unit);
	}


	private void ResolveType(TypeDataBase p_Type)
	{
		var s_ClassType = p_Type as TypeDataClass;
		if (s_ClassType != null)
		{
			foreach (var s_UnresolvedInherrit in s_ClassType.InherritedTypes)
			{
				if (!s_UnresolvedInherrit.Unresolved)
					continue;

				var s_InherritUnit = GetUnit(s_UnresolvedInherrit.TypeName);

				if (s_InherritUnit == null)
					continue;

				s_UnresolvedInherrit.SetTypeData(
					s_InherritUnit.Children.FirstOrDefault(x => x.Name == s_UnresolvedInherrit.TypeName));
			}

			foreach (var s_BaseType in s_ClassType.BaseValues)
			{
				if (!s_BaseType.BaseClass.Unresolved)
					continue;

				var s_InherritUnit = GetUnit(s_BaseType.BaseClass.TypeName);

				if (s_InherritUnit == null)
					continue;

				s_BaseType.BaseClass.SetTypeData(
					s_InherritUnit.Children.FirstOrDefault(x => x.Name == s_BaseType.BaseClass.TypeName));
			}
		}

		var s_MemberType = p_Type as TypeDataMember;
		if (s_MemberType != null)
			if (s_MemberType.BaseType.Unresolved)
			{
				var s_InherritUnit = GetUnit(s_MemberType.BaseType.TypeName);

				if (s_InherritUnit != null)
					s_MemberType.BaseType.SetTypeData(
						s_InherritUnit.Children.FirstOrDefault(x => x.Name == s_MemberType.BaseType.TypeName));
			}

		var s_ArrayType = p_Type as TypeDataArray;
		if (s_ArrayType != null)
			if (s_ArrayType.ArrayType.Unresolved)
			{
				var s_InherritUnit = GetUnit(s_ArrayType.ArrayType.TypeName);

				if (s_InherritUnit != null)
					s_ArrayType.ArrayType.SetTypeData(
						s_InherritUnit.Children.FirstOrDefault(x => x.Name == s_ArrayType.ArrayType.TypeName));
			}

		p_Type.Children.ForEach(x => ResolveType(x));
	}


	public void ResolveImports()
	{
		var s_UnresolvedUnits = Units.Where(x => !x.Value.IsResolved);

		while (true)
		{
			var s_Unit = Units.FirstOrDefault(x => !x.Value.IsResolved);

			if (s_Unit.Value == null)
				break;

			foreach (var s_Include in s_Unit.Value.Includes)
			{
				var s_IncludeUnit = GetUnit(Path.GetFileNameWithoutExtension(s_Include));

				if (s_IncludeUnit != null)
					continue;

				Console.WriteLine($"Resolving import from {s_Unit.Key} -> {s_Include}");

				var s_File = new FBTFile(s_Unit.Value.Path + "/" + s_Include);
			}

			s_Unit.Value.IsResolved = true;
		}

		foreach (var s_Unit in Units) s_Unit.Value.Children.ForEach(x => ResolveType(x));
	}
}