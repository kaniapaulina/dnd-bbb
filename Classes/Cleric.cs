using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Cleric : UnitClass
    {
        public override string ClassName => "Cleric";
        public override int HitDie => 8;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Wis, StatType.Cons, StatType.Dex, StatType.Str, StatType.Intel, StatType.Charm
        };
    }
}
