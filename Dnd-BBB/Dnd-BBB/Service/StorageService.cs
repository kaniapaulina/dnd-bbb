using Dnd_BBB.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dnd_BBB.Service
{
    public class StorageService
    {
        public static void SavePartyJSON(string nazwa, Party p)
        {
            /*
            DataContractJsonSerializer jser =
                new DataContractJsonSerializer(typeof(Party));
            using (var fstream = File.Create(nazwa))
            {
                jser.WriteObject(fstream, p);
            }
            */
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(p, options);
            File.WriteAllText(nazwa, jsonString);
        }

        public static Party ReadPartyJSON(string nazwa)
        {
            /*
            Party odczytany = new Party();
            try
            {
                FileStream fs = new FileStream(nazwa, FileMode.Open);
                DataContractJsonSerializer jsonSr =
                    new DataContractJsonSerializer(typeof(Party));
                fs.Position = 0;
                odczytany = (Party)jsonSr.ReadObject(fs);
                fs.Close();
                return odczytany;

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Nie znaleziono pliku!");
            }
            return odczytany;
            */

            if (!File.Exists(nazwa)) return null;
            string jsonString = File.ReadAllText(nazwa);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            return JsonSerializer.Deserialize<Party>(jsonString);
        }
    }
}
