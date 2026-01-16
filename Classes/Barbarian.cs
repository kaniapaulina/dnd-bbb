using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Barbarian : UnitClass
    {
        public override string ClassName => "Barbarian";
        public override int HitDie => 12;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Str, StatType.Cons, StatType.Dex, StatType.Intel, StatType.Charm, StatType.Wis
        };

    }
}
