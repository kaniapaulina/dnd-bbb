using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Monk : UnitClass
    {
        public override string ClassName => "Monk";
        public override int HitDie => 8;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Dex, StatType.Wis, StatType.Cons, StatType.Intel, StatType.Charm, StatType.Str
        };
    }
}
