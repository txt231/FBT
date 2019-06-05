using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.Generator
{
    [AttributeUsage( AttributeTargets.Class )]
    public class LanguageGeneratorAttribute : Attribute
    {
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Default constructor that takes an alignment
        /// </summary>
        /// <param name="Alignment">Default alignment: 0</param>
        public LanguageGeneratorAttribute( string p_CateogryName )
        {
            this.Name = p_CateogryName;
        }
    }
}
