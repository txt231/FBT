using System;
using System.Collections.Generic;
using FBT.Module;
using FBT.TypeData;
using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;
using FBT.TypeData.DefaultValue;
using FBT.TypeData.Member;

namespace FBT.Parser;

internal class FBTVisitor
	: FBTBaseVisitor<int>
{
	private readonly Stack<TypeDefault> DefaultStack = new();

	private readonly Stack<TypeDataBase> TypeStack = new();
	private TypeUnit Unit;

	public TypeDataBase GetCurrentType()
	{
		return TypeStack.Peek();
	}

	public void SetUnit(TypeUnit p_Unit)
	{
		Unit = p_Unit;
	}

	public override int VisitFrostbiteType(FBTParser.FrostbiteTypeContext p_Context)
	{
		if (Unit == null)
			Unit = new TypeUnit("fb");

		return VisitChildren(p_Context);
	}

	/*
	public override int VisitIncludeBlock( FBTParser.IncludeBlockContext p_Context )
	{
	    return VisitChildren( p_Context );
	}
	*/


	public override int VisitInclude(FBTParser.IncludeContext p_Context)
	{
		var s_Text = p_Context.QUOTED_IMPORT().GetText();

		if (Unit != null)
			Unit.AddInclude(s_Text.Substring(1, s_Text.Length - 2));

		return VisitChildren(p_Context);
	}


	public override int VisitTypeData(FBTParser.TypeDataContext p_Context)
	{
		return VisitChildren(p_Context);
	}


	public override int VisitTypeNamespace(FBTParser.TypeNamespaceContext p_Context)
	{
		var s_Text = p_Context.UNQUOTED_STRING().GetText();

		if (Unit != null)
			Unit.Namespace = s_Text;

		return VisitChildren(p_Context);
	}


	public override int VisitTypeModule(FBTParser.TypeModuleContext p_Context)
	{
		var s_Text = p_Context.UNQUOTED_STRING().GetText();

		if (Unit != null)
			Unit.Module = ModuleManager.Instance.GetModule(s_Text);

		return VisitChildren(p_Context);
	}


	public override int VisitTypeAttributes(FBTParser.TypeAttributesContext p_Context)
	{
		return VisitChildren(p_Context);
	}


	public override int VisitTypeAttribute(FBTParser.TypeAttributeContext p_Context)
	{
		return VisitChildren(p_Context);
	}


	public override int VisitBasicTypeAttribute(FBTParser.BasicTypeAttributeContext p_Context)
	{
		var s_Type = GetCurrentType();

		if (s_Type != null)
		{
			var s_AttributeName = p_Context.UNQUOTED_STRING().GetText();

			s_Type.Attributes.Add(new TypeAttribute(s_AttributeName));
		}

		return VisitChildren(p_Context);
	}


	public override int VisitNumeralTypeAttribute(FBTParser.NumeralTypeAttributeContext p_Context)
	{
		var s_Type = GetCurrentType();

		if (s_Type != null)
		{
			var s_AttributeName = p_Context.UNQUOTED_STRING().GetText();


			var s_NumberText = p_Context.numeral().GetText();

			if (TryParseInteger(s_NumberText, out var s_Value))
				s_Type.Attributes.Add(new TypeNumeralAttribute(s_AttributeName, s_Value));
		}

		return VisitChildren(p_Context);
	}


	public override int VisitTypeEnum(FBTParser.TypeEnumContext p_Context)
	{
		TypeStack.Push(new TypeDataEnum());

		var s_Result = VisitChildren(p_Context);

		var s_PopData = TypeStack.Pop();

		if (TypeStack.Count == 0)
			Unit?.Children.Add(s_PopData);
		else
			GetCurrentType().Children.Add(s_PopData);

		return s_Result;
	}

	public override int VisitEnumName(FBTParser.EnumNameContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataEnum;

		if (s_Type != null)
			s_Type.Name = p_Context.UNQUOTED_STRING().GetText();

		return VisitChildren(p_Context);
	}
	/*
	public override int VisitEnumData( FBTParser.EnumDataContext p_Context )
	{
	    return VisitChildren( p_Context );
	}
	*/

	public override int VisitEnumValuePair(FBTParser.EnumValuePairContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataEnum;

		if (s_Type != null)
		{
			TryParseInteger(p_Context.numeral().GetText(), out var s_Value);


			s_Type.EnumDict.Add(p_Context.UNQUOTED_STRING().GetText(), (int)s_Value);
		}

		return VisitChildren(p_Context);
	}


	public override int VisitTypeClass(FBTParser.TypeClassContext p_Context)
	{
		TypeStack.Push(new TypeDataClass());

		var s_Result = VisitChildren(p_Context);


		var s_PopData = TypeStack.Pop();

		if (TypeStack.Count == 0)
			Unit?.Children.Add(s_PopData);
		else
			GetCurrentType().Children.Add(s_PopData);

		return s_Result;
	}

	public override int VisitTypeArray(FBTParser.TypeArrayContext p_Context)
	{
		var s_Type = new TypeDataArray();
		s_Type.Name = p_Context.arrayTypeName().GetText();

		TypeStack.Push(s_Type);

		var s_Result = VisitChildren(p_Context);

		var s_PopData = TypeStack.Pop();

		if (TypeStack.Count == 0)
			Unit?.Children.Add(s_PopData);
		else
			GetCurrentType().Children.Add(s_PopData);

		return s_Result;
	}

	public override int VisitArrayRef(FBTParser.ArrayRefContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataArray;

		if (s_Type != null)
			s_Type.ArrayType.TypeName = p_Context.arrayRefName().GetText();

		return base.VisitArrayRef(p_Context);
	}

	public override int VisitClassTypeName(FBTParser.ClassTypeNameContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataClass;

		if (s_Type != null)
			s_Type.Name = p_Context.UNQUOTED_STRING().GetText();

		return VisitChildren(p_Context);
	}

	public override int VisitClassInherrits(FBTParser.ClassInherritsContext p_Context)
	{
		return VisitChildren(p_Context);
	}

	public override int VisitClassInherrit(FBTParser.ClassInherritContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataClass;

		if (s_Type != null)
			s_Type.InherritedTypes.Add(new RefTypeData(p_Context.UNQUOTED_STRING().GetText()));

		return VisitChildren(p_Context);
	}


	public override int VisitTypeValueType(FBTParser.TypeValueTypeContext p_Context)
	{
		TypeStack.Push(new TypeDataValueType());

		var s_Result = VisitChildren(p_Context);

		var s_PopData = TypeStack.Pop();

		if (TypeStack.Count == 0)
			Unit?.Children.Add(s_PopData);
		else
			GetCurrentType().Children.Add(s_PopData);


		return s_Result;
	}

	public override int VisitValueTypeName(FBTParser.ValueTypeNameContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataValueType;

		if (s_Type != null)
			s_Type.Name = p_Context.UNQUOTED_STRING().GetText();


		return VisitChildren(p_Context);
	}


	public override int VisitStructureData(FBTParser.StructureDataContext p_Context)
	{
		return VisitChildren(p_Context);
	}


	public override int VisitMemberData(FBTParser.MemberDataContext p_Context)
	{
		TypeStack.Push(new TypeDataMember());

		var s_Result = VisitChildren(p_Context);


		var s_PopData = TypeStack.Pop();

		if (TypeStack.Count == 0)
			Unit?.Children.Add(s_PopData);
		else
			GetCurrentType().Children.Add(s_PopData);

		return s_Result;
	}

	public override int VisitMemberVisibility(FBTParser.MemberVisibilityContext p_Context)
	{
		/*
		var s_Type = GetCurrentType( ) as TypeDataMember;

		if ( s_Type != null )
		{
		    //s_Type.visi
		}
		*/

		return VisitChildren(p_Context);
	}

	public override int VisitMemberTypeName(FBTParser.MemberTypeNameContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataMember;

		if (s_Type != null)
			s_Type.BaseType = new RefTypeData(p_Context.UNQUOTED_STRING().GetText());

		return VisitChildren(p_Context);
	}

	public override int VisitMemberArrayType(FBTParser.MemberArrayTypeContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataMember;

		if (s_Type != null)
			if (p_Context.numeral() != null)
			{
				TryParseInteger(p_Context.numeral().GetText(), out var s_Value);

				s_Type.ArrayCount = (int)s_Value;
			}

		return VisitChildren(p_Context);
	}

	public override int VisitMemberName(FBTParser.MemberNameContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataMember;

		if (s_Type == null)
			throw new Exception("Member is null!");

		s_Type.Name = p_Context.UNQUOTED_STRING().GetText();

		return VisitChildren(p_Context);
	}

	public override int VisitMemberOffset(FBTParser.MemberOffsetContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataMember;

		if (s_Type == null)
			throw new Exception("Member is null!");

		if (p_Context.numeral() == null)
			throw new Exception("Member offset valie is not numeral!");

		if (!TryParseInteger(p_Context.numeral().GetText(), out var s_Value))
			throw new Exception("Failed to parse member offset value");

		s_Type.Offset = s_Value;

		return VisitChildren(p_Context);
	}


	public override int VisitBaseClassVariable(FBTParser.BaseClassVariableContext p_Context)
	{
		var s_BaseClass = p_Context.baseClassName().GetText();
		var s_BaseField = p_Context.baseFieldName().GetText();

		var s_Value = DefaultStack.Pop();


		var s_Type = GetCurrentType() as TypeDataClass;

		if (s_Type == null)
			throw new Exception("Class is null!");

		s_Type.BaseValues.Add(new TypeDataClass.BaseMemberValue
		{
			BaseClass = new RefTypeData(s_BaseClass),
			BaseField = s_BaseField,
			Value = s_Value
		});


		return VisitChildren(p_Context);
	}

	public override int VisitMemberDefaultValue(FBTParser.MemberDefaultValueContext p_Context)
	{
		var s_Result = VisitChildren(p_Context);


		var s_Type = GetCurrentType() as TypeDataMember;

		if (s_Type == null)
			throw new Exception("Member is null!");

		// This should never be null
		s_Type.DefaultValue = DefaultStack.Pop();

		return s_Result;
	}

	public override int VisitMemberValueInstance(FBTParser.MemberValueInstanceContext p_Context)
	{
		var s_IsRoot = DefaultStack.Count == 0;

		var s_CurrentDefault = new TypeDefaultInstance();
		DefaultStack.Push(s_CurrentDefault);


		return VisitChildren(p_Context);
	}

	public override int VisitMemberValueArray(FBTParser.MemberValueArrayContext p_Context)
	{
		var s_IsRoot = DefaultStack.Count == 0;

		var s_CurrentDefault = new TypeDefaultArray();
		DefaultStack.Push(s_CurrentDefault);

		return VisitChildren(p_Context);
	}

	public override int VisitMemberValueNumber(FBTParser.MemberValueNumberContext p_Context)
	{
		var s_CurrentDefault = new TypeDefaultNumber();
		if (!TryParseInteger(p_Context.numeral().GetText(), out var s_Value))
			throw new Exception("Cannot parse integer!?");

		s_CurrentDefault.Value = s_Value;

		DefaultStack.Push(s_CurrentDefault);

		return VisitChildren(p_Context);
	}

	public override int VisitMemberValueFloat(FBTParser.MemberValueFloatContext p_Context)
	{
		var s_CurrentDefault = new TypeDefaultFloat();
		if (!double.TryParse(p_Context.@float().GetText(), out var s_Value))
			throw new Exception("Cannot parse integer!?");

		s_CurrentDefault.Value = s_Value;

		DefaultStack.Push(s_CurrentDefault);

		return VisitChildren(p_Context);
	}

	public override int VisitMemberValueString(FBTParser.MemberValueStringContext p_Context)
	{
		var s_QuotedString = p_Context.QUOTED_STRING().GetText();

		var s_CurrentDefault = new TypeDefaultString();

		//TODO: unescape?
		s_CurrentDefault.Value = s_QuotedString.Substring(1, s_QuotedString.Length - 2);

		DefaultStack.Push(s_CurrentDefault);
		return VisitChildren(p_Context);
	}


	public override int VisitMemberValueNull(FBTParser.MemberValueNullContext p_Context)
	{
		var s_CurrentDefault = new TypeDefaultNull();

		DefaultStack.Push(s_CurrentDefault);
		return VisitChildren(p_Context);
	}

	public override int VisitMemberInstanceField(FBTParser.MemberInstanceFieldContext p_Context)
	{
		var s_Result = VisitChildren(p_Context);


		var s_Name = p_Context.UNQUOTED_STRING().GetText();
		var s_Value = DefaultStack.Pop();

		var s_SourceValue = DefaultStack.Peek() as TypeDefaultInstance;
		if (s_SourceValue == null)
			throw new Exception("Malformed data. this should not really happend!");

		s_SourceValue.Fields.Add(s_Name, s_Value);

		return s_Result;
	}

	public override int VisitMemberArrayElement(FBTParser.MemberArrayElementContext p_Context)
	{
		var s_Result = VisitChildren(p_Context);


		var s_Value = DefaultStack.Pop();

		var s_SourceValue = DefaultStack.Peek() as TypeDefaultArray;
		if (s_SourceValue == null)
			throw new Exception("Malformed data. this should not really happend!");

		s_SourceValue.Elements.Add(s_Value);

		return s_Result;
	}

	public override int VisitTypePrimitive(FBTParser.TypePrimitiveContext p_Context)
	{
		TypeStack.Push(new TypeDataPrimitive());

		var s_Result = VisitChildren(p_Context);


		var s_PopData = TypeStack.Pop();

		if (TypeStack.Count == 0)
			Unit?.Children.Add(s_PopData);
		else
			GetCurrentType().Children.Add(s_PopData);

		return s_Result;
	}

	public override int VisitPrimitiveTypeName(FBTParser.PrimitiveTypeNameContext p_Context)
	{
		var s_Type = GetCurrentType() as TypeDataPrimitive;

		if (s_Type != null)
			s_Type.Name = p_Context.UNQUOTED_STRING().GetText();

		return VisitChildren(p_Context);
	}

	/*
	public override int VisitNumeral( FBTParser.NumeralContext p_Context )
	{
	    return VisitChildren( p_Context );
	}
	*/


	private bool TryParseInteger(string p_String, out long p_Value)
	{
		p_Value = 0;

		if (long.TryParse(p_String, out p_Value))
			return true;

		if (p_String.ToLower().StartsWith("0x"))
		{
			p_Value = Convert.ToInt64(p_String, 16);
			return true;
		}

		return false;
	}
}