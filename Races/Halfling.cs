using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Races
{
    public class Halfling:UnitRace
    {
        public override string RaceName => "Halfling";
        public override void ApplyBonus(Unit unit)
        {
            unit.Dext += 2;
        }
    }
}
