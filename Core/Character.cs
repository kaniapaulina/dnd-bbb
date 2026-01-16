using DnD_BBB.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Core
{

    public class Character:Unit
    {
        private string name;
        private int gold;
        private int level;
        public string Name { get => name;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Name cannot be emtpy");
                }
                name = value;
            }
        }
        public int Gold
        {
            get => gold;
            set
            {
                if (value > 1000)
                {
                    throw new InvalidStatValueException("Impossible Gold Value");
                }
                gold = value;
            }
        }

        public int Level { get => level; 
            set
            {
                if(value < 0 || value > 20)
                {
                    throw new Exception("Impossible level achieved (how??");
                }
                level = value;
            }
        }

        public Character(string name, UnitClass uclass, UnitRace urace):base()
        {
            this.Name = name;
            this.UnitClass = uclass;
            this.UnitRace = urace;
            this.Gold = 0;
            this.Level = 1;
            this.UnitClass.AssignStats(this);
        }

        public virtual void LevelUp()
        {
            Random rand = new Random();
            int maxroll = UnitClass.HitDie;
            int roll = rand.Next(1, maxroll + 1);

            int hpGain = roll + UnitClass.CalcConstitution(Cons);
            Hp += hpGain;
            Level += 1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Name: {name} (level: {level})\n");
            sb.Append($"Race: {UnitRace.RaceName}\n");
            sb.Append($"Class: {UnitClass.ClassName}\n");
            sb.Append("=== Stats === \n");
            sb.AppendLine($"HP: {Hp} and AC: {Ac}");
            sb.AppendLine($"Constitution: {Cons}");
            sb.AppendLine($"Dexterity: {Dext}");
            sb.AppendLine($"Inteligence: {Intel}");
            sb.AppendLine($"Strength: {Str}");
            sb.AppendLine($"Wisdom:  {Wis}");
            sb.AppendLine($"Charm: {Charm}");
            return sb.ToString();
        }
    }
}
