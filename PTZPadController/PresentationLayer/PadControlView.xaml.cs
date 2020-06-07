using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PTZPadController.PresentationLayer
{

    /// <summary>
    /// Represents the method that will handle the UpdatePositionEvent
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="value">Represents a displacement</param>
    /// <param name="direction">Provides direction (Both, X or Y)</param>
    public delegate void UpdatePositionEventHandler(object sender, Vector value, EnumDirection direction);

    public enum EnumDirection
    {
        Both,
        X,
        Y
    }

    /// <summary>
    /// Interaction logic for PadControlView.xaml
    /// </summary>
    public partial class PadControlView : UserControl
    {
        #region DataContext to Driver color
        
        public class ButtonArrowModel : INotifyPropertyChanged
        {

            public ButtonArrowModel()
            {
            }
           

 
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged(string p)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(p));
                }
            }

            internal static Color OnColor(double p)
            {
                return Color.FromArgb(255, 58, 241, 0);
            }
        }
        #endregion

        #region Data
        private Boolean IsMouseDown = false;
        private int IsMouseReseting = 0;
        private Boolean shiftPressed = false;        
        private EnumDirection direction = EnumDirection.Both;
        private const double precision = 0.1;
        private const int constBigArrows = 46;
        private const int constSmallArrows = 28;
        static private Point mCenter = new Point {X = 25, Y = 25 };

        private ButtonArrowModel xButton = new ButtonArrowModel();
        private ButtonArrowModel yButton = new ButtonArrowModel();
        private CancellationTokenSource _cancelTask;
        #endregion

        #region Properties
        private Boolean ControlPressed { get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); } }
        #endregion

        #region CONSTRUCTOR
        
        public PadControlView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                
            }

            arrowX.DataContext = xButton;
            arrowY.DataContext = yButton;
        }
        #endregion

        #region Event
        public event UpdatePositionEventHandler UpdatePositionEvent;

        internal void UpdatePosition(Vector value, EnumDirection direction)
        {
            if (_cancelTask != null)
                _cancelTask.Cancel(true);

            if (direction == EnumDirection.Both || direction == EnumDirection.X)
            {
            }

            if (direction == EnumDirection.Both || direction == EnumDirection.Y)
            {
                if (value.Y > 0.0)
                {
                 }
                else if (value.Y < 0.0)
                {
                }
            }      

            if (UpdatePositionEvent != null)
            {
                UpdatePositionEvent(this, value, direction);
            }
        }
        
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Change IsMouseDown state
        /// </summary>
        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (target.IsMouseOver)
            {
                IsMouseDown = true;                
                IsMouseReseting = 2;
                var setMouse = BorderSlider.PointToScreen(mCenter);
                SetCursorPos((int)setMouse.X, (int)setMouse.Y);                
                target.CaptureMouse();
            }
           
        }

        /// <summary>
        /// Change IsMouseDown state
        /// </summary>
        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
            IsMouseDown = false;
            IsMouseReseting = 0;
            shiftPressed = false;
            direction = EnumDirection.Both;
            target.ReleaseMouseCapture();
            arrowX.Visibility = System.Windows.Visibility.Visible;
            arrowY.Visibility = System.Windows.Visibility.Visible;

            arrowX.Width = constBigArrows;
            arrowY.Width = constBigArrows;

        }


        /// <summary>
        /// Simply grab a 1*1 pixel from the current color image, and
        /// use that and copy the new 1*1 image pixels to a byte array and
        /// then construct a Color from that.
        /// </summary>
        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (!IsMouseDown)                            
                return;                
            

            if (IsMouseReseting>0)
            {
                IsMouseReseting--;
                return;
            }

            
            if (!shiftPressed)
            {
                shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            }            


            try
            {
                IsMouseReseting++;
                var curPos = Mouse.GetPosition(BorderSlider);
                var setMouse = BorderSlider.PointToScreen(mCenter);                
                var value = curPos - mCenter;
                if (!(value.X == 0.0 && value.Y == 0.0))
                {                    
                    
                    if (shiftPressed)
                    {
                        if (direction == EnumDirection.Both)
                        {
                           //we must set a direction
                            if (Math.Abs(value.X) > Math.Abs(value.Y))
                            {
                                arrowY.Visibility = System.Windows.Visibility.Hidden;
                                direction = EnumDirection.X;
                            }
                            else
                            {
                                arrowX.Visibility = System.Windows.Visibility.Hidden;
                                direction = EnumDirection.Y;
                            }
                        }

                        if (direction == EnumDirection.X)
                            value.Y = 0.0;
                        else
                            value.X = 0.0;
                    }

                    if (ControlPressed)
                    {
                        arrowX.Width = constSmallArrows;
                        arrowY.Width = constSmallArrows;

                        value.X *= precision;
                        value.Y *= precision;
                    }
                    else
                    {
                        arrowX.Width = constBigArrows;
                        arrowY.Width = constBigArrows;
                    }

                    UpdatePosition(value, direction);
                }

                SetCursorPos((int)setMouse.X, (int)setMouse.Y);
                
            }
            catch
            {
                //not much we can do
            }
        }

        private void arrowX_Click(object sender, RoutedEventArgs e)
        {
            ArrowsClick(EnumDirection.X);            
        }

        private void arrowY_Click(object sender, RoutedEventArgs e)
        {
            ArrowsClick(EnumDirection.Y);
        }

        private void ArrowsClick(EnumDirection arrow)
        {
            if (!IsMouseDown)
            {
                var curPos = Mouse.GetPosition(BorderSlider);
                var setMouse = BorderSlider.PointToScreen(mCenter);
                var value = curPos - mCenter;
                if (ControlPressed)
                {
                    value.Y *= (precision*precision);
                    value.X *= (precision * precision);
                }
                else
                {
                    value.Y *= precision;
                    value.X *= precision;
                }
                if (arrow == EnumDirection.X)
                    value.Y = 0.0;
                else
                    value.X = 0.0;
                UpdatePosition(value, arrow);
            }
        }



        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        #endregion
    }
}
