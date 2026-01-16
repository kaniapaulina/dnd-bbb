using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Ranger : UnitClass
    {
        public override string ClassName => "Ranger";
        public override int HitDie => 10;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Dex, StatType.Cons, StatType.Wis, StatType.Intel, StatType.Str, StatType.Charm
        };
    }
}
