using FBT.TypeData.Base;
using FBT.TypeData.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData.Member
{
    public class TypeDataClass
        : TypeDataBase
    {
        public string Protection = null;

        public List<string> UnresolvedInherritedTypes = new List<string>( );
        public List<TypeDataBase> InherritedTypes = new List<TypeDataBase>( );

        public int GetClassId( )
        {
            var s_NumeralAttribute = this.FindAttributeIgnoreCase( "classId" ) as TypeNumeralAttribute;

            if ( s_NumeralAttribute == null )
                return -1;

            return s_NumeralAttribute.Value;
        }

        public int GetLastSubClassId( )
        {
            var s_NumeralAttribute = this.FindAttributeIgnoreCase( "lastSubClassId" ) as TypeNumeralAttribute;

            if ( s_NumeralAttribute == null )
                return -1;

            return s_NumeralAttribute.Value;
        }
    }
}
