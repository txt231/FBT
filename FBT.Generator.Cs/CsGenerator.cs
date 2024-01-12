using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using FBT.Generator;
using FBT.TypeData;
using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;
using FBT.TypeData.DefaultValue;
using FBT.TypeData.Member;

namespace FBT.Generator.Cs
{
    [LanguageGenerator( "Cs" )]
    public class CsGenerator
        : FBTCodeGenerator
    {
        public CsGenerator( string p_Name, string p_Path )
            : base( p_Name, p_Path )
        {
        }

        const string s_Extention = ".cs";


        public override void GenerateUnit( TypeUnit p_Unit, string p_Name )
        {
            // -------------------------------------------------------------------------------------
            //
            // Yo, we can use the new assembly generator thingy instead of writing source files.....
            //
            // -------------------------------------------------------------------------------------

            
            // skip primitives
            if (p_Unit.Children.Count == 1 &&
                p_Unit.Children[0] is TypeDataPrimitive)
                return;
            
            var s_FilePath = Path.Join(this.OutPath, p_Name + s_Extention);

            if (p_Unit.Module?.Name != null)
                s_FilePath = Path.Join(this.OutPath, p_Unit.Module.Name, p_Name + s_Extention);

            Directory.CreateDirectory(Path.GetDirectoryName(s_FilePath));
            
            using var s_FileStream = new FileStream(s_FilePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            using var s_StreamWriter = new StreamWriter(s_FileStream);
            using var s_Writer = new IndentedTextWriter( s_StreamWriter, "\t" );


            //Warning
            s_Writer.WriteLine( "///////////////////////////////////////////////////////////////" );
            s_Writer.WriteLine( "//                                                           //" );
            s_Writer.WriteLine( "// This is an automatically generated file.                  //" );
            s_Writer.WriteLine( "// Do *NOT* modify unless you really know what you're doing. //" );
            s_Writer.WriteLine( "//                                                           //" );
            s_Writer.WriteLine( "///////////////////////////////////////////////////////////////" );

            s_Writer.WriteLine( );

            //Include 
            s_Writer.WriteLine("using System;");
            s_Writer.WriteLine("using System.IO;");
            s_Writer.WriteLine("using System.Collections.Generic;");
            s_Writer.WriteLine("using Newtonsoft.Json;");
            s_Writer.WriteLine("using RimeLib.IO;");
            s_Writer.WriteLine("using RimeLib.Frostbite.Core;");
            s_Writer.WriteLine("using RimeLib.Serialization.Attributes;");
            s_Writer.WriteLine("using RimeLib.Serialization;");
            s_Writer.WriteLine("using RimeLib.Serialization.Frostbite2_0.Ebx;");
            
            s_Writer.WriteLine( );



            GenerateNamespaceBegin( s_Writer, p_Unit );

            s_Writer.WriteLine( );

            p_Unit.Children.ForEach( x => this.GenerateType( s_Writer, x ) );

            s_Writer.WriteLine( );

            GenerateNamespaceEnd( s_Writer, p_Unit );
        }

        #region Namespace
        void GenerateNamespaceBegin( IndentedTextWriter p_Writer, TypeUnit p_Unit )
        {
            if ( p_Unit.Namespace == null )
                return;
            
            var s_NamespaceName = p_Unit.Namespace.Replace( "::", "." );

            p_Writer.WriteLine( $"namespace {s_NamespaceName}" );
            p_Writer.WriteLine( "{" );

            p_Writer.Indent++;
        }

        void GenerateNamespaceEnd( IndentedTextWriter p_Writer, TypeUnit p_Unit )
        {
            if ( p_Unit.Namespace == null )
                return;

            p_Writer.Indent--;

            p_Writer.WriteLine( "}" );
        }

        #endregion

        void GenerateType( IndentedTextWriter p_Writer, TypeDataBase p_Type )
        {
            if ( p_Type is TypeDataClass )
                this.GenerateClass( p_Writer, p_Type as TypeDataClass );
            else if ( p_Type is TypeDataValueType )
                this.GenerateValueType( p_Writer, p_Type as TypeDataValueType );
            else if ( p_Type is TypeDataMember )
                this.GenerateMember( p_Writer, p_Type as TypeDataMember );
        }


        void GenerateClass( IndentedTextWriter p_Writer, TypeDataClass p_Type )
        {
            var s_DataContainerBase = p_Type.InherritedTypes.FirstOrDefault( x => x.TypeName == "DataContainer" );


            //TODO = Rimelib FrostbiteContainer

            var s_AlignAttribute = p_Type.FindAttributeIgnoreCase( "align" ) as TypeNumeralAttribute;
            var s_SizeAttribute = p_Type.FindAttributeIgnoreCase("size") as TypeNumeralAttribute;
            
            if (s_AlignAttribute == null ||
                s_SizeAttribute == null)
                throw new System.Exception("Why is this happening");
            
            p_Writer.WriteLine( $"[ContainerType({s_AlignAttribute.Value}, {s_SizeAttribute.Value})]" );

            if(p_Type.Name == "DataContainer")
                p_Writer.WriteLine( $"public abstract class {p_Type.Name}" );
            else
                p_Writer.WriteLine( $"public class {p_Type.Name}" );

            p_Writer.Indent++;

            if (p_Type.Name == "DataContainer")
            {
                if (p_Type.InherritedTypes.Count > 0)
                    throw new Exception("DataContainer class definition has inherited types. This is unsupported.");

                p_Writer.WriteLine(": EbxSerializable");
            }
            else
            {
                
                bool FirstInherrit = true;

                foreach ( var s_Inherrit in p_Type.InherritedTypes )
                {
                    p_Writer.WriteLine( $"{( FirstInherrit ? ":" : "," )} {s_Inherrit.TypeName}" );

                    FirstInherrit = false;
                }

            }
            p_Writer.Indent--;

            p_Writer.WriteLine( "{" );
            p_Writer.Indent++;
            {
                if (p_Type.BaseValues.Count > 0)
                {
                    p_Writer.WriteLine($"public {p_Type.Name}()");
                    p_Writer.WriteLine($"{{");
                    p_Writer.Indent++;
                    
                    p_Type.BaseValues.ForEach(x =>
                    {
                        p_Writer.WriteLine($"//{x.BaseClass.TypeName}");
                        p_Writer.WriteLine($"{x.BaseField} = ;");
                    });


                    p_Writer.Indent--;
                    p_Writer.WriteLine($"}}");
                }
                
                if (p_Type.Name == "DataContainer")
                {
                    p_Writer.WriteLine($"[JsonProperty(\"$type\", Order = -2)]");
                    p_Writer.WriteLine($"public string TypeName => GetType().Name;");
                }
                
                p_Type.Children
                    .OrderBy( x => (x as TypeDataMember)?.Offset)
                    .ToList()
                    .ForEach( x =>
                {
                    if (x is not TypeDataMember)
                    {
                        
                        this.GenerateType(p_Writer, x);
                        return;
                    }

                    var s_Member = (x as TypeDataMember)!;

                    var s_FieldMemberType = s_Member.GetFieldType(); 
                    var s_Pointer = s_FieldMemberType is TypeDataClass;
                    var s_Array = s_Member.IsArray();
                    
                    var s_MemberName = s_Member.Name;
                    while (p_Type.InherritedTypes.Any(x => x.Data?.HasMember((s_MemberName)) ?? false))
                        s_MemberName += "_";

                    var s_CustomNameProperty = "";
                    if (s_MemberName != s_Member.Name)
                        s_CustomNameProperty = $", Name = \"{s_Member.Name}\"";
            
                    p_Writer.WriteLine( $"[ContainerField({s_Member.Offset}{s_CustomNameProperty})]" );
                    p_Writer.WriteLine( $"[JsonProperty(Order = {s_Member.Offset})]" );

           
                    if (s_Pointer && s_Array)
                    {
                        p_Writer.WriteLine($"public RefArray<{s_FieldMemberType.Name}> {s_MemberName} {{ get; set; }} = new();");
                    }
                    else if (s_Pointer && !s_Array)
                    {
                        p_Writer.WriteLine($"public CtrRef<{s_FieldMemberType.Name}> {s_MemberName} {{ get; set; }} = new();");
                    }
                    else if (!s_Pointer && !s_Array)
                    {
                        if (s_FieldMemberType is TypeDataPrimitive)
                        {
                            WritePrimitiveMember(p_Writer, s_Member.BaseType, s_MemberName, s_Member.DefaultValue);
                        }
                        else if (s_FieldMemberType is TypeDataEnum)
                        {
                            var s_EnumData = s_FieldMemberType as TypeDataEnum;

                            
                            var s_DefaultValue = s_Member.DefaultValue?.AsLong();
                            
                            if (s_DefaultValue != null)
                            {
                                var s_FieldMember = s_EnumData.EnumDict
                                    .Where(x => x.Value == s_DefaultValue!)
                                    .Select(x => x.Key)
                                    .FirstOrDefault();

                                if (s_FieldMember != null)
                                {
                                    p_Writer.WriteLine($"public {s_EnumData!.Name} {s_Member.Name} {{ get; set; }} = {s_EnumData!.Name}.{s_FieldMember};");
                                }
                                else
                                {
                                    p_Writer.WriteLine($"public {s_EnumData!.Name} {s_Member.Name} {{ get; set; }} = ({s_EnumData!.Name}){s_DefaultValue};");

                                }
                            }
                            else
                            {
                                p_Writer.WriteLine($"public {s_EnumData!.Name} {s_Member.Name} {{ get; set; }}");
                            }
                        }
                        else
                        {
                            if (s_Member.DefaultValue is TypeDefaultInstance)
                            {
                                p_Writer.WriteLine(
                                    $"public {s_FieldMemberType.Name} {s_MemberName} {{ get; set; }} = new();");
                            }
                            else
                            {
                                p_Writer.WriteLine(
                                    $"public {s_FieldMemberType.Name} {s_MemberName} {{ get; set; }} = new();");
                            }
                        }
                    }
                    else if (!s_Pointer && s_Array)
                    {
                        p_Writer.WriteLine($"public List<{s_FieldMemberType.Name}> {s_MemberName} {{ get; set; }} = new();");
                    }

                    //p_Writer.WriteLine( $"public {FieldType} {p_Type.Name} {{ get; set; }}{EqualString} //0x{p_Type.Offset:X} ({p_Type.Offset})" );

                    p_Writer.WriteLine( );
                    
                });

                
                this.GenerateSerializer(p_Writer, p_Type, p_Type.InherritedTypes.FirstOrDefault()?.Data?.GetSize() ?? 0);
                
                //this.GenerateBindings( p_Writer, p_Type );
                //this.GenerateValueByHash( p_Writer, p_Type );
                //this.GenerateFieldInfoByHash( p_Writer, p_Type );
            }
            p_Writer.Indent--;
            p_Writer.WriteLine( "}" );

        }

        void GenerateValueType( IndentedTextWriter p_Writer, TypeDataValueType p_Type )
        {

            var s_AlignAttribute = p_Type.FindAttributeIgnoreCase( "align" ) as TypeNumeralAttribute;
            var s_SizeAttribute = p_Type.FindAttributeIgnoreCase("size") as TypeNumeralAttribute;
            
            if (s_AlignAttribute == null ||
                s_SizeAttribute == null)
                throw new System.Exception("Why is this happening");
            
            p_Writer.WriteLine( $"[ContainerType({s_AlignAttribute.Value}, {s_SizeAttribute.Value})]" );

            
            p_Writer.WriteLine( $"public class {p_Type.Name}" );

            p_Writer.Indent++;
            p_Writer.WriteLine( ": EbxSerializable" );
            p_Writer.Indent--;

            p_Writer.WriteLine( "{" );
            p_Writer.Indent++;
            {
                p_Type.Children
                    .OrderBy( x => (x as TypeDataMember)?.Offset)
                    .ToList( )
                    .ForEach( x =>
                {
                    if (x is not TypeDataMember)
                    {
                        
                        this.GenerateType(p_Writer, x);
                        return;
                    }

                    var s_Member = x as TypeDataMember;

                    var s_FieldMemberType = s_Member.GetFieldType(); 
                    var s_Pointer = s_FieldMemberType is TypeDataClass;
                    var s_Array = s_Member.IsArray();
                    
                    p_Writer.WriteLine( $"[ContainerField({s_Member.Offset})]" );
                    p_Writer.WriteLine( $"[JsonProperty(Order = {s_Member.Offset})]" );

                    if (s_Pointer && s_Array)
                    {
                        if ((s_Member.DefaultValue as TypeDefaultArray)?.Elements.Count() > 0)
                            throw new Exception("Initialized array not allowed!");
                        
                        p_Writer.WriteLine($"public RefArray<{s_FieldMemberType.Name}> {s_Member.Name} {{ get; set; }} = new();");
                    }
                    else if (s_Pointer && !s_Array)
                    {
                        if (s_Member.DefaultValue is TypeDefaultInstance)
                            throw new Exception("Class cannot be initialized! not supported..");
                        
                        p_Writer.WriteLine($"public CtrRef<{s_FieldMemberType.Name}> {s_Member.Name} {{ get; set; }} = new();");
                    }
                    else if (!s_Pointer && !s_Array)
                    {
                        if (s_FieldMemberType is TypeDataPrimitive)
                        {
                            WritePrimitiveMember(p_Writer, s_Member.BaseType, s_Member.Name, s_Member.DefaultValue);
                        }
                        else if (s_FieldMemberType is TypeDataEnum)
                        {
                            var s_EnumData = s_FieldMemberType as TypeDataEnum;

                            
                            var s_DefaultValue = s_Member.DefaultValue?.AsLong();
                            
                            if (s_DefaultValue != null)
                            {
                                var s_FieldMember = s_EnumData.EnumDict
                                    .Where(x => x.Value == s_DefaultValue!)
                                    .Select(x => x.Key)
                                    .FirstOrDefault();

                                if (s_FieldMember != null)
                                {
                                    p_Writer.WriteLine($"public {s_EnumData!.Name} {s_Member.Name} {{ get; set; }} = {s_EnumData!.Name}.{s_FieldMember};");
                                }
                                else
                                {
                                    p_Writer.WriteLine($"public {s_EnumData!.Name} {s_Member.Name} {{ get; set; }} = ({s_EnumData!.Name}){s_DefaultValue};");
                                }
                             }
                            else
                            {
                                p_Writer.WriteLine($"public {s_EnumData!.Name} {s_Member.Name} {{ get; set; }}");
                            }
                        }
                        else
                        {
                            if ((s_Member.DefaultValue == null))
                                throw new Exception("Cannot happen!");

                            if (s_Member.DefaultValue is TypeDefaultInstance)
                            {
                                var s_InstanceData = (s_Member.DefaultValue as TypeDefaultInstance)!;
                                

                                p_Writer.Write(
                                    $"public {s_FieldMemberType.Name} {s_Member.Name} {{ get; set; }} = ");
                                GenerateDefaultInstance(p_Writer, s_FieldMemberType, s_InstanceData);

                            }
                            else
                            {
                                
                                throw new Exception("Cannot happen 2!");
                            }
                        }
                    }
                    else if (!s_Pointer && s_Array)
                    {
                        if ((s_Member.DefaultValue as TypeDefaultArray)?.Elements.Count() > 0)
                            throw new Exception("Initialized array not allowed!");
                        
                        p_Writer.WriteLine($"public List<{s_FieldMemberType.Name}> {s_Member.Name} {{ get; set; }} = new();");
                    }

                    //p_Writer.WriteLine( $"public {FieldType} {p_Type.Name} {{ get; set; }}{EqualString} //0x{p_Type.Offset:X} ({p_Type.Offset})" );

                    p_Writer.WriteLine( );
                    
                });


                //this.GenerateBindings( p_Writer, p_Type );
                //this.GenerateValueByHash( p_Writer, p_Type );
                //this.GenerateFieldInfoByHash( p_Writer, p_Type );
            }
            p_Writer.Indent--;
            p_Writer.WriteLine( "}" );
        }




        void GenerateSerializer(IndentedTextWriter p_Writer, TypeDataBase p_Type, long p_StartOffset)
        {
            p_Writer.WriteLine( "public override void Serialize(RimeWriter p_Writer, IEbxWriter p_EbxWriter)" );
            p_Writer.WriteLine( "{" );
            p_Writer.Indent++;
            {
                p_Writer.WriteLine( "base.Serialize(p_Writer, p_EbxWriter);" );
                
                
                var s_CurrentOffset = p_StartOffset;
                
                p_Type.Children
                    .Where( x => x is TypeDataMember )
                    .Select( x => x as TypeDataMember )
                    .OrderBy( x => x!.Offset)
                    .ToList( )
                    .ForEach( p_Member =>
                {
                    var s_MemberName = p_Member.Name;

                    while (p_Type is TypeDataClass &&
                        (p_Type as TypeDataClass)!.InherritedTypes.Any(x => x.Data.HasMember((s_MemberName))))
                     s_MemberName += "_";

                    if (s_CurrentOffset > p_Member.Offset)
                        throw new Exception("Offset is off. Bad fbt probably.");

                    var s_BytesToSkip = p_Member.Offset - s_CurrentOffset;
                    if (s_BytesToSkip > 0)
                    {
                        p_Writer.WriteLine($"p_Writer.WriteNullBytes({s_BytesToSkip});");
                        s_CurrentOffset += s_BytesToSkip;
                    }

                    var s_FieldMemberType = p_Member.GetFieldType(); 
                    var s_Pointer = s_FieldMemberType is TypeDataClass;
                    var s_Array =p_Member.IsArray();

                    if (s_Pointer && s_Array)
                    {
                        p_Writer.WriteLine($"(RimeWriter Writer, uint ArrayIndex) s_{s_MemberName} = p_EbxWriter.GetArrayWriter({s_MemberName}.GetType(), {s_MemberName}.Count);");
                        p_Writer.WriteLine($"p_Writer.Write(s_{s_MemberName}.ArrayIndex);");
                        
                        p_Writer.WriteLine($"foreach (var s_Entry in {s_MemberName})");
                        p_Writer.WriteLine($"{{");
                        
                        p_Writer.Indent += 1;
                        {
                            p_Writer.WriteLine($"s_{s_MemberName}.Writer.Write(p_EbxWriter.WriteImport(s_Entry));");
                        }
                        p_Writer.Indent -= 1;
                        p_Writer.WriteLine($"}}");
                        
                        s_CurrentOffset += 4;
                    }
                    else if (s_Pointer && !s_Array)
                    {
                        // CtrRef
                        p_Writer.WriteLine($"p_Writer.Write(p_EbxWriter.WriteImport({s_MemberName}));");
                        s_CurrentOffset += 4;
                    }
                    else if (!s_Pointer && !s_Array && p_Member.BaseType.Data is TypeDataPrimitive)
                    { 
                        switch (p_Member.BaseType.Data.Name.ToLower())
                        {
                        case "cstring":
                            p_Writer.WriteLine($"p_Writer.Write(p_EbxWriter.WriteString({s_MemberName}));");
                            break;
                        case "float32":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "float64":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "boolean":
                        case "bool":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "int64":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "uint64":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "int32":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "uint32":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "int16":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "uint16":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "int8":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "uint8":
                            p_Writer.WriteLine($"p_Writer.Write({s_MemberName});");
                            break;
                        case "guid":
                            p_Writer.WriteLine($"{s_MemberName}.Serialize(p_Writer);");
                            break;
                        default:
                            throw new Exception("TODO");

                        }
                        s_CurrentOffset += p_Member.BaseType.Data?.GetSize() ?? 4;
                    }
                    else if (!s_Pointer && !s_Array)
                    {
                        if (p_Member.BaseType.Data is TypeDataEnum)
                        {
                            p_Writer.WriteLine($"p_Writer.Write((int) {s_MemberName});");
                            
                            // does enums other than 32 bit exist?
                            s_CurrentOffset += p_Member.BaseType.Data.GetSize().Value;   
                        }
                        else
                        {
                            p_Writer.WriteLine($"{s_MemberName}.Serialize(p_Writer, p_EbxWriter);");
                            s_CurrentOffset += p_Member.BaseType.Data.GetSize().Value;   
                        }
                    }
                    else
                    {
                        
                        // Array of values.
                        p_Writer.WriteLine($"(RimeWriter Writer, uint ArrayIndex) s_{s_MemberName} = p_EbxWriter.GetArrayWriter({s_MemberName}.GetType(), {s_MemberName}.Count);");
                        p_Writer.WriteLine($"p_Writer.Write(s_{s_MemberName}.ArrayIndex);");

                        p_Writer.WriteLine($"foreach (var s_Entry in {s_MemberName})");
                        p_Writer.WriteLine($"{{");

                        p_Writer.Indent += 1;

                        if (p_Member.BaseType.Data is TypeDataPrimitive)
                        {
                            switch (p_Member.BaseType.Data.Name.ToLower())
                            {
                                case "cstring":
                                    p_Writer.WriteLine($"s_{s_MemberName}.Writer.Write(p_EbxWriter.WriteString(s_Entry));");
                                    break;
                                case "float32":
                                case "float64":
                                case "boolean":
                                case "bool":
                                case "int64":
                                case "int32":
                                case "int16":
                                case "int8":
                                case "uint64":
                                case "uint32":
                                case "uint16":
                                case "uint8":
                                    p_Writer.WriteLine($"s_{s_MemberName}.Writer.Write(s_Entry);");
                                    break;
                                case "guid":
                                    p_Writer.WriteLine($"s_Entry.Serialize(s_{s_MemberName}.Writer);");
                                    break;
                                default:
                                    throw new Exception("TODO2");
                                    p_Writer.WriteLine($"s_{s_MemberName}.Writer.Write((int) s_Entry);");

                                    break;
                            }
                        }
                        else
                        {
                            p_Writer.WriteLine($"s_Entry.Serialize(s_{s_MemberName}.Writer, p_EbxWriter);");
                        }

                        p_Writer.Indent -= 1;
                        p_Writer.WriteLine($"}}");

                        s_CurrentOffset += 4;
                    }
                } );

                var s_LeftClassBytes = p_Type.GetSize().Value - s_CurrentOffset;
                if (s_LeftClassBytes > 0)
                    p_Writer.WriteLine($"p_Writer.WriteNullBytes({s_LeftClassBytes});");

     
            }
            p_Writer.Indent--;
            p_Writer.WriteLine( "}" );
        }
        
        void GenerateBindings( IndentedTextWriter p_Writer, TypeDataBase p_Type )
        {
            p_Writer.WriteLine( "public override void Bind(FieldDescriptor p_Descriptor, object p_Value)" );
            p_Writer.WriteLine( "{" );
            p_Writer.Indent++;
            {
                p_Writer.WriteLine( "switch ((uint)p_Descriptor.NameHash)" );
                p_Writer.WriteLine( "{" );

                p_Writer.Indent++;
                {
                    //TODO print members

                    p_Type.Children
                        .Where( x => x is TypeDataMember )
                        .Select( x => x as TypeDataMember )
                        .OrderBy( x => x!.Offset )
                        .ToList( ).ForEach( x =>
                     {
                         p_Writer.WriteLine( $"case 0x{Util.Hash( x.Name ):X}:" );
                         p_Writer.Indent++;
                         {
                             if ( x.BaseType is TypeDataEnum )
                                 p_Writer.WriteLine( $"this.{x.Name} = ({x.BaseType.Data.Name}) Enum.ToObject(typeof({x.BaseType.Data.Name}), p_Value);" );
                             else if ( x.BaseType is TypeDataClass )
                                 p_Writer.WriteLine( $"this.{x.Name} = (CtrRef<{x.BaseType.Data.Name}>) p_Value;" );
                             else if ( x.BaseType != null )
                                 p_Writer.WriteLine( $"this.{x.Name} = ({x.BaseType.Data.Name}) p_Value;" );
                             else
                                 p_Writer.WriteLine( $"this.{x.Name} = ({x.BaseType.TypeName}) p_Value;" );

                             p_Writer.WriteLine( "break;" );
                         }
                         p_Writer.Indent--;
                     } );

                    p_Writer.WriteLine( "default:" );
                    p_Writer.Indent++;
                    {
                        p_Writer.WriteLine( "base.Bind(p_Descriptor, p_Value);" );
                        p_Writer.WriteLine( "break;" );
                    }
                    p_Writer.Indent--;
                }
                p_Writer.Indent--;
                p_Writer.WriteLine( "}" );

            }
            p_Writer.Indent--;
            p_Writer.WriteLine( "}" );

        }

        void GenerateValueByHash( IndentedTextWriter p_Writer, TypeDataBase p_Type )
        {
            p_Writer.WriteLine( "public override object GetFieldValueByHash(uint p_Hash)" );
            p_Writer.WriteLine( "{" );
            p_Writer.Indent++;
            {
                p_Writer.WriteLine( "switch (p_Hash)" );
                p_Writer.WriteLine( "{" );

                p_Writer.Indent++;
                {
                    //TODO print members

                    p_Type.Children
                        .Where( x => x is TypeDataMember )
                        .Select( x => x as TypeDataMember )
                        .OrderBy( x => x!.Offset )
                        .ToList( )
                        .ForEach( x =>
                    {
                        p_Writer.WriteLine( $"case 0x{Util.Hash( x.Name ):X}:" );
                        p_Writer.Indent++;
                        {
                            p_Writer.WriteLine( $"return this.{x.Name};" );
                        }
                        p_Writer.Indent--;
                    } );

                    p_Writer.WriteLine( "default:" );
                    p_Writer.Indent++;
                    {
                        p_Writer.WriteLine( "return base.GetFieldValueByHash(p_Hash);" );
                    }
                    p_Writer.Indent--;
                }
                p_Writer.Indent--;
                p_Writer.WriteLine( "}" );

            }
            p_Writer.Indent--;
            p_Writer.WriteLine( "}" );
            p_Writer.WriteLine( );

        }

        void GenerateFieldInfoByHash( IndentedTextWriter p_Writer, TypeDataBase p_Type )
        {
            p_Writer.WriteLine( "public override PropertyInfo GetFieldInfoByHash(uint p_Hash)" );
            p_Writer.WriteLine( "{" );
            p_Writer.Indent++;
            {
                p_Writer.WriteLine( "switch (p_Hash)" );
                p_Writer.WriteLine( "{" );

                p_Writer.Indent++;
                {
                    //TODO print members

                    p_Type.Children
                        .Where( x => x is TypeDataMember )
                        .Select( x => x as TypeDataMember )
                        .OrderBy( x => x!.Offset )
                        .ToList( )
                        .ForEach( x =>
                    {
                        p_Writer.WriteLine( $"case 0x{Util.Hash( x.Name ):X}:" );
                        p_Writer.Indent++;
                        {
                            p_Writer.WriteLine( $"return typeof({p_Type.Name}).GetProperty(nameof({x.Name}));" );
                        }
                        p_Writer.Indent--;
                    } );

                    p_Writer.WriteLine( "default:" );
                    p_Writer.Indent++;
                    {
                        p_Writer.WriteLine( "return base.GetFieldInfoByHash(p_Hash);" );
                    }
                    p_Writer.Indent--;
                }
                p_Writer.Indent--;
                p_Writer.WriteLine( "}" );

            }
            p_Writer.Indent--;
            p_Writer.WriteLine( "}" );
            p_Writer.WriteLine( );

        }

        void GenerateMember( IndentedTextWriter p_Writer, TypeDataMember p_Type )
        {
           
        }
        
        private void WritePrimitiveMember(IndentedTextWriter p_Writer, TypeDataBase p_MemberType, string p_MemberName, TypeDefault? p_Default)
        {
            switch (p_MemberType.Name.ToLower())
            {
                
                case "string":
                    if (p_Default?.AsString() != null)
                    {
                        if(p_Default.AsString() == string.Empty)
                            p_Writer.WriteLine($"public string {p_MemberName} {{ get; set; }} = string.Empty;"); 
                        else
                            p_Writer.WriteLine($"public string {p_MemberName} {{ get; set; }} = \"{p_Default.AsString()!}\";"); 
                    }
                    else
                    {
                        p_Writer.WriteLine($"public string {p_MemberName} {{ get; set; }} = string.Empty;");    
                    }
                    break;
                case "guid":
                    p_Writer.WriteLine($"public string {p_MemberName} {{ get; set; }} = GUID.Empty;");
                    break;
                
                case "int8":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public sbyte {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "int16":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public short {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "int32":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public int {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "int64":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public long {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "uint8":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public byte {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "uint16":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public ushort {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "uint32":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public uint {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                case "uint64":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public ulong {p_MemberName} {{ get; set; }} = {p_Default.AsLong()!};");   
                    break;
                
                case "bool":
                case "boolean":
                    if (p_Default?.AsLong() == null)
                        goto default;
                    p_Writer.WriteLine($"public bool {p_MemberName} {{ get; set; }} = {(p_Default.AsLong()! != 0).ToString().ToLower()};");   
                    break;
                
                case "float32":
                    if (p_Default?.AsDouble() == null)
                        goto default;
                    
                    p_Writer.WriteLine($"public float {p_MemberName} {{ get; set; }} = {p_Default.AsDouble()!:F}f;");   
                    break;
                
                case "float64":
                    if (p_Default?.AsDouble() == null)
                        goto default;

                    p_Writer.WriteLine($"public double {p_MemberName} {{ get; set; }} = {p_Default?.AsDouble():F};");   
                    break;
                default:
                    p_Writer.WriteLine($"public {p_MemberType.Name} {p_MemberName} {{ get; set; }}");
                    break;
            }
        }

        void GenerateDefaultInstance(IndentedTextWriter p_Writer, TypeDataBase p_Type, TypeDefaultInstance p_Default, bool p_IsInInstance = false)
        {
            if (!p_Default.Fields.Any())
            {
                if(p_IsInInstance)
                    p_Writer.WriteLine("new (),");
                else
                    p_Writer.WriteLine("new ();");
                return;
            }
            
            p_Writer.WriteLine("new ()");
            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            
            foreach (var s_Field in p_Default.Fields)
            {
                var s_Member = p_Type.Children
                    .Where(x => x.Name == s_Field.Key)
                    .Select(x => x as TypeDataMember)
                    .FirstOrDefault();

                if (s_Member == null)
                    throw new Exception("Yo shits null!");

                if (s_Field.Value is TypeDefaultInstance)
                {
                    p_Writer.Write($"{s_Field.Key} = ");
                    GenerateDefaultInstance(p_Writer, s_Member.BaseType, (s_Field.Value as TypeDefaultInstance)!, true);
                }
                else
                { 
                    switch (s_Member.BaseType.TypeName.ToLower())
                    {
                        
                        case "string":
                            if (s_Field.Value?.AsString() != null)
                            {
                                if(s_Field.Value.AsString() == string.Empty)
                                    p_Writer.WriteLine($"{s_Field.Key} = string.Empty,"); 
                                else
                                    p_Writer.WriteLine($"{s_Field.Key} = \"{s_Field.Value.AsString()!}\","); 
                            }
                            else
                            {
                                p_Writer.WriteLine($"{s_Field.Key} = string.Empty,");    
                            }
                            break;
                        case "guid":
                            p_Writer.WriteLine($"{s_Field.Key} = GUID.Empty,");
                            break;
                        
                        case "int8":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "int16":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "int32":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "int64":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "uint8":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "uint16":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "uint32":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        case "uint64":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsLong()!},");   
                            break;
                        
                        case "bool":
                        case "boolean":
                            if (s_Field.Value?.AsLong() == null)
                                goto default;
                            p_Writer.WriteLine($"{s_Field.Key} = {(s_Field.Value.AsLong()! != 0).ToString().ToLower()},");   
                            break;
                        
                        case "float32":
                            if (s_Field.Value?.AsDouble() == null)
                                goto default;
                            
                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value.AsDouble()!:F}f,");   
                            break;
                        
                        case "float64":
                            if (s_Field.Value?.AsDouble() == null)
                                goto default;

                            p_Writer.WriteLine($"{s_Field.Key} = {s_Field.Value?.AsDouble()},");   
                            break;
                        default:
                            throw new NotImplementedException();
                            break;
                    }
                }
                
                
            }

            p_Writer.Indent--;
            if(p_IsInInstance)
                p_Writer.WriteLine("},");
            else
                p_Writer.WriteLine("};");
        }
        
    }
}
