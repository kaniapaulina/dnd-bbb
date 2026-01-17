using DnD_BBB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Exceptions_and_Interfaces
{
    public class HpComparer : IComparer<Character>
    {
        public int Compare(Character? x, Character? y) => x.Hp.CompareTo(y.Hp);
        /*
        {
            throw new NotImplementedException();
        }
        */
    }
}
