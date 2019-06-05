using FBT.Parser;
using FBT.TypeData;
using FBT.TypeData.Base;
using FBT.TypeData.Member;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FBT.TypeManager
{
    public class TypeUnitManager
    {
        public Dictionary<string, TypeUnit> Units = new Dictionary<string, TypeUnit>( );


        public TypeUnit GetUnit( string p_Name )
        {
            TypeUnit s_Unit = null;

            this.Units.TryGetValue( p_Name, out s_Unit );

            return s_Unit;
        }

        public void AddUnit( string p_Name, TypeUnit p_Unit ) => this.Units.Add( p_Name, p_Unit );


        private void ResolveType( TypeDataBase p_Type )
        {
            var s_ClassType = p_Type as TypeDataClass;

            if ( s_ClassType != null )
            {
                if ( s_ClassType.InherritedTypes.Count != s_ClassType.UnresolvedInherritedTypes.Count )
                {
                    foreach ( var s_UnresolvedInherrit in s_ClassType.UnresolvedInherritedTypes )
                    {
                        var s_InherritUnit = this.GetUnit( s_UnresolvedInherrit );

                        if ( s_InherritUnit == null )
                            continue;

                        s_ClassType.InherritedTypes.Add( s_InherritUnit.Children.FirstOrDefault( x => x.Name == s_UnresolvedInherrit ) );
                    }

                    s_ClassType.UnresolvedInherritedTypes.Clear( );
                }
            }

            var s_MemberType = p_Type as TypeDataMember;

            if ( s_MemberType != null )
            {
                if ( s_MemberType.BaseType == null && s_MemberType.UnresolvedBaseType != null )
                {
                    var s_InherritUnit = this.GetUnit( s_MemberType.UnresolvedBaseType );

                    if( s_InherritUnit  != null)
                        s_MemberType.BaseType = s_InherritUnit.Children.FirstOrDefault( x => x.Name == s_MemberType.UnresolvedBaseType );
                }
            }


            p_Type.Children.ForEach( x => ResolveType( x ) );
        }



        public void ResolveImports( )
        {
            var s_UnresolvedUnits = this.Units.Where( x => !x.Value.IsResolved );

            while ( true )
            {
                var s_Unit = this.Units.FirstOrDefault( x => !x.Value.IsResolved );

                if ( s_Unit.Value == null )
                    break;

                foreach ( var s_Include in s_Unit.Value.Includes )
                {
                    var s_IncludeUnit = this.GetUnit( Path.GetFileNameWithoutExtension( s_Include ) );

                    if ( s_IncludeUnit != null )
                        continue;

                    Console.WriteLine( $"Resolving import from {s_Unit.Key} -> {s_Include}" );

                    var s_File = new FBTFile( s_Unit.Value.Path + "/" + s_Include );
                }

                s_Unit.Value.IsResolved = true;
            }

            foreach ( var s_Unit in this.Units )
            {
                s_Unit.Value.Children.ForEach( x => ResolveType( x ) );
            }
        }


        static TypeUnitManager _Instance;
        public static TypeUnitManager Instance => _Instance ?? ( _Instance = new TypeUnitManager( ) );
    }
}
