using FBT.Module;
using FBT.TypeData;
using FBT.TypeData.Base;
using FBT.TypeData.Member;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.Parser
{
    internal class FBTVisitor
        : FBTBaseVisitor<int>
    {
        TypeUnit Unit = null;

        Stack<TypeDataBase> TypeStack = new Stack<TypeDataBase>( );

        public TypeDataBase GetCurrentType( )
        {
            return TypeStack.Peek( );
        }

        public void SetUnit( TypeUnit p_Unit )
        {
            this.Unit = p_Unit;
        }

        public override int VisitFrostbiteType( FBTParser.FrostbiteTypeContext p_Context )
        {
            if (this.Unit == null)
                this.Unit = new TypeUnit( "fb" );

            return VisitChildren( p_Context );
        }

        /*
        public override int VisitIncludeBlock( FBTParser.IncludeBlockContext p_Context )
        {
            return VisitChildren( p_Context );
        }
        */


        public override int VisitInclude( FBTParser.IncludeContext p_Context )
        {
            var s_Text = p_Context.QUOTED_IMPORT( ).GetText( );

            if ( this.Unit != null )
                this.Unit.AddInclude( s_Text.Substring( 1, s_Text.Length - 2 ) );

            return VisitChildren( p_Context );
        }


        public override int VisitTypeData( FBTParser.TypeDataContext p_Context )
        {
            return VisitChildren( p_Context );
        }


        public override int VisitTypeNamespace( FBTParser.TypeNamespaceContext p_Context )
        {
            var s_Text = p_Context.UNQUOTED_STRING( ).GetText( );

            if ( this.Unit != null )
                this.Unit.Namespace = s_Text;

            return VisitChildren( p_Context );
        }


        public override int VisitTypeModule( FBTParser.TypeModuleContext p_Context )
        {
            var s_Text = p_Context.UNQUOTED_STRING( ).GetText( );

            if ( this.Unit != null )
                this.Unit.Module = ModuleManager.Instance.GetModule( s_Text );

            return VisitChildren( p_Context );
        }


        public override int VisitTypeAttributes( FBTParser.TypeAttributesContext p_Context )
        {
            return VisitChildren( p_Context );
        }


        public override int VisitTypeAttribute( FBTParser.TypeAttributeContext p_Context )
        {
            return VisitChildren( p_Context );
        }


        public override int VisitBasicTypeAttribute( FBTParser.BasicTypeAttributeContext p_Context )
        {
            var s_Type = GetCurrentType( );

            if ( s_Type != null )
            {
                var s_AttributeName = p_Context.UNQUOTED_STRING( ).GetText( );

                s_Type.Attributes.Add( new TypeData.Base.Attributes.TypeAttribute( s_AttributeName ) );
            }

            return VisitChildren( p_Context );
        }


        public override int VisitNumeralTypeAttribute( FBTParser.NumeralTypeAttributeContext p_Context )
        {
            var s_Type = GetCurrentType( );

            if ( s_Type != null )
            {
                var s_AttributeName = p_Context.UNQUOTED_STRING( ).GetText( );


                var s_NumberText = p_Context.numeral( ).GetText( );

                int s_Value = -1;
                if ( int.TryParse( s_NumberText, out s_Value ) )
                    s_Type.Attributes.Add( new TypeData.Base.Attributes.TypeNumeralAttribute( s_AttributeName, s_Value ) );
            }

            return VisitChildren( p_Context );
        }



        public override int VisitTypeEnum( FBTParser.TypeEnumContext p_Context )
        {
            TypeStack.Push( new TypeDataEnum( ) );

            var s_Result = VisitChildren( p_Context );

            var s_PopData = TypeStack.Pop( );

            if ( TypeStack.Count == 0 )
                this.Unit?.Children.Add( s_PopData );
            else
                this.GetCurrentType( ).Children.Add( s_PopData );

            return s_Result;
        }

        public override int VisitEnumName( FBTParser.EnumNameContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataEnum;

            if ( s_Type != null )
                s_Type.Name = p_Context.UNQUOTED_STRING( ).GetText( );

            return VisitChildren( p_Context );
        }
        /*
        public override int VisitEnumData( FBTParser.EnumDataContext p_Context )
        {
            return VisitChildren( p_Context );
        }
        */

        public override int VisitEnumValuePair( FBTParser.EnumValuePairContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataEnum;

            if ( s_Type != null )
            {
                int s_Value = -1;

                int.TryParse( p_Context.numeral( ).GetText( ), out s_Value );


                s_Type.EnumDict.Add( p_Context.UNQUOTED_STRING( ).GetText( ), s_Value );
            }

            return VisitChildren( p_Context );
        }



        public override int VisitTypeClass( FBTParser.TypeClassContext p_Context )
        {
            TypeStack.Push( new TypeDataClass( ) );

            var s_Result = VisitChildren( p_Context );


            var s_PopData = TypeStack.Pop( );

            if ( TypeStack.Count == 0 )
                this.Unit?.Children.Add( s_PopData );
            else
                this.GetCurrentType( ).Children.Add( s_PopData );

            return s_Result;
        }

        public override int VisitClassTypeName( FBTParser.ClassTypeNameContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataClass;

            if ( s_Type != null )
                s_Type.Name = p_Context.UNQUOTED_STRING( ).GetText( );

            return VisitChildren( p_Context );
        }

        public override int VisitClassInherrits( FBTParser.ClassInherritsContext p_Context )
        {
            return VisitChildren( p_Context );
        }

        public override int VisitClassInherrit( FBTParser.ClassInherritContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataClass;

            if ( s_Type != null )
                s_Type.UnresolvedInherritedTypes.Add( p_Context.UNQUOTED_STRING( ).GetText( ) );

            return VisitChildren( p_Context );
        }




        public override int VisitTypeValueType( FBTParser.TypeValueTypeContext p_Context )
        {
            TypeStack.Push( new TypeDataValueType( ) );

            var s_Result = VisitChildren( p_Context );

            var s_PopData = TypeStack.Pop( );

            if ( TypeStack.Count == 0 )
                this.Unit?.Children.Add( s_PopData );
            else
                this.GetCurrentType( ).Children.Add( s_PopData );


            return s_Result;
        }

        public override int VisitValueTypeName( FBTParser.ValueTypeNameContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataValueType;

            if ( s_Type != null )
                s_Type.Name = p_Context.UNQUOTED_STRING( ).GetText( );


            return VisitChildren( p_Context );
        }



        public override int VisitStructureData( FBTParser.StructureDataContext p_Context )
        {
            return VisitChildren( p_Context );
        }


        public override int VisitMemberData( FBTParser.MemberDataContext p_Context )
        {
            TypeStack.Push( new TypeDataMember( ) );

            var s_Result = VisitChildren( p_Context );


            var s_PopData = TypeStack.Pop( );

            if ( TypeStack.Count == 0 )
                this.Unit?.Children.Add( s_PopData );
            else
                this.GetCurrentType( ).Children.Add( s_PopData );

            return s_Result;
        }

        public override int VisitMemberVisibility( FBTParser.MemberVisibilityContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataMember;

            if ( s_Type != null )
            {
                //s_Type.visi
            }

            return VisitChildren( p_Context );
        }

        public override int VisitMemberTypeName( FBTParser.MemberTypeNameContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataMember;

            if ( s_Type != null )
                s_Type.UnresolvedBaseType = p_Context.UNQUOTED_STRING( ).GetText( );

            return VisitChildren( p_Context );
        }

        public override int VisitMemberArrayType( FBTParser.MemberArrayTypeContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataMember;

            if ( s_Type != null )
            {
                s_Type.IsArray = true;

                if ( p_Context.numeral( ) != null )
                {
                    int s_Value = -1;

                    int.TryParse( p_Context.numeral( ).GetText( ), out s_Value );

                    s_Type.ArrayCount = s_Value;
                }
            }

            return VisitChildren( p_Context );
        }

        public override int VisitMemberName( FBTParser.MemberNameContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataMember;

            if ( s_Type != null )
            {
                s_Type.Name = p_Context.UNQUOTED_STRING( ).GetText( );
            }

            return VisitChildren( p_Context );
        }

        public override int VisitMemberOffset( FBTParser.MemberOffsetContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataMember;

            if ( s_Type != null )
            {
                if ( p_Context.numeral( ) != null )
                {
                    int s_Value = -1;

                    int.TryParse( p_Context.numeral( ).GetText( ), out s_Value );

                    s_Type.Offset = s_Value;
                }
            }

            return VisitChildren( p_Context );
        }



        public override int VisitTypePrimitive( FBTParser.TypePrimitiveContext p_Context )
        {
            TypeStack.Push( new TypeDataPrimitive( ) );

            var s_Result = VisitChildren( p_Context );


            var s_PopData = TypeStack.Pop( );

            if ( TypeStack.Count == 0 )
                this.Unit?.Children.Add( s_PopData );
            else
                this.GetCurrentType( ).Children.Add( s_PopData );

            return s_Result;
        }

        public override int VisitPrimitiveTypeName( FBTParser.PrimitiveTypeNameContext p_Context )
        {
            var s_Type = GetCurrentType( ) as TypeDataPrimitive;

            if ( s_Type != null )
                s_Type.Name = p_Context.UNQUOTED_STRING( ).GetText( );

            return VisitChildren( p_Context );
        }

        /*
        public override int VisitNumeral( FBTParser.NumeralContext p_Context )
        {
            return VisitChildren( p_Context );
        }
        */
    }
}
