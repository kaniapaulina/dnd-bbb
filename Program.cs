using DnD_BBB.Classes;
using DnD_BBB.Core;
using DnD_BBB.Races;
using System.Runtime.InteropServices;

namespace DnD_BBB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UnitRace r1 = new Human();
            UnitClass c1 = new Bard();
            Character char1 = new Character("Paulina", c1, r1);
            char1.LevelUp();
            char1.LevelUp();


            Console.WriteLine(char1);

            UnitRace r2 = new Dragonborn();
            UnitClass c2 = new Sorcerer();
            Character char2 = new Character("Wiktoria", c2, r2);

            Console.WriteLine(char2);
        }
    }
}
