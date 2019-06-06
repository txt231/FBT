using FBT.TypeData;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.Generator
{
    public abstract class FBTCodeGenerator
    {
        public FBTCodeGenerator(string p_Name, string p_Path = "./")
        {
            this.OutPath = p_Path;
        }

        public string OutPath;

        public virtual void PreBuild()
        {
        }

        public virtual void PostBuild()
        {
        }

        public abstract void GenerateUnit(TypeUnit p_Unit, string p_Name);
    }
}
