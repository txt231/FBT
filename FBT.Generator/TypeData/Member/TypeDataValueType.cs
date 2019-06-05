using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData.Member
{
    public class TypeDataValueType
        : TypeDataBase
    {
        public string Protection = null;


        public int GetSize( )
        {
            var s_NumeralAttribute = this.FindAttributeIgnoreCase( "size" ) as TypeNumeralAttribute;

            if ( s_NumeralAttribute == null )
                return -1;

            return s_NumeralAttribute.Value;
        }

    }
}
