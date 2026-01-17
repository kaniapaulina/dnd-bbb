using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Exceptions_and_Interfaces
{
    public class DextComparer : IComparer<Character>
    {
        public int Compare(Character? x, Character? y) => x.Dext.CompareTo(y.Dext);
        /*
        {
            throw new NotImplementedException();
        }
        */
    }
}
