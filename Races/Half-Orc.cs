using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Races
{
    public class Half_Orc:UnitRace
    {
        public override string RaceName => "Half-Orc";
        public override void ApplyBonus(Unit unit)
        {
            unit.Hp += 1;
            unit.Ac += 1;
            unit.Str += 2;
        }
    }
}
