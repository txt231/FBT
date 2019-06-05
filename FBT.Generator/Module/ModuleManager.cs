using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBT.Module
{
    public class ModuleManager
    {
        List<ModuleInfo> Modules = new List<ModuleInfo>( );


        public ModuleInfo GetModule( string p_Name )
        {
            var s_Module = this.Modules.FirstOrDefault( x => x.Name == p_Name );

            if ( s_Module == null )
            {
                s_Module = new ModuleInfo( p_Name );
                this.Modules.Add( s_Module );
            }

            return s_Module;
        }


        static ModuleManager _Instance;
        public static ModuleManager Instance => _Instance ?? ( _Instance = new ModuleManager( ) );
    }
}
