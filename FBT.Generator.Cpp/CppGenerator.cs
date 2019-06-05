using System;
using System.Collections.Generic;
using System.Text;

using FBT.Generator;
using FBT.TypeData;

namespace FBT.Generator.Cpp
{
    [LanguageGenerator("Cpp")]
    public class CppGenerator
        : FBTCodeGenerator
    {
        public CppGenerator( string p_Name, string p_Path )
            : base( p_Name, p_Path)
        {
        }


        public override void GenerateUnit( TypeUnit p_Unit, string p_Name, string p_OutPath )
        {
        }
    }
}
