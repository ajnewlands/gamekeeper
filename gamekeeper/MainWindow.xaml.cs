using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace gamekeeper
{
    public class GameEntry
    {
        public String Name { get; set; }
        public String Library { get; set; }
        public String Path { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private const String configuration_path = "./configuration.json";
        private List<GameEntry> _games = new List<GameEntry>();
        public List<GameEntry>  games {
            get {
                return _games;
            }
            set {
                _games = value;
                OnPropertyChanged();
            }
        }

        private Configuration LoadConfiguration()
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

            configuration = new Configuration { gamekeeper_path = init.path, libraries = new List<Library>() };

            configuration.WriteToDisk(configuration_path);
            return configuration;
        }

        public MainWindow()
        {
            var configuration = LoadConfiguration();
            DataContext = this;
            InitializeComponent();

            var mygames = new List<GameEntry>();
            foreach (var lib in configuration.libraries)
            {
                // TODO read files/junction points and add to dataview (asynchronously?)
                foreach (var dir in Directory.GetDirectories(lib.path))
                {
                    // TODO better to just calculator name via an accessor
                    mygames.Add(new GameEntry {
                        Name = System.IO.Path.GetFileName(dir),
                        Path = dir,
                        Library = lib.name, 
                    }); ;
                }
            }
            games = mygames;
            // Do the same with the gamekeeper library
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
