using FBT.TypeData;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.Generator
{
    public abstract class FBTCodeGenerator
    {
        public FBTCodeGenerator( string p_Name, string p_Path )
        {
        }


        public abstract void GenerateUnit( TypeUnit p_Unit, string p_Name, string p_OutPath = "./" );
    }
}
