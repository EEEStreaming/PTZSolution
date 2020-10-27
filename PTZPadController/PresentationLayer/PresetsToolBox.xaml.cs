using GalaSoft.MvvmLight.Ioc;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PTZPadController.PresentationLayer
{
    /// <summary>
    /// Logique d'interaction pour PresetsToolBox.xaml
    /// </summary>
    public partial class PresetsToolBox : Window
    {
        public PresetsToolBox()
        {
            InitializeComponent();


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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
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
            var winMoveSub = obsWinMove.Throttle(TimeSpan.FromSeconds(2)).ObserveOnDispatcher().Subscribe(e => SaveWindowsPosition());

            var obsWinResize = Observable.FromEventPattern<SizeChangedEventHandler, EventArgs>(h => SizeChanged += h, h => SizeChanged -= h);
            var winResizeSub = obsWinResize.Throttle(TimeSpan.FromSeconds(2)).ObserveOnDispatcher().Subscribe(e => SaveWindowsPosition());
        }
    }
}
