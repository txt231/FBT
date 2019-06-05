using FBT.TypeData.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.Module
{
    public class ModuleInfo
    {
        public ModuleInfo( string p_Name )
        {
            this.Name = p_Name;

            this.Types = new List<TypeDataBase>( );
        }

        public string Name;

        public List<TypeDataBase> Types;


        public void AddType( TypeDataBase p_Type ) => this.Types.Add( p_Type );
    }
}
