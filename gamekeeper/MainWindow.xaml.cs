using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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
            this._model.Configuration.libraries.CollectionChanged += OnLibraryChanged;
            DataContext = _model;
            InitializeComponent();

        }

        private void OnLibraryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                for (var i = this._model.Games.Count - 1; i >= 0; i--)
                {
                    foreach (Library lib in e.OldItems)
                    {
                        if (this._model.Games[i].Library == lib.name)
                        {
                            this._model.Games.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                MessageBox.Show("Add not implemented");
            }
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
