using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Exceptions_and_Interfaces
{
    public class InvalidStatValueException : Exception
    {
        public InvalidStatValueException(string message) : base(message)
        {
            throw new InvalidStatValueException(message);
        }
    }
}
