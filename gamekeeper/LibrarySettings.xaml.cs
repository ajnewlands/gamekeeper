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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gamekeeper
{
    /// <summary>
    /// Interaction logic for LibrarySettings.xaml
    /// </summary>
    public partial class LibrarySettings : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private String _LibraryPath;
        public String LibraryPath
        {
            get
            {
                return this._LibraryPath;
            }
            set
            {
                this._LibraryPath = value;
                OnPropertyChanged();
            }
        }

        private String _LibraryName;
        public String LibraryName
        {
            get
            {
                return this._LibraryName;
            }
            set
            {
                this._LibraryName = value;
                OnPropertyChanged();
            }
        }
        public LibrarySettings(String LibraryName, String LibraryPath)
        {
            _LibraryPath = LibraryPath;
            _LibraryName = LibraryName;
            DataContext = this;
            InitializeComponent();
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = this.LibraryPath;
            if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                this.LibraryPath = dlg.SelectedPath;
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Click_OK (object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
