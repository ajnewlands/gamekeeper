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
using System.IO;

namespace gamekeeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String configuration_path = "./configuration.json";
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
                MessageBox.Show($"Path is {configuration.gamekeeper_path}");
            } 
            // Configuration not found -> do first time setup
            catch (FileNotFoundException)
            {
                configuration = FirstTimeSetup();
            }
            // Configuration is corrupt -> offer to generate default
            catch (Newtonsoft.Json.JsonException e)
            {
                var result = MessageBox.Show("Configuration is corrupt. Click 'cancel' to quit now (allowing you to edit and attempt to fix) or 'OK' to generate a default configuration, replacing the existing file.",
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
                MessageBox.Show(e.Message, "Could not complete initial setups", MessageBoxButton.OK);
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

            // TODO ensure this possible exception is handled
            Directory.CreateDirectory(init.path);
            configuration = new Configuration { gamekeeper_path = init.path, libraries = new List<Library>() };
            return configuration;
        }

        public MainWindow()
        {
            var configuration = LoadConfiguration();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
