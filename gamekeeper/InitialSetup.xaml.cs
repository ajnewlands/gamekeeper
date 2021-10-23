using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Runtime.CompilerServices;

namespace gamekeeper
{
    /// <summary>
    /// Interaction logic for InitialSetup.xaml
    /// </summary>
    public partial class InitialSetup : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private String _path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gamekeeper");
        public String path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        public InitialSetup()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
            {
                path = dlg.SelectedPath;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
