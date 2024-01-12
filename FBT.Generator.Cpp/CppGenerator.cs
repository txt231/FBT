using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using FBT.Generator;
using FBT.TypeData;
using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;
using FBT.TypeData.Member;

namespace FBT.Generator.Cpp
{
    [LanguageGenerator("Cpp")]
    public class CppGenerator
        : FBTCodeGenerator
    {
        public CppGenerator(string p_Name, string p_Path)
            : base(p_Name, p_Path)
        {
        }
        const string s_DefsExtention = "Defs.h";
        const string s_ClassExtention = ".h";


        public override void GenerateUnit(TypeUnit p_Unit, string p_Name)
        {
            using (var s_FileStream = new FileStream(this.OutPath + "/" + p_Name + s_DefsExtention, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            {
                using (var s_StreamWriter = new StreamWriter(s_FileStream))
                {
                    using (var s_Writer = new IndentedTextWriter(s_StreamWriter, "\t"))
                    {


                        //Warning
                        s_Writer.WriteLine("///////////////////////////////////////////////////////////////");
                        s_Writer.WriteLine("//                                                           //");
                        s_Writer.WriteLine("// FBT.Compiler                                              //");
                        s_Writer.WriteLine("// This is an automatically generated definitions file.      //");
                        s_Writer.WriteLine("//                                                           //");
                        s_Writer.WriteLine("///////////////////////////////////////////////////////////////");

                        s_Writer.WriteLine();

                        //Include 
                        foreach (var s_Include in p_Unit.Includes)
                        {
                            var s_File = Path.GetFileNameWithoutExtension(s_Include);


                            s_Writer.WriteLine($"#include \"{s_File + s_DefsExtention}\"");
                        }

                        s_Writer.WriteLine();


                        GenerateNamespaceBegin(s_Writer, p_Unit);

                        s_Writer.WriteLine();

                        p_Unit.Children.ForEach(x => this.GenerateDefinitions(s_Writer, x));

                        s_Writer.WriteLine();

                        GenerateNamespaceEnd(s_Writer, p_Unit);
                    }
                }
            }
        }

        #region Namespace 

        void GenerateNamespaceBegin(IndentedTextWriter p_Writer, TypeUnit p_Unit)
        {
            if (p_Unit.Namespace == null)
                return;

            foreach (var s_Namespace in p_Unit.Namespace.Replace("::", ".").Split('.'))
            {
                p_Writer.WriteLine($"namespace {s_Namespace}");
                p_Writer.WriteLine("{");

                p_Writer.Indent++;
            }
        }

        void GenerateNamespaceEnd(IndentedTextWriter p_Writer, TypeUnit p_Unit)
        {
            if (p_Unit.Namespace == null)
                return;

            foreach (var s_Namespace in p_Unit.Namespace.Replace("::", ".").Split('.'))
            {
                p_Writer.Indent--;

                p_Writer.WriteLine("}");
            }
        }

        #endregion

        #region Definitions

        void GenerateDefinitions(IndentedTextWriter p_Writer, TypeDataBase p_Type)
        {
            if (p_Type is TypeDataClass)
                this.GenerateClassDefinition(p_Writer, p_Type as TypeDataClass);
            else if (p_Type is TypeDataValueType)
                this.GenerateValueTypeDefinition(p_Writer, p_Type as TypeDataValueType);

        }

        void GenerateClassDefinition(IndentedTextWriter p_Writer, TypeDataClass p_Type)
        {
            p_Writer.WriteLine($"class {p_Type.Name};");
            p_Writer.WriteLine();
        }

        void GenerateValueTypeDefinition(IndentedTextWriter p_Writer, TypeDataValueType p_Type)
        {
            p_Writer.WriteLine($"struct {p_Type.Name};");
            p_Writer.WriteLine();
        }

        #endregion


        void GenerateType(IndentedTextWriter p_Writer, TypeDataBase p_Type)
        {
            if (p_Type is TypeDataClass)
                this.GenerateClass(p_Writer, p_Type as TypeDataClass);
            else if (p_Type is TypeDataValueType)
                this.GenerateValueType(p_Writer, p_Type as TypeDataValueType);
            else if (p_Type is TypeDataMember)
                this.GenerateMember(p_Writer, p_Type as TypeDataMember);
        }


        void GenerateClass(IndentedTextWriter p_Writer, TypeDataClass p_Type)
        {
            var s_DataContainerBase = p_Type.InherritedTypes.FirstOrDefault(x => x.Data.Name == "DataContainer");


            //TODO = Rimelib FrostbiteContainer

            var s_AlignAttribute = p_Type.FindAttributeIgnoreCase("align") as TypeNumeralAttribute;
            if (s_AlignAttribute != null)
                p_Writer.WriteLine($"#pragma push( pack, {s_AlignAttribute.Value} )"); //Not right pragma



            p_Writer.WriteLine($"class {p_Type.Name}");

            p_Writer.Indent++;
            {

                bool FirstInherrit = true;

                foreach (var s_Inherrit in p_Type.InherritedTypes)
                {
                    p_Writer.WriteLine($"{(FirstInherrit ? ":" : ",")} {s_Inherrit.Data.Name}");

                    FirstInherrit = false;
                }

            }
            p_Writer.Indent--;

            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Type.Children.ForEach(x => this.GenerateType(p_Writer, x));


                //this.GenerateBindings(p_Writer, p_Type);
                //this.GenerateValueByHash(p_Writer, p_Type);
                //this.GenerateFieldInfoByHash(p_Writer, p_Type);
            }
            p_Writer.Indent--;
            p_Writer.WriteLine("}");

            if (s_AlignAttribute != null)
                p_Writer.WriteLine($"#pragma push( pop )"); //Not right pragma


        }

        void GenerateValueType(IndentedTextWriter p_Writer, TypeDataValueType p_Type)
        {

            var s_AlignAttribute = p_Type.FindAttributeIgnoreCase("align") as TypeNumeralAttribute;
            if (s_AlignAttribute != null)
                p_Writer.WriteLine($"[ContainerType({s_AlignAttribute.Value})]");
            else
                p_Writer.WriteLine("[ContainerType]");


            p_Writer.WriteLine($"struct {p_Type.Name}");
            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Type.Children.ForEach(x => this.GenerateType(p_Writer, x));


                //this.GenerateBindings(p_Writer, p_Type);
                //this.GenerateValueByHash(p_Writer, p_Type);
                //this.GenerateFieldInfoByHash(p_Writer, p_Type);
            }
            p_Writer.Indent--;
            p_Writer.WriteLine("}");
        }


        void GenerateBindings(IndentedTextWriter p_Writer, TypeDataBase p_Type)
        {
            p_Writer.WriteLine("public override void Bind(FieldDescriptor p_Descriptor, object p_Value)");
            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Writer.WriteLine("switch ((uint)p_Descriptor.NameHash)");
                p_Writer.WriteLine("{");

                p_Writer.Indent++;
                {
                    //TODO print members

                    p_Type.Children.Where(x => x is TypeDataMember).Select(x => x as TypeDataMember).ToList().ForEach(x =>
               {
                   p_Writer.WriteLine($"case 0x{Util.Hash(x.Name):X}:");
                   p_Writer.Indent++;
                   {
                       if (x.BaseType is TypeDataEnum)
                           p_Writer.WriteLine($"this.{x.Name} = ({x.BaseType.Data.Name}) Enum.ToObject(typeof({x.BaseType.Data.Name}), p_Value);");
                       else if (x.BaseType is TypeDataClass)
                           p_Writer.WriteLine($"this.{x.Name} = (CtrRef<{x.BaseType.Data.Name}>) p_Value;");
                       else if (x.BaseType != null)
                           p_Writer.WriteLine($"this.{x.Name} = ({x.BaseType.Data.Name}) p_Value;");
                       else
                           p_Writer.WriteLine($"this.{x.Name} = ({x.BaseType.TypeName}) p_Value;");

                       p_Writer.WriteLine("break;");
                   }
                   p_Writer.Indent--;
               });

                    p_Writer.WriteLine("default:");
                    p_Writer.Indent++;
                    {
                        p_Writer.WriteLine("base.Bind(p_Descriptor, p_Value);");
                        p_Writer.WriteLine("break;");
                    }
                    p_Writer.Indent--;
                }
                p_Writer.Indent--;
                p_Writer.WriteLine("}");

            }
            p_Writer.Indent--;
            p_Writer.WriteLine("}");

        }

