using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Bard : UnitClass
    {
        public override string ClassName => "Bard";
        public override int HitDie => 8;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Charm, StatType.Dex,  StatType.Cons, StatType.Intel, StatType.Wis, StatType.Str
        };
    }
}
