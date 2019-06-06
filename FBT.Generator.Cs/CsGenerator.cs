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

namespace FBT.Generator.Cs
{
    [LanguageGenerator("Cs")]
    public class CsGenerator
        : FBTCodeGenerator
    {
        public CsGenerator(string p_Name, string p_Path)
            : base(p_Name, p_Path)
        {
        }

        const string s_Extention = ".cs";


        public override void GenerateUnit(TypeUnit p_Unit, string p_Name)
        {
            // -------------------------------------------------------------------------------------
            //
            // Yo, we can use the new assembly generator thingy instead of writing source files.....
            //
            // -------------------------------------------------------------------------------------

            using (var s_FileStream = new FileStream(this.OutPath + "/" + p_Name + s_Extention, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            {
                using (var s_StreamWriter = new StreamWriter(s_FileStream))
                {
                    using (var s_Writer = new IndentedTextWriter(s_StreamWriter, "\t"))
                    {


                        //Warning
                        s_Writer.WriteLine("///////////////////////////////////////////////////////////////");
                        s_Writer.WriteLine("//                                                           //");
                        s_Writer.WriteLine("// This is an automatically generated file.                  //");
                        s_Writer.WriteLine("// Do *NOT* modify unless you really know what you're doing. //");
                        s_Writer.WriteLine("//                                                           //");
                        s_Writer.WriteLine("///////////////////////////////////////////////////////////////");

                        s_Writer.WriteLine();

                        //Include 
                        s_Writer.WriteLine("using System;");
                        s_Writer.WriteLine("using System.Collections.Generic;");
                        s_Writer.WriteLine("using System.Linq;");
                        s_Writer.WriteLine("using System.ComponentModel;");
                        s_Writer.WriteLine("using System.Reflection;");

                        s_Writer.WriteLine();

                        s_Writer.WriteLine("using RimeLib.Frostbite.Ebx.Fb2;");
                        s_Writer.WriteLine("using RimeLib.IO;");
                        s_Writer.WriteLine("using RimeLib.Frostbite.Containers;");
                        s_Writer.WriteLine("using RimeLib.Frostbite.Core;");
                        s_Writer.WriteLine("using RimeLib.Frostbite.Ebx;");

                        s_Writer.WriteLine();



                        GenerateNamespaceBegin(s_Writer, p_Unit);

                        s_Writer.WriteLine();

                        p_Unit.Children.ForEach(x => this.GenerateType(s_Writer, x));

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

            var s_NamespaceName = p_Unit.Namespace.Replace("::", ".");

            p_Writer.WriteLine($"namespace {s_NamespaceName}");
            p_Writer.WriteLine("{");

            p_Writer.Indent++;
        }

        void GenerateNamespaceEnd(IndentedTextWriter p_Writer, TypeUnit p_Unit)
        {
            if (p_Unit.Namespace == null)
                return;

            p_Writer.Indent--;

            p_Writer.WriteLine("}");
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
            var s_DataContainerBase = p_Type.InherritedTypes.FirstOrDefault(x => x.Name == "DataContainer");


            //TODO = Rimelib FrostbiteContainer

            var s_AlignAttribute = p_Type.FindAttributeIgnoreCase("align") as TypeNumeralAttribute;
            if (s_AlignAttribute != null)
                p_Writer.WriteLine($"[ContainerType({s_AlignAttribute.Value})]");
            else
                p_Writer.WriteLine("[ContainerType]");


            p_Writer.WriteLine($"public class {p_Type.Name}");

            p_Writer.Indent++;
            {

                bool FirstInherrit = true;

                foreach (var s_Inherrit in p_Type.InherritedTypes)
                {
                    p_Writer.WriteLine($"{(FirstInherrit ? ":" : ",")} {s_Inherrit.Name}");

                    FirstInherrit = false;
                }

            }
            p_Writer.Indent--;

            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Type.Children.ForEach(x => this.GenerateType(p_Writer, x));


                this.GenerateBindings(p_Writer, p_Type);
                this.GenerateValueByHash(p_Writer, p_Type);
                this.GenerateFieldInfoByHash(p_Writer, p_Type);
            }
            p_Writer.Indent--;
            p_Writer.WriteLine("}");

        }

        void GenerateValueType(IndentedTextWriter p_Writer, TypeDataValueType p_Type)
        {

            var s_AlignAttribute = p_Type.FindAttributeIgnoreCase("align") as TypeNumeralAttribute;
            if (s_AlignAttribute != null)
                p_Writer.WriteLine($"[ContainerType({s_AlignAttribute.Value})]");
            else
                p_Writer.WriteLine("[ContainerType]");


            p_Writer.WriteLine($"public class {p_Type.Name}");

            p_Writer.Indent++;
            p_Writer.WriteLine(": FrostbiteContainer");
            p_Writer.Indent--;

            p_Writer.WriteLine("{");
            p_Writer.Indent++;
            {
                p_Type.Children.ForEach(x => this.GenerateType(p_Writer, x));


                this.GenerateBindings(p_Writer, p_Type);
                this.GenerateValueByHash(p_Writer, p_Type);
                this.GenerateFieldInfoByHash(p_Writer, p_Type);
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
                           p_Writer.WriteLine($"this.{x.Name} = ({x.BaseType.Name}) Enum.ToObject(typeof({x.BaseType.Name}), p_Value);");
                       else if (x.BaseType is TypeDataClass)
                           p_Writer.WriteLine($"this.{x.Name} = (CtrRef<{x.BaseType.Name}>) p_Value;");
                       else if (x.BaseType != null)
                           p_Writer.WriteLine($"this.{x.Name} = ({x.BaseType.Name}) p_Value;");
                       else
                           p_Writer.WriteLine($"this.{x.Name} = ({x.UnresolvedBaseType}) p_Value;");

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

            string FieldType = (p_Type.BaseType != null ? p_Type.BaseType.Name : p_Type.UnresolvedBaseType);

            if (p_Type.BaseType is TypeDataClass)
                FieldType = $"CtrRef<{p_Type.BaseType.Name}>";


            string EqualString = "";

            if (p_Type.BaseType is TypeDataClass)
                EqualString = $" = new CtrRef<{p_Type.BaseType.Name}>();";
            else if (p_Type.BaseType is TypeDataClass)
                EqualString = $" = new {p_Type.BaseType.Name}();";


            //TODO: Array

            p_Writer.WriteLine($"public {FieldType} {p_Type.Name} {{ get; set; }}{EqualString} //0x{p_Type.Offset:X} ({p_Type.Offset})");

            p_Writer.WriteLine();
        }
    }
}
