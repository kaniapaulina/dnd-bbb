using Dnd_BBB.Classes;
using Dnd_BBB.Exceptions;
using Dnd_BBB.Races;
using Dnd_BBB.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dnd_BBB.Core
{
    public class Party: ICloneable
    {
        #region EF
        [Key]
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public virtual List<Character> PartyMembers { get; set; } = new();

        public void SaveToDb(Party party)
        {
            using (var db = new PartyDbContext())
            {
                foreach (var c in party.PartyMembers)
                {
                    c.SpellsJson = JsonSerializer.Serialize(c.Spells ?? new List<string>());
                    c.ProficienciesJson = JsonSerializer.Serialize(c.Proficiencies ?? new List<string>());
                    c.EquipmentJson = JsonSerializer.Serialize(c.Equipment ?? new List<string>());
                    c.Party = party;
                }
                db.Parties.Add(party);
                db.SaveChanges();
            }
        }

        public static List<Party> ReadFromDb()
        {
            using (var db = new PartyDbContext())
            {
                var parties = db.Parties.Include("PartyMembers").ToList();

                foreach (var p in parties)
                {
                    foreach (var c in p.PartyMembers)
                    {
                        // REKONSTRUKCJA KLASY I RASY :((
                        c.UnitClass = ResolveClass(c.UnitClassName);
                        c.UnitRace = ResolveRace(c.UnitRaceName);

                        _ = c.Spells;
                        _ = c.Proficiencies;
                        _ = c.Equipment;
                    }
                }
                return parties;
            }
        }

        private static UnitClass ResolveClass(string name) => name switch
        {
            "Bard" => new Bard(),
            "Wizard" => new Wizard(),
            "Fighter" => new Fighter(),
            "Barbarian" => new Barbarian(),
            "Cleric" => new Cleric(),
            "Druid" => new Druid(),
            "Monk" => new Monk(),
            "Paladin" => new Paladin(),
            "Ranger" => new Ranger(),
            "Rogue" => new Rogue(),
            "Sorcerer" => new Sorcerer(),
            "Warlock" => new Warlock(),
            _ => null
        };

        private static UnitRace ResolveRace(string name) => name switch
        {
            "Human" => new Human(),
            "Elf" => new Elf(),
            "Dwarf" => new Dwarf(),
            "Dragonborn" => new Dragonborn(),
            "Gnome" => new Gnome(),
            "Halfling" => new Halfling(),
            "Half-Orc" => new Half_Orc(),
            "Half-Elf" => new Half_Elf(),
            "Tiefling" => new Tiefling(),
            _ => null
        };

        public static List<Character> ReadAllCharactersFromDb()
        {
            var parties = ReadFromDb();
            return parties
                .SelectMany(p => p.PartyMembers ?? Enumerable.Empty<Character>())
                .GroupBy(c => c.CharacterId)
                .Select(g => g.First())
                .ToList();
        }

        #endregion EF

        public Party() { }
        public Party(string nazwa)
        {
            PartyName = nazwa;
        }

        public void AddMember(Character c)
        {
            //if(c.Equals(PartyMembers.Any()))
            if(PartyMembers.Any(mem => mem.Name.Equals(c.Name)))
            {
                throw new Exception("This member is already in your Party");
            }
            PartyMembers.Add(c);
        }

        public bool ExistMember(string mName)
        {
            return PartyMembers.Exists(m => m.Name == mName);
        }

        public void DeleteMember(String dName)
        {
            if(ExistMember(dName))
            {
                PartyMembers.Remove(PartyMembers.Find(m => m.Name.Equals(dName)));
            }
        }

        public List<Character> FindClass(UnitClass uc)
        {
            List<Character> mlist = new List<Character>();
            mlist = PartyMembers.FindAll(m => m.UnitClass.Equals(uc));
            return mlist;
        }

        public List<Character> FindRace(UnitRace ur)
        {
            List<Character> mlist = new List<Character>();
            mlist = PartyMembers.FindAll(m => m.UnitRace.Equals(ur));
            return mlist;
        }

        public void SortByName() => PartyMembers.Sort();

        // Ponizej sorty, sortuja rosnaco, kinda nieintuicyjne ale nie bd tego zmieniac
        public void SortByHp() => PartyMembers.Sort(new HpComparer());
        public void SortByStr() => PartyMembers.Sort(new StrComparer());
        public void SortByDext() => PartyMembers.Sort(new DextComparer());


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {PartyName} with {PartyMembers.Count()} member(s)");
            foreach(var member in PartyMembers)
            {
                sb.AppendLine($"{member.ToString()}");
            }

            return sb.ToString();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
            //throw new NotImplementedException();
        }
        public Party DeepCopy()
        {
            Party kopia = (Party)this.Clone();
            kopia.PartyName = (string)this.PartyName.Clone();
            kopia.PartyMembers = new List<Character>(PartyMembers.Select(
                x => (Character)x.Clone()
                ));
            return kopia;
        }
    }
}
