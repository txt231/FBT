
using CommandLine;
using FBT.Generator;
using FBT.Parser;
using FBT.TypeManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FBTC1
{
    class Program
    {
        public class ArgumentOptions
        {
            [Option( 'i', "inputFile", Required = true, HelpText = "Input file or list of files seperated by ';'" )]
            public string InputFile
            {
                get; set;
            }

            [Option( 'o', "outputFolder", Required = true, HelpText = "Output folder. Default is current dir" )]
            public string OutFolder
            {
                get; set;
            }

            [Option( 'l', "outputLanguage", Required = true, HelpText = "Output language to compile to. Options is [cs, cpp, c, json]" )]
            public string OutputLanguage
            {
                get; set;
            }

        }


        static void LoadGenerators( )
        {
            var s_LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies( ).ToList( );
            var s_LoadedPaths = s_LoadedAssemblies.Select( a => a.Location ).ToArray( );

            var s_ReferencedPaths = Directory.GetFiles( AppDomain.CurrentDomain.BaseDirectory, "*.dll" );

            var s_AssembliesToLoad = s_ReferencedPaths.Where( r => !s_LoadedPaths.Contains( r, StringComparer.InvariantCultureIgnoreCase ) ).ToList( );


            s_AssembliesToLoad.ForEach( path => s_LoadedAssemblies.Add( AppDomain.CurrentDomain.Load( AssemblyName.GetAssemblyName( path ) ) ) );
        }


        static IEnumerable<Type> GetLanguageTypes( ) => AppDomain.CurrentDomain.GetAssemblies( ).Select( x => x.GetExportedTypes( ).Where( y => y.GetCustomAttributes( typeof( LanguageGeneratorAttribute ), true ).Length > 0 ) ).SelectMany( x => x ).ToList( );



        static void Run( ArgumentOptions p_Arguments )
        {


            var s_InputFiles = p_Arguments.InputFile.Split( ';' );

            foreach ( var s_FileName in s_InputFiles )
            {
                var s_File = new FBTFile( s_FileName );
            }

            TypeUnitManager.Instance.ResolveImports( );

            Type s_LanguageType = null;

#region Language find

            foreach ( var s_Language in GetLanguageTypes( ) )
            {
                var s_LanguageGenerator = s_Language.GetCustomAttributes( typeof( LanguageGeneratorAttribute ), true ) as LanguageGeneratorAttribute[];

                foreach ( var s_Lang in s_LanguageGenerator )
                {
                    Console.WriteLine( $"\t{s_Lang.Name}" );

                    if ( s_Lang.Name.ToLower( ) != p_Arguments.OutputLanguage.ToLower( ) )
                        continue;

                    s_LanguageType = s_Language;
                    break;
                }

                if ( s_LanguageType != null )
                    break;
            }

            #endregion


            var s_UnitGenerator = Activator.CreateInstance( s_LanguageType, new object[] { "", p_Arguments.OutFolder } ) as FBTCodeGenerator;

            foreach ( var s_Unit in TypeUnitManager.Instance.Units )
            {
                s_UnitGenerator.GenerateUnit( s_Unit.Value, s_Unit.Key, p_Arguments.OutFolder );
            }
        }




        static void Main( string[] args )
        {
            LoadGenerators( );

            

            Parser.Default.ParseArguments<ArgumentOptions>( args )
                .WithParsed( arguments => Run( arguments ) )
                .WithNotParsed( error =>
            {
                Console.WriteLine( "Supported languages: " );

                foreach ( var s_Language in GetLanguageTypes( ) )
                {
                    var s_LanguageGenerator = s_Language.GetCustomAttributes( typeof( LanguageGeneratorAttribute ), true ) as LanguageGeneratorAttribute[];

                    foreach ( var s_Lang in s_LanguageGenerator )
                    {
                        Console.WriteLine( $"\t{s_Lang.Name}" );
                    }
                }
                Console.WriteLine( );
            } );



            

        }
    }
}