        void GenerateValueByHash(IndentedTextWriter p_Writer, TypeDataBase p_Type)
        {
            p_Writer.WriteLine("public override object GetFieldValueByHash(uint p_Hash)");
            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Writer.WriteLine("switch (p_Hash)");
                p_Writer.WriteLine("{");

                p_Writer.Indent++;
                {
                    //TODO print members

                    p_Type.Children.Where(x => x is TypeDataMember).Select(x => x as TypeDataMember).ToList().ForEach(x =>
              {
                  p_Writer.WriteLine($"case 0x{Util.Hash(x.Name):X}:");
                  p_Writer.Indent++;
                  {
                      p_Writer.WriteLine($"return this.{x.Name};");
                  }
                  p_Writer.Indent--;
              });

                    p_Writer.WriteLine("default:");
                    p_Writer.Indent++;
                    {
                        p_Writer.WriteLine("return base.GetFieldValueByHash(p_Hash);");
                    }
                    p_Writer.Indent--;
                }
                p_Writer.Indent--;
                p_Writer.WriteLine("}");

            }
            p_Writer.Indent--;
            p_Writer.WriteLine("}");
            p_Writer.WriteLine();

        }

        void GenerateFieldInfoByHash(IndentedTextWriter p_Writer, TypeDataBase p_Type)
        {
            p_Writer.WriteLine("public override PropertyInfo GetFieldInfoByHash(uint p_Hash)");
            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Writer.WriteLine("switch (p_Hash)");
                p_Writer.WriteLine("{");

                p_Writer.Indent++;
                {
                    //TODO print members

                    p_Type.Children.Where(x => x is TypeDataMember).Select(x => x as TypeDataMember).ToList().ForEach(x =>
              {
                  p_Writer.WriteLine($"case 0x{Util.Hash(x.Name):X}:");
                  p_Writer.Indent++;
                  {
                      p_Writer.WriteLine($"return typeof({p_Type.Name}).GetProperty(nameof({x.Name}));");
                  }
                  p_Writer.Indent--;
              });

                    p_Writer.WriteLine("default:");
                    p_Writer.Indent++;
                    {
                        p_Writer.WriteLine("return base.GetFieldInfoByHash(p_Hash);");
                    }
                    p_Writer.Indent--;
                }
                p_Writer.Indent--;
                p_Writer.WriteLine("}");

            }
            p_Writer.Indent--;
            p_Writer.WriteLine("}");
            p_Writer.WriteLine();

        }

        void GenerateMember(IndentedTextWriter p_Writer, TypeDataMember p_Type)
        {
            p_Writer.WriteLine($"[ContainerField({p_Type.Offset})]");

            string FieldType = (p_Type.BaseType != null ? p_Type.BaseType.Data.Name : p_Type.BaseType.TypeName);


            if (p_Type.BaseType.Data is TypeDataArray && (p_Type.BaseType.Data as TypeDataArray)!.ArrayType.Data is TypeDataClass)
                FieldType = $"RefArray<{(p_Type.BaseType.Data as TypeDataArray)!.ArrayType.Data.Name}>";
            else if (p_Type.BaseType.Data is TypeDataClass)
                FieldType = $"{p_Type.BaseType.Data.Name}*";
            else if (p_Type.BaseType.Data is TypeDataArray)
                FieldType = $"Array<{(FieldType)}>";



            p_Writer.WriteLine($"{FieldType} {p_Type.Name}; //0x{p_Type.Offset:08X} ({p_Type.Offset})");

            p_Writer.WriteLine();
        }
    }
}
