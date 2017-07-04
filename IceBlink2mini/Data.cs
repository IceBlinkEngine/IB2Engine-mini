using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceBlink2mini
{
    public class Data
    {
        public List<Item> dataItemsList = new List<Item>();
        public List<Creature> dataCreaturesList = new List<Creature>();
        public List<Prop> dataPropsList = new List<Prop>();
        public List<PlayerClass> dataPlayerClassList = new List<PlayerClass>();
        public List<Race> dataRacesList = new List<Race>();
        public List<Spell> dataSpellsList = new List<Spell>();
        public List<Trait> dataTraitsList = new List<Trait>();
        public List<Effect> dataEffectsList = new List<Effect>();

        public Data()
        {

        }
        public void saveDataFile(string filename, bool indent)
        {
            string json = "";
            if (indent)
            {
                json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);
            }
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(json.ToString());
            }
        }
        public Data loadDataFile(string filename)
        {
            Data toReturn = null;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                toReturn = (Data)serializer.Deserialize(file, typeof(Data));
            }
            return toReturn;
        }
    }
}
