using FBT.TypeData.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBT.TypeData.Base
{
    public class TypeDataBase
    {
        public string Name;

        public List<TypeAttribute> Attributes = new List<TypeAttribute>( );



        public List<TypeDataBase> Children = new List<TypeDataBase>( );


        public TypeAttribute FindAttribute( string p_Name ) => this.Attributes.FirstOrDefault( x => x.Key == p_Name);
        public TypeAttribute FindAttributeIgnoreCase( string p_Name ) => this.Attributes.FirstOrDefault( x => x.Key.ToLower( ) == p_Name.ToLower( ) );
    }
}
