using DnD_BBB.Classes;
using DnD_BBB.Core;
using DnD_BBB.Races;
using DnD_BBB.Service;
using System.Runtime.InteropServices;

namespace DnD_BBB
{
    public enum StatType { Str, Dex, Intel, Wis, Charm, Cons }
    internal class Program
    {
        static void Main(string[] args)
        {
            UnitRace r1 = new Human();
            UnitClass c1 = new Bard();
            Character char1 = new Character("Paulina", c1, r1);
            char1.LevelUp();
            char1.LevelUp();


            //Console.WriteLine(char1);

            UnitRace r2 = new Dragonborn();
            UnitClass c2 = new Sorcerer();
            Character char2 = new Character("Wiktoria", c2, r2);
            char2.AddSpell("Daj Dupe");
            char2.AddSpell("Abrakababra");
            char2.AddProficiencies("Łowienie ryb", "Gotowanie dań mięsnych", "Inwestycja miejsc zbrodni");


            //Console.WriteLine(char2);

            UnitRace r3 = new Gnome();
            UnitClass c3 = new Ranger();
            Character char3 = new Character("Nates", c3, r3);

            Party p1 = new Party("Nerdy nerdują");
            p1.AddMember(char1);
            p1.AddMember(char2);
            p1.AddMember(char3);

            Console.WriteLine(p1);

            char1.TakeDamage(40);

            //p1.SortByHp();
            //Console.WriteLine(p1);

            // TEST NA JSONA
            Console.WriteLine("======== TEST JSON");
            StorageService.SavePartyJSON("party.json", p1);

            Party odczyt = StorageService.ReadPartyJSON("party.json");
            Console.WriteLine(odczyt);
        }
    }
}
