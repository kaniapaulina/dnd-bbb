using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnD_BBB.Exceptions;
namespace DnD_BBB.Core
{

    public enum StatType { Str, Dex, Intel, Wis, Charm, Cons}

    public abstract class Unit
    {
        private int hp; //Hitpoints
        private int ac; //Armor Class
        private int cons; //Constitution

        private int dext; //Dexterity
        private int str; //Strength
        private int wis; //Wisdom 
        private int intel; //Intelligence
        private int charm; //Charm

        private UnitRace unitrace;
        private UnitClass unitclass;

        /// <summary>
        /// Full Properties for each base stat, throwing InvalidStatValueException if stat reaches impossible value
        /// </summary>
        public int Hp { get => hp; 
            set 
            { 
                if( value > 640)
                {
                    throw new InvalidStatValueException("Impossible Hitpoints Value");
                }
                hp = value;
            } 
        }
        public int Ac
        {
            get => ac;
            set
            {
                if (value > 50)
                {
                    throw new InvalidStatValueException("Impossible Armor Class Value");
                }
                ac = value;
            }
        }

        public int Cons
        {
            get => cons;
            set
            {
                if (value > 50)
                {
                    throw new InvalidStatValueException("Impossible Constitution Value");
                }
                cons = value;
            }
        }

        public int Dext { get => dext;
            set
            {
                if (value < 0 || value > 30)
                {
                    throw new InvalidStatValueException("Impossible Dexterity Value");
                }
                dext = value;
            }
        }
        public int Str { get => str;
            set
            {
                if (value < 0 || value > 30)
                {
                    throw new InvalidStatValueException("Impossible Strength Value");
                }
                str = value;
            }
        }
        public int Wis { get => wis;
            set
            {
                if (value < 0 || value > 30)
                {
                    throw new InvalidStatValueException("Impossible Wisdom Value");
                }
                wis = value;
            }
        }
        public int Intel { get => intel;
            set
            {
                if (value < 0 || value > 30)
                {
                    throw new InvalidStatValueException("Impossible Intelligence Value");
                }
                intel = value;
            }
        }
        public int Charm { get => charm;
            set
            {
                if (value < 0 || value > 30)
                {
                    throw new InvalidStatValueException("Impossible Charm Value");
                }
                charm = value;
            }
        }


        public UnitRace UnitRace { get; set; }
        public UnitClass UnitClass { get; set; }

        protected Unit()
        {
            this.hp = 0;
            this.ac = 10;
            this.cons = 0;

            this.dext = 0;
            this.str = 0;
            this.wis = 0;
            this.intel = 0;
            this.charm = 0;
        }
    }
}
