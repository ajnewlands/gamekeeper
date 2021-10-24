using System;
using System.Collections.Generic;
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
        public String path;
        [JsonProperty(Required = Required.Always)]
        public String name;
    }

    
    /// <summary>
    /// Gamekeeper configuration, essentially a collection of source/destination paths.
    /// </summary>
    public class Configuration
    {
        [JsonProperty(Required = Required.Always)]
        public String gamekeeper_path { get; set; }
        public List<Library> libraries;
        [JsonProperty(Required = Required.Always)]
        public int foo { get; set; }

        public Configuration()
        {
            this.gamekeeper_path = null;
            this.libraries = new List<Library>();
            this.foo = 42;
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
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(path, json);
        }
    }
}
