using DnD_BBB.Exceptions_and_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Core
{
    public class Party
    {
        private string partyName;
        private List<Character> partyMembers = new List<Character>();

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


        public string PartyName { get => partyName; set => partyName = value; }
        public List<Character> PartyMembers { get => partyMembers; set => partyMembers = value; }

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
    }
}
