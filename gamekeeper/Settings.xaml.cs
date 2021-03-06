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
using System.Windows.Shapes;

namespace gamekeeper
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings(ref Model model)
        {
            DataContext = model;
            InitializeComponent();
            LibrarySelection.SelectedItem = model.Configuration.libraries.FirstOrDefault();
            this.Closed += Settings_Closed;
        }

        private void Settings_Closed(object sender, EventArgs e)
        {
            ((Model)this.DataContext).Configuration.WriteToDisk();
        }

        private void Click_New(object sender, RoutedEventArgs e)
        {
            var name = "New Library";
            var path = "C:\\Program Files";

            /// We try to prevent the user duplicating either a library name or source path,
            /// either of which is likely to cause some confusion.
            while(true)
            {
                var win = new LibrarySettings(name, path);
                var r = win.ShowDialog();
                if (r == true)
                {
                     var lib = new Library { name = win.LibraryName, path = win.LibraryPath };
                    try
                    {
                        ((Model)this.DataContext).AddLibrary(lib);
                        break;
                    }
                    catch (InvalidLibraryException ex)
                    {
                        MessageBox.Show(ex.Message, "Invalid library configuration");
                        name = win.LibraryName;
                        path = win.LibraryPath;
                    }
                } else
                {
                    break;
                }   
            }
        }

        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            if (LibrarySelection.SelectedItem != null)
            {
                var name = ((Library)LibrarySelection.SelectedItem).name;
                var r = MessageBox.Show($"Really delete '{name}'?", "Are you sure?", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ((Model)this.DataContext).RemoveLibrary((Library)LibrarySelection.SelectedItem);
                }
            }
        }

        private void Click_Edit(object sender, RoutedEventArgs e)
        {
            var selected = (Library)LibrarySelection.SelectedItem;
            var win = new LibrarySettings(selected.name, selected.path);
            var r = win.ShowDialog();

            if (r == true) {
                ((Library)LibrarySelection.SelectedItem).name = win.LibraryName;
                ((Library)LibrarySelection.SelectedItem).path = win.LibraryPath;
            }
        }
    }
}
