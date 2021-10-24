using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gamekeeper
{

    /// <summary>
    /// Configuration of an individual game library, i.e.
    /// what it is called and
    /// where it is found
    /// </summary>
    public class Library
    {
        [JsonProperty(Required = Required.Always)]
        public String path { get; set; }
        [JsonProperty(Required = Required.Always)]
        public String name { get; set; }
    }

    
    /// <summary>
    /// Gamekeeper configuration, essentially a collection of source/destination paths.
    /// </summary>
    public class Configuration
    {
        [JsonProperty(Required = Required.Always)]
        public String gamekeeper_path { get; set; }
        public ObservableCollection<Library> libraries { get; set; }

        public Configuration()
        {
            this.gamekeeper_path = null;
            this.libraries = new ObservableCollection<Library>();
        }

        /// <summary>
        /// Instantiate a Configuration object from provided JSON buffer.
        /// </summary>
        /// <param name="json">String containing the JSON to (attempt to) deserialize into a Configuration</param>
        /// <returns>Deserialized Configuration</returns>
        public static Configuration FromJson(String json)
        {
            var configuration = JsonConvert.DeserializeObject<Configuration>(json);

            return configuration;
        }

        public void WriteToDisk(String path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.File.WriteAllText(path, json);
        }
    }
}
