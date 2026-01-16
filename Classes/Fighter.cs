using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Fighter:UnitClass
    {
        public override string ClassName => "Fighter";
        public override int HitDie => 10;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Str, StatType.Dex, StatType.Cons, StatType.Intel, StatType.Wis, StatType.Charm
        };
    }
}
