using DnD_BBB.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Core
{
    public abstract class UnitClass
    {
        public abstract string ClassName { get; }

        public abstract List<StatType> StatPrio { get; }

        public abstract int HitDie { get; }
        public virtual int BaseHp => HitDie;

        public virtual void AssignStats(Character c) 
        {
            c.UnitRace.ApplyBonus(c);
            StatService service = new StatService();
            service.AssignWeightedStats(c);
            
            c.Hp += BaseHp;
            c.Hp += CalcConstitution(c.Cons);

            c.Ac += CalcDext(c.Dext);
        }

        public int CalcConstitution(int stat)
        {
            return (int)Math.Floor((stat - 10) / 2.0);
        }

        public int CalcDext(int stat)
        {
            return (int)Math.Floor((stat - 10) / 2.0);
        }
    }

}
