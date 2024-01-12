namespace FBT.TypeData.DefaultValue;

public class TypeDefaultString
	: TypeDefault
{
	public string Value { get; set; }

	public override long? AsLong()
	{
		return long.TryParse(Value, out var s_Value) ? s_Value : null;
	}

	public override double? AsDouble()
	{
		return double.TryParse(Value, out var s_Value) ? s_Value : null;
	}

	public override string? AsString()
	{
		return Value;
	}
}