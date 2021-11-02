using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace gamekeeper
{
    public class InvalidLibraryException : Exception
    {
        public InvalidLibraryException()
        {
        }

        public InvalidLibraryException(string message)
            : base(message)
        {
        }

        public InvalidLibraryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class GameEntry: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _name;
        public String Name { 
            get
            {
                return this._name;
            }     
            set
            {
                this._name = value;
                OnPropertyChanged();
            }
        }
        public String Library { get; set; }

        private String _path;
        public String Path {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
                OnPropertyChanged();
            }
        }
        public String ButtonText
        {
            get
            {
                return "\u2B18 Import  ";

            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Configuration of an individual game library, i.e.
    /// what it is called and
    /// where it is found
    /// </summary>
    public class Library: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _path;
        [JsonProperty(Required = Required.Always)]
        public String path { 
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
                OnPropertyChanged();
            }
        }

        private String _name;
        [JsonProperty(Required = Required.Always)]
        public String name { 
            get {
                return this._name;
            }
            set
            {
                this._name = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Model
    {
        public Configuration Configuration { get; set; }
        public ObservableCollection<GameEntry> Games { get; set; }
        private List<FileSystemWatcher> _watchers;

        public Model()
        {
            _watchers = new List<FileSystemWatcher>();
            Configuration = Configuration.LoadConfiguration();
            Games = new ObservableCollection<GameEntry>();

            foreach (var lib in Configuration.libraries)
            {
                PopulateLibrary(lib);
            }
        }

        private void PopulateLibrary(Library lib)
        {
            var watcher = new FileSystemWatcher(lib.path);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Renamed += Watcher_Renamed;
            watcher.EnableRaisingEvents = true;
            _watchers.Add(watcher);
            foreach (var dir in Directory.GetDirectories(lib.path))
            {
                // TODO better to just calculator name via an accessor
                Games.Add(new GameEntry
                {
                    Name = System.IO.Path.GetFileName(dir),
                    Path = dir,
                    Library = lib.name,
                });
            }

            //  TODO Gamekeeper entries are created by checking the type, i.e. junction rather than directory
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            for (var i = this.Games.Count - 1; i >= 0; i--)
                if (this.Games[i].Path == e.OldFullPath)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        this.Games[i].Name = e.Name;
                        this.Games[i].Path = e.FullPath;
                    }));
                }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                var library_name = this.Configuration.libraries.Where(x => x.path == Path.GetDirectoryName(e.FullPath)).First().name;
                var game = new GameEntry { Library = library_name, Name = e.Name, Path = e.FullPath };
                this.Games.Add(game);
            }));
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            for (var i = this.Games.Count - 1; i >= 0; i--)
                if (this.Games[i].Path == e.FullPath)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        this.Games.RemoveAt(i);
                    }));
                }
        }

        public void RemoveLibrary(Library library)
        {
            // Remove any datagrid entries for the removed library
            for (var i = this.Games.Count - 1; i >= 0; i--)
                if (this.Games[i].Library == library.name)
                    this.Games.RemoveAt(i);
            this.Configuration.libraries.Remove(library);

            // Stop watching for changes in the deconfigured directory
            for (var i = this._watchers.Count - 1; i >= 0; i--)
                if (this._watchers[i].Path == library.path)
                    this._watchers.RemoveAt(i);
        }


        public void AddLibrary(Library library)
        {
            if (this.Configuration.libraries.Where(l => l.name == library.name).Any())
            {
                throw new InvalidLibraryException($"There is already a library called {library.name}");
            }
            if (this.Configuration.libraries.Where(l => l.path == library.path).Any())
            {
                throw new InvalidLibraryException($"There is already a library entry for path {library.path}");
            }
            this.Configuration.libraries.Add(library);
            PopulateLibrary(library);
        }
    }
    
    /// <summary>
    /// Gamekeeper configuration, essentially a collection of source/destination paths.
    /// </summary>
    public class Configuration
    {
        private const String configuration_path = "./configuration.json";
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

        public void WriteToDisk(String path = configuration_path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.File.WriteAllText(path, json);
        }
        public static Configuration LoadConfiguration()
        {
            Configuration configuration = null;
            Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            try
            {
                String json = File.ReadAllText(configuration_path);
                if (json.Length==0)
                {
                    // Hack; Newtonsoft.JSON doesn't handle an empty string properly.
                    throw new Newtonsoft.Json.JsonException("Configuration file is empty");
                }
                configuration = Configuration.FromJson(json);
            } 
            // Configuration not found -> do first time setup
            catch (FileNotFoundException)
            {
                configuration = FirstTimeSetup();
            }
            // Configuration is corrupt -> offer to generate default
            catch (Newtonsoft.Json.JsonException e)
            {
                var result = MessageBox.Show($@" 
Reason: {e.Message}

Click 'cancel' to quit now (allowing you to edit and attempt to fix) or 'OK' to generate a default configuration, replacing the existing file.",
                    "Corrupt configuration.json", 
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK) {
                    configuration = FirstTimeSetup();
                } else {
                    Environment.Exit(1);
                }
            }
            // Configuration cannot be read (access denied) -> abort
            // Directory cannot be created (access denied) -> abort
            // Something we did not anticipate -> abort
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Could not complete initial setup. Aborting", MessageBoxButton.OK);
                Environment.Exit(1);
            }

            return configuration;
        }

        private static Configuration FirstTimeSetup()
        {
            Configuration configuration;
            var init = new InitialSetup();
            if (init.ShowDialog().GetValueOrDefault() == false)
            {
                Environment.Exit(0);
            }

            Directory.CreateDirectory(init.path);
            // TODO scan for easily identified libraries, e.g Steam (registry key)

            configuration = new Configuration { gamekeeper_path = init.path, libraries = new ObservableCollection<Library>() };

            configuration.WriteToDisk(configuration_path);
            return configuration;
        }
    }
}
