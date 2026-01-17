using DnD_BBB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Exceptions_and_Interfaces
{
    public class StrComparer : IComparer<Character>
    {
        public int Compare(Character? x, Character? y) => x.Str.CompareTo(y.Str);
        /*
        {
            throw new NotImplementedException();
        }
        */
    }
}
