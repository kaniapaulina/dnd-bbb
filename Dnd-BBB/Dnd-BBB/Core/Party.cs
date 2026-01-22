using Dnd_BBB.Exceptions;
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
