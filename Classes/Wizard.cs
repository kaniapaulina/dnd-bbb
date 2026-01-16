using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Core;

namespace DnD_BBB.Classes
{
    public class Wizard : UnitClass
    {
        public override string ClassName => "Wizard";
        public override int HitDie => 6;
        public override List<StatType> StatPrio => new List<StatType>
        {
            StatType.Intel, StatType.Dex, StatType.Cons, StatType.Charm, StatType.Wis, StatType.Str
        };
    }
}
