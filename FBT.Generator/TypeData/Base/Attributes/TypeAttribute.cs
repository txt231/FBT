using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData.Base.Attributes
{
    public class TypeAttribute
    {
        public TypeAttribute( string p_Key )
        {
            this.Key = p_Key;
        }

        public string Key;
    }
}
