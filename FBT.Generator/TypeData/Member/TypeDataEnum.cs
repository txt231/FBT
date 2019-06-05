using FBT.TypeData.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData.Member
{
    public class TypeDataEnum
        : TypeDataBase
    {
        public Dictionary<string, int> EnumDict = new Dictionary<string, int>( );
    }
}
