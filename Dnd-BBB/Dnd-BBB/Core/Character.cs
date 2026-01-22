using Dnd_BBB.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dnd_BBB.Core
{
    public class Character:Unit, IEquatable<Character>, IComparable<Character>, ICloneable
    {
        #region EF
        public int CharacterId { get; set; }
        public int PartyId { get; set; }
        public virtual Party Party { get; set; }

        #endregion EF

        private string name;
        private int gold;
        //private int level;
        
        public int MaxSpellCount
        {
            get
            {
                return 3 + Level;
            }
        }
        //Spella Proficinties i Equipment sa przechowywane jako json co umizliwia zapis do bazy danych, same obiekty nie sa mapowane
        [NotMapped]
        private List<string> _spells;
        [NotMapped]
        public List<string> Spells
        {
            get => _spells ??= (string.IsNullOrEmpty(SpellsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(SpellsJson));
            set
            {
                _spells = value ?? new List<string>();
                SpellsJson = JsonSerializer.Serialize(_spells);
            }
        }

        public string SpellsJson { get; set; } = "[]";

        [NotMapped]
        private List<string> _proficiencies;
        [NotMapped]
        public List<string> Proficiencies
        {
            get => _proficiencies ??= (string.IsNullOrEmpty(ProficienciesJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(ProficienciesJson));
            set
            {
                _proficiencies = value ?? new List<string>();
                ProficienciesJson = JsonSerializer.Serialize(_proficiencies);
            }
        }
        public string ProficienciesJson { get; set; } = "[]";

        [NotMapped]
        private List<string> _equipment;
        [NotMapped]
        public List<string> Equipment
        {
            get => _equipment ??= (string.IsNullOrEmpty(EquipmentJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(EquipmentJson));
            set
            {
                _equipment = value ?? new List<string>();
                EquipmentJson = JsonSerializer.Serialize(_equipment);
            }
        }
        public string EquipmentJson { get; set; } = "[]";

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

        /*public int Level { get => level; 
            set
            {
                if(value < 0 || value > 20)
                {
                    throw new Exception("Impossible level achieved (how??");
                }
                level = value;
            }
        }*/

        public Character():base() { }

        public Character(string name, UnitClass uclass, UnitRace urace):base()
        {
            this.Name = name;
            this.UnitClass = uclass;
            this.UnitRace = urace;
            this.Gold = 0;
            this.UnitClass.AssignStats(this);
            this.UnitClass.AssignStarterPack(this);
        }

        public void AddSpell(string spell)
        {
            if(UnitClass.Spell == false)
            {
                throw new Exception($"{UnitClass.ClassName} cant use magic");
            }
            if(Spells.Count() >=  MaxSpellCount)
            {
                throw new Exception($"Your max spell count is: {MaxSpellCount}");
            }
            Spells.Add(spell);
            SpellsJson = JsonSerializer.Serialize(Spells);
        }

        public void AddProficiencies(string p1, string p2, string p3)
        {
            Proficiencies.Add(p1);
            Proficiencies.Add(p2);
            Proficiencies.Add(p3);
            ProficienciesJson = JsonSerializer.Serialize(Proficiencies);
        }

        //public int RollProficiency(string p)
        //{
        //    if(!Proficiencies.Contains(p))
        //    {
        //        throw new Exception("You dont have that Proficiency");
        //    }
        //    Random rand = new Random();
        //    int roll = rand.Next(1,21) + ProficiencyBonus;
        //    return roll;
        //}

        /*public virtual void LevelUp()
        {
            Random rand = new Random();
            int maxroll = UnitClass.HitDie;
            int roll = rand.Next(1, maxroll + 1);

            int hpGain = roll + UnitClass.CalcConstitution(Cons);
            Hp += hpGain;
            Level += 1;
        }*/

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Name: {name} (level: {Level})\n");
            sb.Append($"Race: {UnitRace.RaceName}\n");
            sb.Append($"Class: {UnitClass.ClassName}\n");
            sb.AppendLine("=== Proficiencies ===");
            foreach (var p in Proficiencies)
            {
                sb.AppendLine($"{p}");
            }
            sb.Append("=== Stats === \n");
            sb.AppendLine($"HP: {Hp} and AC: {Ac}");
            sb.AppendLine($"Constitution: {Cons}");
            sb.AppendLine($"Dexterity: {Dext}");
            sb.AppendLine($"Inteligence: {Intel}");
            sb.AppendLine($"Strength: {Str}");
            sb.AppendLine($"Wisdom:  {Wis}");
            sb.AppendLine($"Charm: {Charm}");
            sb.AppendLine("=== Spells ===");
            foreach(var spell in Spells)
            {
                sb.AppendLine($"{spell}");
            }
            sb.AppendLine("=== Equipment ===");
            foreach (var e in Equipment)
            {
                sb.AppendLine($"{e}");
            }
            return sb.ToString();
        }

        public bool Equals(Character? other)
        {
            //throw new NotImplementedException();
            return Equals(this, other);
        }

        //to idzie najpierw po imieniu potem Hp ale imie should be the same
        public int CompareTo(Character? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            int nameComparison = string.Compare(this.Name ?? string.Empty, other.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            if (nameComparison != 0) return nameComparison;

            return this.Hp.CompareTo(other.Hp);
        }


        protected override void DeathScreen(int damage)
        {
            Console.WriteLine($"{Name} took a lot of damage ({damage}) and has died in an epic battle xoxoxox");
        }

        public object Clone()
        {
            return this.MemberwiseClone();
            //throw new NotImplementedException();
        }
    }
}
