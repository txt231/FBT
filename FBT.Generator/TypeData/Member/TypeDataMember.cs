using FBT.TypeData.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FBT.TypeData.Member
{
    public class TypeDataMember
        : TypeDataBase
    {
        public string Protection = null;

        public string UnresolvedBaseType = null;
        public TypeDataBase BaseType = null;

        public int Offset;

        public bool IsArray;
        public int ArrayCount = -1;
    }
}
