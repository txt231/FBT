using FBT.TypeData;

namespace FBT.Generator;

public abstract class FBTCodeGenerator
{
	public string OutPath;

	public FBTCodeGenerator(string p_Name, string p_Path = "./")
	{
		OutPath = p_Path;
	}

	public virtual void PreBuild()
	{
	}

	public virtual void PostBuild()
	{
	}

	public abstract void GenerateUnit(TypeUnit p_Unit, string p_Name);
}