using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Core
{
    public abstract class UnitRace
    {
        public abstract string RaceName { get; }
        public virtual void ApplyBonus(Unit unit) { }
    }
}
