using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;

namespace gamekeeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model _model;
        public Model Model { get
            {
                return this._model;
            }
        }


        public MainWindow()
        {
            _model = new Model();
            DataContext = _model;
            InitializeComponent();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Settings(ref this._model);
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
