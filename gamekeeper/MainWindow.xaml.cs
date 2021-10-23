using System;
using System.Collections.Generic;
using System.Linq;
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

            try
            {
                String json = System.IO.File.ReadAllText(configuration_path);
                configuration = Configuration.FromJson(json);
            } 
            // Configuration not found -> do first time setup
            catch (System.IO.FileNotFoundException)
            {
                var init = new InitialSetup();
                if (init.ShowDialog().GetValueOrDefault() == false)
                {
                    Environment.Exit(0);
                }

                // TODO ensure this possible exception is handled
                System.IO.Directory.CreateDirectory(init.path);
                configuration = new Configuration { gamekeeper_path = init.path, libraries = new List<Library>() };
            }
            // Configuration is corrupt -> offer to generate default
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
