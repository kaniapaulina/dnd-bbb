using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Sorcerer : UnitClass
    {
        public override string ClassName => "Sorcerer";
        public override int HitDie => 6;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Charm, StatType.Dex, StatType.Cons, StatType.Intel, StatType.Wis, StatType.Str
        };
    }
}
