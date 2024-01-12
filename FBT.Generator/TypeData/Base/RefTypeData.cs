namespace FBT.TypeData.Base;

public class RefTypeData
{
	public string? TypeName;

	public RefTypeData()
	{
	}

	public RefTypeData(string p_Ref)
	{
		TypeName = p_Ref;
	}

	public RefTypeData(TypeDataBase p_Type)
	{
		SetTypeData(p_Type);
	}

	public TypeDataBase? Data { get; set; }


	public bool Unresolved => Data == null && TypeName != null;


	public void SetTypeData(TypeDataBase p_Type)
	{
		Data = p_Type;
		TypeName = p_Type.Name;
	}

	public static implicit operator TypeDataBase?(RefTypeData p_Ref)
	{
		return p_Ref.Data;
	}
}