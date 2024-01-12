namespace FBT.TypeData.DefaultValue;

public class TypeDefaultNull
	: TypeDefault
{
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
		return "null";
	}
}