using System.Collections.Generic;

namespace FBT.TypeData.DefaultValue;

public class TypeDefaultArray
	: TypeDefault
{
	public List<TypeDefault> Elements = new();


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