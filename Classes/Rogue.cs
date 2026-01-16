using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Rogue : UnitClass
    {
        public override string ClassName => "Rogue";
        public override int HitDie => 8;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Dex, StatType.Intel, StatType.Str, StatType.Cons, StatType.Wis, StatType.Charm
        };
    }
}
