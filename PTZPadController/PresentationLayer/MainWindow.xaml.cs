using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
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
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xaml.Behaviors;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
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
        private IDisposable winMoveSub;
        private IDisposable winResizeSub;

        public MainWindow()
        {
            InitializeComponent();
            App.Win = this;

        }

        private void SaveWindowsPosition()
        {
            var ptzManager = SimpleIoc.Default.GetInstance<IPTZManager>();
            WindowPositionModel winPos = new WindowPositionModel
            {
                Height = this.Height,
                Left = this.Left,
                Top = this.Top,
                Width = this.Width
            };
            ptzManager.SaveWindowPosition(this, winPos);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            winMoveSub.Dispose();
            winMoveSub = null;
            winResizeSub.Dispose();
            winResizeSub = null;
            App.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_winPresetsToolBox == null)
               _winPresetsToolBox = new PresetsToolBox();

            if (_winPresetsToolBox.IsVisible)
                _winPresetsToolBox.Hide();
            else
                _winPresetsToolBox.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //load windows Position
            var ptzManager = SimpleIoc.Default.GetInstance<IPTZManager>();
            var winPos = ptzManager.LoadWindowPosition(this);
            if (winPos != null)
            {
                Top = winPos.Top;
                Left = winPos.Left;
                Height = winPos.Height;
                Width = winPos.Width;
            }

            //We save the position when we have finished to move or to resize the window after 2 sec.
            var obsWinMove = Observable.FromEventPattern<EventHandler, EventArgs>(h => LocationChanged += h, h => LocationChanged -= h);
            winMoveSub = obsWinMove.Throttle(TimeSpan.FromSeconds(2)).ObserveOnDispatcher().Subscribe(e => SaveWindowsPosition());

            var obsWinResize = Observable.FromEventPattern<SizeChangedEventHandler, EventArgs>(h => SizeChanged += h, h => SizeChanged -= h);
            winResizeSub = obsWinResize.Throttle(TimeSpan.FromSeconds(2)).ObserveOnDispatcher().Subscribe(e => SaveWindowsPosition());

        }
    }
}
