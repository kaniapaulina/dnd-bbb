using Dnd_BBB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dnd_BBB.Service
{
    public static class PartyRepository
    {
        public static List<Party> GetAllParties()
        {
            using var db = new PartyDbContext();
            return db.Parties.Include("PartyMembers").ToList();
        }

        public static Party? GetPartyById(int id)
        {
            using var db = new PartyDbContext();
            return db.Parties.Include("PartyMembers").FirstOrDefault(p => p.PartyId == id);
        }

        public static List<Character> GetAllCharacters()
        {
            using var db = new PartyDbContext();
            return db.Characters.ToList();
        }

        public static async Task<List<Character>> GetAllCharactersAsync()
        {
            using var db = new PartyDbContext();
            return await db.Characters.ToListAsync();
        }

        public static void SaveParty(Party party)
        {
            using var db = new PartyDbContext();

            // JSON i powi¹zania
            foreach (var c in party.PartyMembers ?? new List<Character>())
            {
                c.SpellsJson = JsonSerializer.Serialize(c.Spells ?? new List<string>());
                c.ProficienciesJson = JsonSerializer.Serialize(c.Proficiencies ?? new List<string>());
                c.EquipmentJson = JsonSerializer.Serialize(c.Equipment ?? new List<string>());
                c.Party = party;
            }

            if (party.PartyId == 0)
            {
                db.Parties.Add(party);
            }
            else
            {
                db.Parties.Attach(party);
                db.Entry(party).State = EntityState.Modified;

                // synchronizuj cz³onków
                foreach (var c in party.PartyMembers)
                {
                    if (c.CharacterId == 0)
                    {
                        db.Characters.Add(c);
                    }
                    else
                    {
                        db.Characters.Attach(c);
                        db.Entry(c).State = EntityState.Modified;
                    }
                }
            }

            db.SaveChanges();
        }

        public static void SaveCharacter(Character c)
        {
            using var db = new PartyDbContext();

            c.SpellsJson = JsonSerializer.Serialize(c.Spells ?? new List<string>());
            c.ProficienciesJson = JsonSerializer.Serialize(c.Proficiencies ?? new List<string>());
            c.EquipmentJson = JsonSerializer.Serialize(c.Equipment ?? new List<string>());

            if (c.CharacterId == 0)
            {
                db.Characters.Add(c);
            }
            else
            {
                db.Characters.Attach(c);
                db.Entry(c).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        // Asynchroniczna wersja zapisu — u¿yj z UI aby nie blokowaæ w¹tku interfejsu
        public static async Task SaveCharacterAsync(Character c)
        {
            using var db = new PartyDbContext();

            c.SpellsJson = JsonSerializer.Serialize(c.Spells ?? new List<string>());
            c.ProficienciesJson = JsonSerializer.Serialize(c.Proficiencies ?? new List<string>());
            c.EquipmentJson = JsonSerializer.Serialize(c.Equipment ?? new List<string>());

            if (c.CharacterId == 0)
            {
                db.Characters.Add(c);
            }
            else
            {
                db.Characters.Attach(c);
                db.Entry(c).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();
        }

        // Sprawdza, czy po³¹czenie z baz¹ dzia³a (synchron.)
        public static bool IsDatabaseAvailable()
        {
            try
            {
                using var db = new PartyDbContext();
                var conn = db.Database.Connection;
                if (conn.State != ConnectionState.Open) conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Sprawdza, czy po³¹czenie z baz¹ dzia³a (asynchronicznie)
        public static async Task<bool> IsDatabaseAvailableAsync()
        {
            try
            {
                using var db = new PartyDbContext();
                var conn = db.Database.Connection;
                if (conn.State != ConnectionState.Open) await conn.OpenAsync();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}