namespace FBT.TypeData.DefaultValue;

public class TypeDefaultNumber
	: TypeDefault
{
	public long Value { get; set; }

	public override long? AsLong()
	{
		return Value;
	}

	public override double? AsDouble()
	{
		return (double)Value;
	}

	public override string? AsString()
	{
		return $"{Value}";
	}
}