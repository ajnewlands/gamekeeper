using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;

namespace gamekeeper
{
    public class GameEntry
    {
        public String Name { get; set; }
        public String Library { get; set; }
        public String Path { get; set; }
        public String ButtonText
        {
            get
            {
                return "\u2B18 Import  ";

            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String configuration_path = "./configuration.json";
        public ObservableCollection<GameEntry> games { get; set; }
        private Configuration _configuration;

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

            configuration = new Configuration { gamekeeper_path = init.path, libraries = new ObservableCollection<Library>() };

            configuration.WriteToDisk(configuration_path);
            return configuration;
        }

        public MainWindow()
        {
            _configuration = LoadConfiguration();
            games = new ObservableCollection<GameEntry>();
            DataContext = this;
            InitializeComponent();

            foreach (var lib in _configuration.libraries)
            {
                // TODO read files/junction points and add to dataview (asynchronously?)
                foreach (var dir in Directory.GetDirectories(lib.path))
                {
                    // TODO better to just calculator name via an accessor
                    games.Add(new GameEntry {
                        Name = System.IO.Path.GetFileName(dir),
                        Path = dir,
                        Library = lib.name, 
                    });
                }
            }
            // Do the same with the gamekeeper library
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Settings(ref this._configuration);
            sw.ShowDialog();
        }

        private void ExportImportClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var ge = (GameEntry)button.DataContext;
            MessageBox.Show($"Import/Export for {ge.Name}");
        }

    }
}
