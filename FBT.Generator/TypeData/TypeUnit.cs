using FBT.Module;
using FBT.TypeData.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData
{
    public class TypeUnit
    {
        public TypeUnit( string p_Namespace )
        {
            this.Namespace = p_Namespace;
        }

        public List<string> Includes = new List<string>( );

        public ModuleInfo Module;
        public string Namespace = null;
        public string Path = null;

        public List<TypeDataBase> Children = new List<TypeDataBase>( );

        public bool IsResolved = false;

        public void AddInclude( string p_Include )
        {
            this.Includes.Add( p_Include );
        }
    }
}
