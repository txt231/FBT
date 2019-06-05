using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData.Base.Attributes
{
    public class TypeNumeralAttribute
        : TypeAttribute
    {
        public TypeNumeralAttribute( string p_Key, int p_Value )
            : base( p_Key )
        {
            this.Value = p_Value;
        }

        public int Value;
    }
}
