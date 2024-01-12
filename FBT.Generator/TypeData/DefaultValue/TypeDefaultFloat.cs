namespace FBT.TypeData.DefaultValue;

public class TypeDefaultFloat
	: TypeDefault
{
	public double Value { get; set; }

	public override long? AsLong()
	{
		return (long)Value;
	}

	public override double? AsDouble()
	{
		return Value;
	}

	public override string? AsString()
	{
		return $"{Value:F}";
	}
}