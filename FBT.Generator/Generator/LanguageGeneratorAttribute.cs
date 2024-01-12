using System;

namespace FBT.Generator;

[AttributeUsage(AttributeTargets.Class)]
public class LanguageGeneratorAttribute : Attribute
{
	/// <summary>
	///     Default constructor that takes an alignment
	/// </summary>
	/// <param name="Alignment">Default alignment: 0</param>
	public LanguageGeneratorAttribute(string p_CateogryName)
	{
		Name = p_CateogryName;
	}

	public string Name { get; set; }
}