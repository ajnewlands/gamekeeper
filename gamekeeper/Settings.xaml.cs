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
            MessageBox.Show("Save on close not yet implemented..");
        }

        private void Click_New(object sender, RoutedEventArgs e)
        {
            var win = new LibrarySettings();
            win.ShowDialog();
        }
        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            if (LibrarySelection.SelectedItem != null)
            {
                var name = ((Library)LibrarySelection.SelectedItem).name;
                var r = MessageBox.Show($"Really delete '{name}'?", "Are you sure?", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ((Model)this.DataContext).Configuration.libraries.Remove((Library)LibrarySelection.SelectedItem);
                }
            }
        }
        private void Click_Edit(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked Edit");
        }

    }
}
