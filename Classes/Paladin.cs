using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Paladin : UnitClass
    {
        public override string ClassName => "Paladin";
        public override int HitDie => 10;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Str, StatType.Dex, StatType.Cons, StatType.Charm, StatType.Wis, StatType.Intel
        };
    }
}
