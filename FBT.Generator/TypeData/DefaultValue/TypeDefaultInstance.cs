using System.Collections.Generic;
using FBT.TypeData.Base;

namespace FBT.TypeData.DefaultValue;

public class TypeDefaultInstance
	: TypeDefault
{
	public Dictionary<string, TypeDefault> Fields = new();
	private RefTypeData? BaseClass { get; set; } = null;

	public override long? AsLong()
	{
		return null;
	}

	public override double? AsDouble()
	{
		return null;
	}

	public override string? AsString()
	{
		return null;
	}
}