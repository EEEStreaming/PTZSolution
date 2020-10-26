using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Xaml.Behaviors;
using PTZPadController.PresentationLayer;

namespace PTZPadController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private List<PresentationLayer.PresetIcon> _IconList;
        PresetsToolBox _winPresetsToolBox;

        public MainWindow()
        {
            InitializeComponent();
            App.Win = this;

            //var currentDir = System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location);
            //currentDir = System.IO.Path.Combine(currentDir, "PresetIcons");

            //string[] filePaths = Directory.GetFiles(currentDir, "*.png");
            //PresentationLayer.PresetIcon icon;
            //_IconList = new List<PresentationLayer.PresetIcon>();
            //foreach (var file in filePaths)
            //{
            //    icon = new PresentationLayer.PresetIcon();
            //    icon.FullPath = file;
            //    icon.Key = System.IO.Path.GetFileNameWithoutExtension(file);
            //    _IconList.Add(icon);
            //}

            //listviewTest.ItemsSource = _IconList;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_winPresetsToolBox == null)
                _winPresetsToolBox = new PresetsToolBox();

            _winPresetsToolBox.Show();
        }
    }
}
